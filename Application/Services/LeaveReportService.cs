using Application.Common.Extensions; // Extension metodun burada
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.DTOs.LeaveRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class LeaveReportService : ILeaveReportService
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;

        public LeaveReportService(ILeaveRequestRepository leaveRequestRepository)
        {
            _leaveRequestRepository = leaveRequestRepository;
        }

        public async Task<List<LeaveReportDto>> GetLeaveReportAsync(DateTime startDate, DateTime endDate, int? employeeId)
        {
            // 1. ADIM: REPOSITORY'DEN FİLTRELENMİŞ VERİYİ ÇEK (Database seviyesinde filtreleme)
            // GetAll yapıp RAM'e yüklemek yerine, sadece gerekeni çekiyoruz.
            var approvedLeaves = await _leaveRequestRepository.GetApprovedLeaveRequestsByDateRangeAsync(startDate, endDate, employeeId);

            // 2. ADIM: GRUPLAMA VE DTO'YA ÇEVİRME
            // Veri zaten az ve filtrelenmiş olduğu için bu işlemi burada yapmak güvenlidir.
            var reportData = approvedLeaves
                .GroupBy(x => x.EmployeeId)
                .Select(g =>
                {
                    var first = g.First();
                    string dept = (first.Employee?.Department != null) ? first.Employee.Department.Name : "-";
                    int rem = first.Employee != null ? first.Employee.AnnualLeaveAllowance : 0;
                    string empName = first.Employee != null ? $"{first.Employee.FirstName} {first.Employee.LastName}" : "Bilinmeyen";

                    return new LeaveReportDto
                    {
                        EmployeeName = empName,
                        DepartmentName = dept,
                        RemainingDays = rem,

                        // Extension metodunu burada kullanıyoruz (Hafta sonları hariç toplam)
                        TotalDays = g.Sum(x => x.StartDate.CalculateBusinessDays(x.EndDate)),

                        // İzin türlerine göre detaylı dağılım
                        LeaveStats = g.GroupBy(t => t.LeaveType.Name)
                                      .Select(t => new LeaveTypeStatDto
                                      {
                                          LeaveTypeName = t.Key,
                                          // Her bir tür için de iş günü hesabı
                                          DaysUsed = t.Sum(x => x.StartDate.CalculateBusinessDays(x.EndDate))
                                      }).ToList()
                    };
                })
                .ToList();

            return reportData;
        }
    }
}