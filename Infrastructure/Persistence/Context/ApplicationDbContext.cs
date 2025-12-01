using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Context
{// EF Core'un DbContext sınıfından miras alır ve bizim yazdığımız arayüzü uygular.
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        // Constructor: Veritabanı ayarlarını (ConnectionString vb.) dışarıdan alır.
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<LeaveType> LeaveTypes { get; set; }
        public DbSet<Department> Departments { get; set; }

        // Tablo ayarlarını (Configuration dosyalarını) devreye soktuğumuz yer.
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Bu satır, "Infrastructure" katmanındaki IEntityTypeConfiguration uygulayan 
            // TÜM sınıfları (EmployeeConfiguration vb.) otomatik bulur ve uygular.
            // Tek tek elle eklememize gerek kalmaz.
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(builder);
        }

        // SaveChanges metodunu eziyoruz (Override).
        // Amacımız: Kayıt eklerken veya güncellerken CreatedDate ve UpdateDate'i otomatik doldurmak.
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            // ChangeTracker: EF Core'un değişiklikleri izlediği mekanizma.
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedDate = DateTime.UtcNow; // Yeni kayıtsa oluşturma tarihi ata.
                        entry.Entity.IsDeleted = false; // Varsayılan silinmemiş.
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdateDate = DateTime.UtcNow; // Güncelleme ise güncelleme tarihi ata.
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}