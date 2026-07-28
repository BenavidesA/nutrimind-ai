using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using NutriMind.Infrastructure.Persistence.Context;

namespace NutriMind.Infrastructure.Persistence
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            // This is the local database so the console tool knows where to create it directly
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=NutriMindAI_DB;Trusted_Connection=True;MultipleActiveResultSets=true");

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}