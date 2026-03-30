using ProjectManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ProjectManager.Infrastructure.Persistence
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            DotNetEnv.Env.Load(FindEnvFile(Directory.GetCurrentDirectory()));

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            var dbUser = Environment.GetEnvironmentVariable("POSTGRES_USER");
            var dbPass = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
            var dbName = Environment.GetEnvironmentVariable("POSTGRES_DB");
            var dbHost = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost";
            var dbPort = Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432";

            var connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPass}";

            optionsBuilder.UseNpgsql(connectionString);

            return new ApplicationDbContext(optionsBuilder.Options);
        }

        private static string FindEnvFile(string currentPath)
        {
            var filePath = Path.Combine(currentPath, ".env");
            if (File.Exists(filePath)) return filePath;
            var parent = Directory.GetParent(currentPath);
            return parent == null ? currentPath : FindEnvFile(parent.FullName);
        }
    }
}
