using Domain.Common;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Employee : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public  string Email { get; set; }

        public string Password { get; set; }
        public Role Role { get; set; }
        public int AnnualLeaveAllowance { get; set; }
        public DateTime HireDate { get; set; }

        public int DepartmentId { get; set; }
        public Department Department { get; set; }

        public ICollection<LeaveRequest> LeaveRequests { get; set; }
    }
}
