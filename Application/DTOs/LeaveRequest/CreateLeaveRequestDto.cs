using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.LeaveRequest
{
    public class CreateLeaveRequestDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int LeaveTypeId { get; set; }
        public string? RequestComments { get; set; }

        // YENİ EKLENEN: Admin başkası için izin girebilsin diye
        public string? EmployeeId { get; set; } // String veya int (Senin Employee tablon int ise int yap)
    }
}
