using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.LeaveRequest
{
    public class LeaveRequestDto
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Detaylı isimler yerine sadece ID'leri ve temel bilgileri tutar
        public int LeaveTypeId { get; set; }
        public int EmployeeId { get; set; }

        public DateTime? RequestDate { get; set; }
        public string RequestComments { get; set; }

        public LeaveRequestStatus Status { get; set; }
    }
}
