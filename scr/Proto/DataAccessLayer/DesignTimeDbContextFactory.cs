namespace Server
{
    using DataAccessLayer;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Configuration;

    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddUserSecrets("c1e371db-001d-43ad-adc6-bc53b7e7ab88", false)
            .Build();

            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
            optionsBuilder.UseSqlServer(configuration["DatabaseInfo:ConnectionString"]);

            return new DatabaseContext(optionsBuilder.Options);
        }
    }
}
