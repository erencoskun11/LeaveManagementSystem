using System;

namespace Application.DTOs.LeaveRequest
{
    public class UpdateLeaveRequestDto
    {
        public int Id { get; set; } // Hangi izni güncelliyoruz?
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int LeaveTypeId { get; set; }
        public string? RequestComments { get; set; }
    }
}