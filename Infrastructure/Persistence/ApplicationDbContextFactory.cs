using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // HATA ÇÖZÜMÜ: appsettings.json okumaya çalışmak yerine string'i doğrudan buraya yazıyoruz.
            // Bu yöntem, dosya yolu hatalarını %100 engeller.
            var connectionString = "Host=localhost;Port=5432;Database=LeaveManagementDb;Username=postgres;Password=guest";

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}