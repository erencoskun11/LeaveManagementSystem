using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.LeaveRequest
{
    public class LeaveTypeStatDto
    {
        public string LeaveTypeName { get; set; }
        public int DaysUsed { get; set; }

        public List<LeaveTypeStatDto> LeaveStats { get; set; } = new List<LeaveTypeStatDto>();

        public int TotalDays { get; set; }



    }
}
