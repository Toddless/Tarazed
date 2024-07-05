namespace Server
{
    using System.Globalization;
    using DataAccessLayer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.OpenApi.Models;
    using Server.Filters;
    using Server.Resources;

    public class Startup
    {
        private readonly MyConfigKeys _config;

        public Startup(IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);
            Configuration = configuration;
            _config = new MyConfigKeys(configuration);
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc((x) => { x.EnableEndpointRouting = false; });
            services.AddSingleton(_config);
            services.AddSingleton<ExceptionFilter>();
            services.AddTransient<IUserStore<IdentityUser>, MyUserStore>();
            services.AddControllers(o =>
            {
                o.Filters.AddService<ExceptionFilter>();
            });
            services.AddControllers().AddXmlSerializerFormatters();
            services.AddDbContext<IDatabaseContext, DatabaseContext>(options =>
#if !DEBUG
                options.UseSqlServer(Configuration["DatabaseInfo:ConnectionString"], o => o.EnableRetryOnFailure()));
#else
                options.UseSqlServer(Configuration["DatabaseInfo:LocalConnectionString"], o => o.EnableRetryOnFailure()));
#endif

            #region 
            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            //{
            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuer = true,
            //        ValidateAudience = true,
            //        ValidateLifetime = true,
            //        ValidateIssuerSigningKey = true,
            //        ValidIssuer = _config.JWTIssuer,
            //        ValidAudience = _config.JWTIssuer,
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.JWTKey)),
            //    };
            //})
            #endregion

            services.AddAuthentication().AddJwtBearer();
            services.AddAuthorization();
            services.AddIdentityApiEndpoints<IdentityUser>(o =>
            {
                o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(20);
                o.Password.RequiredLength = 5;
                o.User.RequireUniqueEmail = true;
                o.Password.RequireNonAlphanumeric = false;
                o.SignIn.RequireConfirmedPhoneNumber = false;
            }).AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<DatabaseContext>()
            .AddDefaultTokenProviders();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen((c) =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Sample API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    In = ParameterLocation.Header,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                        },
                        new string[] { }
                    },
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope())
            {
                var context = serviceScope?.ServiceProvider.GetRequiredService<DatabaseContext>();

                // context?.Database.EnsureCreated();
                if (!context.Database.IsInMemory())
                {
                    context?.Database.Migrate();
                }
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My Api V1");
                });
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                ApplyCurrentCultureToResponseHeaders = true,
                DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en"),
                SupportedUICultures = new List<CultureInfo>
                {
                    new CultureInfo("en"),
                    new CultureInfo("de"),
                },
                SupportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("en"),
                    new CultureInfo("de"),
                },
            });
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapIdentityApi<IdentityUser>();
                endpoints.MapSwagger();
            });
            app.UseMvc();

            CreateRolesAsync(serviceProvider).Wait();
        }

        public async Task CreateRolesAsync(IServiceProvider service)
        {
            var roleManager = service.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = service.GetRequiredService<UserManager<IdentityUser>>();
            string[] roleNames = { "Admin", "User", "Trainer" };
            IdentityResult roleResult;
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            var supUser = new IdentityUser
            {
                UserName = Configuration["AppSettings:UserName"],
                Email = Configuration["AppSettings:UserEmail"],
            };

            string userPwd = Configuration["AppSettings:UserPassword"] ?? string.Empty;
            var user = await userManager.FindByEmailAsync(Configuration["AppSettings:AdminUserEmail"] ?? string.Empty);

            if (user == null)
            {
                var createSupUser = await userManager.CreateAsync(supUser, userPwd);
                if (createSupUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(supUser, "Admin");
                }
            }
        }
    }
}
