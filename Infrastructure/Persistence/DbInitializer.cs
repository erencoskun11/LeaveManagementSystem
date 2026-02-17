using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(ApplicationDbContext context)
        {
            // 1. ÖNCE TABLOLARI OLUŞTUR (Migration)
            await context.Database.MigrateAsync();

            // 2. DEPARTMANLARI EKLE
            if (!context.Departments.Any())
            {
                var depts = new List<Department>
                {
                    new Department { Name = "Yönetim", Description = "Üst Düzey Yönetim", CreatedDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc) },
                    new Department { Name = "Yazılım (IT)", Description = "Teknoloji Ekibi", CreatedDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc) },
                    new Department { Name = "İnsan Kaynakları", Description = "Personel Yönetimi", CreatedDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc) },
                    new Department { Name = "Muhasebe", Description = "Finansal İşlemler", CreatedDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc) },
                    new Department { Name = "Satış & Pazarlama", Description = "Müşteri İlişkileri", CreatedDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc) }
                };
                await context.Departments.AddRangeAsync(depts);
                await context.SaveChangesAsync();
            }

            // 3. İZİN TÜRLERİNİ EKLE
            if (!context.LeaveTypes.Any())
            {
                var leaveTypes = new List<LeaveType>
                {
                    new LeaveType { Name = "Yıllık İzin", DefaultDays = 14, CreatedDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc) },
                    new LeaveType { Name = "Hastalık İzni", DefaultDays = 10, CreatedDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc) },
                    new LeaveType { Name = "Mazeret İzni", DefaultDays = 3, CreatedDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc) },
                    new LeaveType { Name = "Ücretsiz İzin", DefaultDays = 0, CreatedDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc) }
                };
                await context.LeaveTypes.AddRangeAsync(leaveTypes);
                await context.SaveChangesAsync();
            }

            // 4. VARSAYILAN ADMIN KULLANICISINI EKLE
            // E-posta kontrolü (küçük harf duyarlılığı için)
            if (!context.Employees.Any(e => e.Email == "eren1coskun11@gmail.com"))
            {
                // Admin'e atamak için bir departman bulalım (Genelde 'Yönetim' veya ilk sıradaki)
                var adminDept = await context.Departments.FirstOrDefaultAsync(d => d.Name == "Yönetim");
                if (adminDept == null) adminDept = await context.Departments.FirstOrDefaultAsync(); // Yoksa ilkini al

                int targetDeptId = adminDept != null ? adminDept.Id : 1; // Hiç yoksa 1 (Riskli ama migration varsa sorun olmaz)

                var admin = new Employee
                {
                    FirstName = "Eren",
                    LastName = "Coskun",
                    Email = "eren1coskun11@gmail.com",

                    // EKSİK OLAN ŞİFRE EKLENDİ
                    Password = "12345",

                    AnnualLeaveAllowance = 30,
                    Role = Role.Admin,

                    // EKSİK OLAN DEPARTMAN EKLENDİ
                    DepartmentId = targetDeptId,

                    // TARİHLER (UTC)
                    HireDate = DateTime.SpecifyKind(DateTime.UtcNow.AddYears(-2), DateTimeKind.Utc), // 2 yıllık çalışan gibi gösterelim
                    CreatedDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc),

                    IsDeleted = false
                };

                await context.Employees.AddAsync(admin);
                await context.SaveChangesAsync();
            }
        }
    }
}