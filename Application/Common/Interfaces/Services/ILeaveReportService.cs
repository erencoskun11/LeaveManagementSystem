using Application.DTOs.LeaveRequest; // DTO namespace'ine dikkat et
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Common.Interfaces.Services
{
    public interface ILeaveReportService
    {
        Task<List<LeaveReportDto>> GetLeaveReportAsync(DateTime startDate, DateTime endDate, int? employeeId);
    }
}