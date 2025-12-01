using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services; // <-- Bu using EKLİ OLMALI
using Application.Services;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services; // <-- Bu using EKLİ OLMALI
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. Veritabanı
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

            // 2. Repositories
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            // 3. Services (AuthService'i eklemiştik, LeaveRequestService EKSİKTİ)
            services.AddScoped<IAuthService, AuthService>();

            // --- İŞTE EKSİK OLAN SATIR BURASI: ---
            services.AddScoped<ILeaveRequestService, LeaveRequestService>();
            // -------------------------------------

            services.AddMemoryCache();
            services.AddScoped<ICacheService, CacheService>();

            // Notification Service Kaydı
            services.AddScoped<INotificationService, NotificationService>();


            return services;
        }
    }
}