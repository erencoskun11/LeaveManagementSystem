using System.Collections.Generic;

namespace Application.DTOs.LeaveRequest
{
    public class LeaveReportDto
    {
        public string EmployeeName { get; set; }
        public string DepartmentName { get; set; }
        public List<LeaveTypeStatDto> LeaveStats { get; set; } = new List<LeaveTypeStatDto>();
        public int TotalDays { get; set; }
        public int RemainingDays { get; set; }
    }
}