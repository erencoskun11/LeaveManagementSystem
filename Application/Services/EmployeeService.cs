using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.DTOs.Employee;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        // Özel Repository'mizi kullanıyoruz (Include işlemleri için)
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        public EmployeeService(
            IEmployeeRepository employeeRepository,
            IMapper mapper,
            INotificationService notificationService)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        // 1. LİSTELEME
        public async Task<List<EmployeeListDto>> GetAllEmployeesAsync()
        {
            // Repository'deki özel metodu çağırıyoruz (Departman verisi dolu gelir)
            var employees = await _employeeRepository.GetEmployeesWithDetailsAsync();

            // Entity -> DTO dönüşümü
            return _mapper.Map<List<EmployeeListDto>>(employees);
        }

        // 2. DETAY GETİRME
        public async Task<EmployeeDetailDto> GetEmployeeByIdAsync(int id)
        {
            var emp = await _employeeRepository.GetByIdAsync(id);

            // Eğer kayıt yoksa veya silinmişse null dön
            if (emp == null || emp.IsDeleted) return null;

            return _mapper.Map<EmployeeDetailDto>(emp);
        }

        // 3. GÜNCELLEME
        public async Task UpdateEmployeeAsync(UpdateEmployeeDto request)
        {
            var emp = await _employeeRepository.GetByIdAsync(request.Id);
            if (emp == null) throw new Exception("Çalışan bulunamadı.");

            // Manuel Mapping (veya AutoMapper kullanılabilir ama hassas ayarlar var)
            emp.FirstName = request.FirstName;
            emp.LastName = request.LastName;
            emp.Email = request.Email;
            emp.Role = (Role)request.Role; // Enum dönüşümü
            emp.AnnualLeaveAllowance = request.AnnualLeaveAllowance;
            emp.DepartmentId = request.DepartmentId;

            // Tarih varsa UTC'ye çevirip kaydet
            if (request.HireDate.HasValue)
                emp.HireDate = DateTime.SpecifyKind(request.HireDate.Value, DateTimeKind.Utc);

            // Şifre boş gelmediyse güncelle (Normalde burada Hashleme yapılmalı!)
            if (!string.IsNullOrEmpty(request.Password))
                emp.Password = request.Password;

            // Güncelleme tarihini at
            emp.UpdateDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

            await _employeeRepository.UpdateAsync(emp);

          
            await _notificationService.SendLeaveUpdate();
        }

        public async Task DeleteEmployeeAsync(int id)
        {
            var emp = await _employeeRepository.GetByIdAsync(id);
            if (emp == null) throw new Exception("Çalışan bulunamadı.");

            emp.IsDeleted = true;

            await _employeeRepository.UpdateAsync(emp);

            await _notificationService.SendLeaveUpdate();
        }
    }
}