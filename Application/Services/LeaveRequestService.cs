using AutoMapper;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.DTOs.LeaveRequest;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Extensions; // Extension metodun bulunduğu namespace

namespace Application.Services
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly IGenericRepository<Employee> _employeeRepository;
        private readonly IGenericRepository<LeaveType> _leaveTypeRepository;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly INotificationService _notificationService;

        private const string ALL_LEAVES_KEY = "all_leaves_cache";

        public LeaveRequestService(
            ILeaveRequestRepository leaveRequestRepository,
            IGenericRepository<Employee> employeeRepository,
            IGenericRepository<LeaveType> leaveTypeRepository,
            IMapper mapper,
            IMemoryCache cache,
            INotificationService notificationService)
        {
            _leaveRequestRepository = leaveRequestRepository;
            _employeeRepository = employeeRepository;
            _leaveTypeRepository = leaveTypeRepository;
            _mapper = mapper;
            _cache = cache;
            _notificationService = notificationService;
        }

        public async Task<int> CreateLeaveRequestAsync(CreateLeaveRequestDto request, string userEmail)
        {
            var allEmployees = await _employeeRepository.GetAllAsync();
            var employee = allEmployees.FirstOrDefault(x => x.Email.ToLower().Trim() == userEmail.ToLower().Trim());
            if (employee == null) throw new Exception("Kullanıcı bulunamadı.");

            var leaveType = await _leaveTypeRepository.GetByIdAsync(request.LeaveTypeId);
            if (leaveType == null) throw new Exception("İzin türü bulunamadı.");

            
            int requestedBusinessDays = request.StartDate.CalculateBusinessDays(request.EndDate);

            // Bakiye kontrolü (Sadece Yıllık İzin ise)
            if (leaveType.Name.ToLower().Contains("yıllık"))
            {
                if (requestedBusinessDays > employee.AnnualLeaveAllowance)
                {
                    throw new Exception($"Yetersiz bakiye! Mevcut hakkınız: {employee.AnnualLeaveAllowance} gün, İstenen (İş Günü): {requestedBusinessDays} gün.");
                }
            }

            var leaveRequest = _mapper.Map<LeaveRequest>(request);
            leaveRequest.EmployeeId = employee.Id;
            leaveRequest.Status = LeaveRequestStatus.Pending;
            leaveRequest.StartDate = DateTime.SpecifyKind(request.StartDate, DateTimeKind.Utc);
            leaveRequest.EndDate = DateTime.SpecifyKind(request.EndDate, DateTimeKind.Utc);
            leaveRequest.RequestDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
            leaveRequest.CreatedDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

            await _leaveRequestRepository.AddAsync(leaveRequest);
            _cache.Remove(ALL_LEAVES_KEY);
            await _notificationService.SendNotification($"{employee.FirstName} yeni izin talep etti.");
            await _notificationService.SendLeaveUpdate();

            return leaveRequest.Id;
        }

        public async Task<List<LeaveRequestDetailDto>> GetMyLeaveRequestsAsync(string userEmail)
        {
            var allEmployees = await _employeeRepository.GetAllAsync();
            var employee = allEmployees.FirstOrDefault(x => x.Email.ToLower().Trim() == userEmail.ToLower().Trim());
            if (employee == null) return new List<LeaveRequestDetailDto>();

            var allRequests = await _leaveRequestRepository.GetLeaveRequestsWithDetailsAsync();
            var myRequests = allRequests.Where(x => x.EmployeeId == employee.Id).OrderByDescending(x => x.StartDate);

            return _mapper.Map<List<LeaveRequestDetailDto>>(myRequests);
        }

        public async Task<List<LeaveRequestDetailDto>> GetAllLeaveRequestsAsync()
        {
            if (_cache.TryGetValue(ALL_LEAVES_KEY, out List<LeaveRequestDetailDto> cachedList))
                return cachedList;

            var requests = await _leaveRequestRepository.GetLeaveRequestsWithDetailsAsync();
            var dtoList = _mapper.Map<List<LeaveRequestDetailDto>>(requests);

            _cache.Set(ALL_LEAVES_KEY, dtoList, TimeSpan.FromMinutes(10));
            return dtoList;
        }

        public async Task<bool> UpdateLeaveRequestAsync(UpdateLeaveRequestDto request, string userEmail)
        {
            var leaveRequest = await _leaveRequestRepository.GetByIdAsync(request.Id);
            if (leaveRequest == null) throw new Exception("İzin bulunamadı.");

            var allEmployees = await _employeeRepository.GetAllAsync();
            var user = allEmployees.FirstOrDefault(x => x.Email.ToLower().Trim() == userEmail.ToLower().Trim());

            if (user.Role != Role.Admin && user.Role != (Role)2 && leaveRequest.EmployeeId != user.Id)
                throw new Exception("Yetkisiz işlem.");

            if (leaveRequest.Status != LeaveRequestStatus.Pending)
                throw new Exception("Sadece bekleyen talepler güncellenebilir.");

            leaveRequest.StartDate = DateTime.SpecifyKind(request.StartDate, DateTimeKind.Utc);
            leaveRequest.EndDate = DateTime.SpecifyKind(request.EndDate, DateTimeKind.Utc);
            leaveRequest.LeaveTypeId = request.LeaveTypeId;
            leaveRequest.RequestComments = request.RequestComments;
            leaveRequest.UpdateDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

            await _leaveRequestRepository.UpdateAsync(leaveRequest);
            _cache.Remove(ALL_LEAVES_KEY);
            await _notificationService.SendLeaveUpdate();

            return true;
        }

        public async Task<bool> DeleteLeaveRequestAsync(int id, string userEmail, string userRole)
        {
            var leaveRequest = await _leaveRequestRepository.GetByIdAsync(id);
            if (leaveRequest == null) throw new Exception("İzin bulunamadı.");

            bool isAdmin = (userRole == "Admin" || userRole == "2");

            if (!isAdmin)
            {
                var allEmployees = await _employeeRepository.GetAllAsync();
                var user = allEmployees.FirstOrDefault(x => x.Email.ToLower().Trim() == userEmail.ToLower().Trim());
                if (user == null || leaveRequest.EmployeeId != user.Id) throw new Exception("Başkasının iznini silemezsiniz.");
                if (leaveRequest.Status != LeaveRequestStatus.Pending) throw new Exception("İşlem görmüş kayıt silinemez.");
            }

            // --- İADE MANTIĞI GÜNCELLENDİ (Sadece İş Günleri) ---
            if (leaveRequest.Status == LeaveRequestStatus.Approved)
            {
                var leaveType = await _leaveTypeRepository.GetByIdAsync(leaveRequest.LeaveTypeId);
                if (leaveType != null && leaveType.Name.ToLower().Contains("yıllık"))
                {
                    var employee = await _employeeRepository.GetByIdAsync(leaveRequest.EmployeeId);
                    if (employee != null)
                    {
                        // Sadece iş günlerini iade et
                        int daysToRefund = leaveRequest.StartDate.CalculateBusinessDays(leaveRequest.EndDate); // <-- DÜZELTME
                        employee.AnnualLeaveAllowance += daysToRefund;
                        await _employeeRepository.UpdateAsync(employee);
                    }
                }
            }
            // ----------------------------------------------------

            leaveRequest.IsDeleted = true;
            await _leaveRequestRepository.UpdateAsync(leaveRequest);
            _cache.Remove(ALL_LEAVES_KEY);
            await _notificationService.SendLeaveUpdate();

            return true;
        }

        // 5. ONAY/RED (DÜŞME MANTIĞI)
        public async Task<bool> ManageLeaveRequestAsync(int requestId, bool approved, string managerEmail)
        {
            var leaveRequest = await _leaveRequestRepository.GetByIdAsync(requestId);
            if (leaveRequest == null) throw new Exception("İzin bulunamadı.");

            leaveRequest.Status = approved ? LeaveRequestStatus.Approved : LeaveRequestStatus.Rejected;
            await _leaveRequestRepository.UpdateAsync(leaveRequest);

            if (approved)
            {
                var leaveType = await _leaveTypeRepository.GetByIdAsync(leaveRequest.LeaveTypeId);

                if (leaveType != null && leaveType.Name.ToLower().Contains("yıllık"))
                {
                    var employee = await _employeeRepository.GetByIdAsync(leaveRequest.EmployeeId);

                    // Sadece iş günlerini hesapla
                    int daysToDeduct = leaveRequest.StartDate.CalculateBusinessDays(leaveRequest.EndDate); // <-- DÜZELTME

                    if (daysToDeduct > 0)
                    {
                        employee.AnnualLeaveAllowance -= daysToDeduct;
                        await _employeeRepository.UpdateAsync(employee);
                    }
                }
            }
            // -----------------------------------------------------

            _cache.Remove(ALL_LEAVES_KEY);
            await _notificationService.SendLeaveUpdate();

            return true;
        }

         
    }
}