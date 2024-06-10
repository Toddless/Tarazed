namespace Server
{
    using DataAccessLayer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.OpenApi.Models;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc((x) => { x.EnableEndpointRouting = false; });
            services.AddControllers().AddXmlSerializerFormatters();

            services.AddDbContext<IDatabaseContext, DatabaseContext>(options =>
                options.UseSqlServer("Server=tcp:tarazed.database.windows.net,1433;Initial Catalog=TarazedDatabase;Persist Security Info=False;User ID=Odmin;Password=Dsad123cxz@12W;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;", o => o.EnableRetryOnFailure()));
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen((c) =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Sample API", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<DatabaseContext>();
                context.Database.EnsureCreated();
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

            app.UseRouting();
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.UseMvc();
        }
    }
}
