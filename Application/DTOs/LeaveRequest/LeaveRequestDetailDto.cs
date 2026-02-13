using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.LeaveRequest
{
    public class LeaveRequestDetailDto
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; } 
        public string EmployeeName { get; set; }
        public string LeaveTypeName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public DateTime RequestDate { get; set; }
        public string RequestComments { get; set; } // Açıklama alanı
        public string Department_Name { get; set; }
    }
}
