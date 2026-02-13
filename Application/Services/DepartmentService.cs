
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.DTOs.Department;
using AutoMapper;
using Domain.Entities; // Entity'ye erişim için gerekli
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService; // EKLENDİ

        // Constructor'a notificationService parametresi eklendi
        public DepartmentService(
            IDepartmentRepository departmentRepository,
            IMapper mapper,
            INotificationService notificationService)
        {
            _departmentRepository = departmentRepository;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        // 1. LİSTELEME
        public async Task<List<DepartmentDto>> GetAllDepartmentsAsync()
        {
            var depts = await _departmentRepository.GetAllAsync();
            // Silinmemişleri filtrele
            var activeDepts = depts.Where(x => !x.IsDeleted).ToList();
            return _mapper.Map<List<DepartmentDto>>(activeDepts);
        }

        // 2. TEK KAYIT GETİRME (Eksikti, eklendi)
        public async Task<DepartmentDto> GetDepartmentByIdAsync(int id)
        {
            var dept = await _departmentRepository.GetByIdAsync(id);
            if (dept == null || dept.IsDeleted) return null;
            return _mapper.Map<DepartmentDto>(dept);
        }

        // 3. OLUŞTURMA
        public async Task CreateDepartmentAsync(CreateDepartmentDto request)
        {
            // HATA DÜZELTİLDİ: DTO -> Entity dönüşümü yapılmalı
            var dept = _mapper.Map<Department>(request);

            dept.CreatedDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
            dept.IsDeleted = false;

            await _departmentRepository.AddAsync(dept);

            // SignalR ile frontend'i güncelle
            await _notificationService.SendDepartmentUpdate();
        }

        // 4. GÜNCELLEME
        public async Task UpdateDepartmentAsync(UpdateDepartmentDto request)
        {
            var dept = await _departmentRepository.GetByIdAsync(request.Id);
            if (dept == null) throw new Exception("Departman bulunamadı.");

            // Manuel güncelleme (Mapper da kullanılabilir ama 2 alan için gerek yok)
            dept.Name = request.Name;
            dept.Description = request.Description;
            dept.UpdateDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

            await _departmentRepository.UpdateAsync(dept);

            // SignalR ile frontend'i güncelle
            await _notificationService.SendDepartmentUpdate();
        }

        // 5. SİLME (Kontrollü Silme)
        public async Task DeleteDepartmentAsync(int id)
        {
            // Önce: Bu departmanda çalışan var mı?
            bool hasEmployees = await _departmentRepository.HasEmployeesAsync(id);
            if (hasEmployees) throw new Exception("Bu departmanda çalışanlar var, silinemez!");

            var dept = await _departmentRepository.GetByIdAsync(id);
            if (dept == null) throw new Exception("Departman bulunamadı.");

            // Soft Delete (Veriyi tamamen silme, işaretle)
            dept.IsDeleted = true;

            await _departmentRepository.UpdateAsync(dept);

            // SignalR ile frontend'i güncelle
            await _notificationService.SendDepartmentUpdate();
        }
    }
}