namespace Server
{
    using System.Globalization;
    using DataAccessLayer;
    using Microsoft.AspNetCore.Authentication.BearerToken;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.OpenApi.Models;
    using Server.Filters;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc((x) => { x.EnableEndpointRouting = false; });
            services.AddSingleton<ExceptionFilter>();

            // userroles sind ausgeschaltet für jetzt, da es ArgumentNullException verursacht, da bei der erstellung
            // ein neuen user versucht MyUserStore eine role hunzifügen, die es noch nicht existiert.
            // services.AddTransient<IUserStore<ApplicationUser>, MyUserStore>();
            services.AddControllers(o =>
            {
                o.Filters.AddService<ExceptionFilter>();
            });
            services.AddControllers().AddXmlSerializerFormatters();
            services.AddDbContext<IDatabaseContext, DatabaseContext>(options =>
#if !DEBUG
                options.UseSqlServer(Configuration["DatabaseInfo:ConnectionString"], o => o.EnableRetryOnFailure()));
#else
                 options.UseSqlServer(
                     Configuration["DatabaseInfo:LocalConnectionString"], o => o.EnableRetryOnFailure()));
#endif
            services.AddAuthentication().AddBearerToken();
            services.AddOptions<BearerTokenOptions>(IdentityConstants.BearerScheme).Configure(o =>
            {
                o.BearerTokenExpiration = TimeSpan.FromDays(10);
                o.RefreshTokenExpiration = TimeSpan.FromDays(30);
            });
            services.AddAuthorization();
            services.AddIdentityApiEndpoints<ApplicationUser>(o =>
            {
                o.Password.RequiredLength = 5;
                o.User.RequireUniqueEmail = true;
                o.Password.RequireNonAlphanumeric = false;
            })

                // .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<DatabaseContext>()
            .AddDefaultTokenProviders();
            services.AddHttpContextAccessor();
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
                c.OperationFilter<SwaggerParameterIgnoreFilter>();
                c.EnableAnnotations();
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope())
            {
                var context = serviceScope?.ServiceProvider.GetRequiredService<DatabaseContext>();

                if (!context!.Database.IsInMemory())
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
                endpoints.MapIdentityApi<ApplicationUser>();
                endpoints.MapSwagger();
            });
            app.UseMvc();
        }
    }
}
