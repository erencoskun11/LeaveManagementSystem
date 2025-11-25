using AutoMapper;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.DTOs.LeaveRequest;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Hubs; // NotificationHub burada olmalı
using Microsoft.AspNetCore.SignalR; // SignalR kütüphanesi
using Microsoft.Extensions.Caching.Memory; // Cache kütüphanesi

namespace Infrastructure.Services
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly IGenericRepository<Employee> _employeeRepository;
        private readonly IMapper _mapper;

        // EKLEME: Cache ve SignalR servisleri
        private readonly IMemoryCache _cache;
        private readonly IHubContext<NotificationHub> _hubContext;

        // Cache için sabit bir anahtar ismi
        private const string ALL_LEAVES_KEY = "all_leaves_cache";

        public LeaveRequestService(
            ILeaveRequestRepository leaveRequestRepository,
            IGenericRepository<Employee> employeeRepository,
            IMapper mapper,
            IMemoryCache cache,
            IHubContext<NotificationHub> hubContext)
        {
            _leaveRequestRepository = leaveRequestRepository;
            _employeeRepository = employeeRepository;
            _mapper = mapper;
            _cache = cache;
            _hubContext = hubContext;
        }

        // 1. İzin Talebi Oluşturma
        public async Task<int> CreateLeaveRequestAsync(CreateLeaveRequestDto request, string userEmail)
        {
            var employees = await _employeeRepository.GetAllAsync();
            var employee = employees.FirstOrDefault(x => x.Email == userEmail);

            if (employee == null) throw new Exception("Kullanıcı bulunamadı.");

            var requestedDays = (request.EndDate - request.StartDate).Days;
            if (requestedDays <= 0) throw new Exception("Bitiş tarihi başlangıç tarihinden sonra olmalıdır.");

            if (employee.AnnualLeaveAllowance < requestedDays)
                throw new Exception($"Yeterli izin hakkınız yok. Kalan: {employee.AnnualLeaveAllowance}, İstenen: {requestedDays}");

            var leaveRequest = _mapper.Map<LeaveRequest>(request);
            leaveRequest.EmployeeId = employee.Id;
            leaveRequest.RequestDate = DateTime.UtcNow;
            leaveRequest.Status = LeaveRequestStatus.Pending;

            await _leaveRequestRepository.AddAsync(leaveRequest);

            // --- CACHE & SIGNALR İŞLEMLERİ ---

            // 1. Veri değiştiği için eski Cache'i temizle (böylece liste güncellenir)
            _cache.Remove(ALL_LEAVES_KEY);

            // 2. Yöneticilere anlık bildirim gönder
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", $"{employee.FirstName} yeni bir izin talep etti.");

            return leaveRequest.Id;
        }

        // 2. Tüm İzinleri Listeleme (CACHE KULLANILDI)
        public async Task<List<LeaveRequestDetailDto>> GetAllLeaveRequestsAsync()
        {
            // Önce Cache'e bak: Veri var mı?
            if (_cache.TryGetValue(ALL_LEAVES_KEY, out List<LeaveRequestDetailDto> cachedList))
            {
                // Varsa direkt RAM'den döndür (Veritabanına gitmez, çok hızlıdır)
                return cachedList;
            }

            // Yoksa Veritabanından çek
            var requests = await _leaveRequestRepository.GetLeaveRequestsWithDetailsAsync();
            var dtoList = _mapper.Map<List<LeaveRequestDetailDto>>(requests);

            // Veriyi Cache'e kaydet (10 dakika boyunca sakla)
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(20))
                .SetPriority(CacheItemPriority.High);

            _cache.Set(ALL_LEAVES_KEY, dtoList, cacheOptions);

            return dtoList;
        }

        // 3. Kişisel İzinler (Kişiye özel olduğu için genellikle cachelenmez)
        public async Task<List<LeaveRequestDetailDto>> GetMyLeaveRequestsAsync(string userEmail)
        {
            var employees = await _employeeRepository.GetAllAsync();
            var employee = employees.FirstOrDefault(x => x.Email == userEmail);
            if (employee == null) throw new Exception("Kullanıcı bulunamadı.");

            // Repository metodunu kullan
            var requests = await _leaveRequestRepository.GetLeaveRequestsByEmployeeIdAsync(employee.Id);

            return _mapper.Map<List<LeaveRequestDetailDto>>(requests);
        }

        // 4. İzin Onaylama / Reddetme
        public async Task<bool> ManageLeaveRequestAsync(int requestId, bool approved, string managerEmail)
        {
            var leaveRequest = await _leaveRequestRepository.GetByIdAsync(requestId);
            if (leaveRequest == null) throw new Exception("İzin talebi bulunamadı.");

            leaveRequest.Status = approved ? LeaveRequestStatus.Approved : LeaveRequestStatus.Rejected;

            await _leaveRequestRepository.UpdateAsync(leaveRequest);

            if (approved)
            {
                var employee = await _employeeRepository.GetByIdAsync(leaveRequest.EmployeeId);
                int days = (leaveRequest.EndDate - leaveRequest.StartDate).Days;
                if (days > 0)
                {
                    employee.AnnualLeaveAllowance -= days;
                    await _employeeRepository.UpdateAsync(employee);
                }
            }

            // --- CACHE & SIGNALR İŞLEMLERİ ---

            // 1. Veri değişti, Cache'i temizle ki liste güncellensin
            _cache.Remove(ALL_LEAVES_KEY);

            // 2. Çalışana bildirim gönder
            var statusMsg = approved ? "Onaylandı" : "Reddedildi";
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", $"İzin talebiniz {statusMsg}!");

            return true;
        }
    }
}