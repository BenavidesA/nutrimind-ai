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

            // Esta es tu base de datos local para que la consola sepa dónde crearla directamente
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=NutriMindAI_DB;Trusted_Connection=True;MultipleActiveResultSets=true");

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}