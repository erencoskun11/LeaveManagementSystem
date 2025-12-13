using System;

namespace Application.DTOs.Employee
{
    public class EmployeeDetailDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int Role { get; set; }
        public int AnnualLeaveAllowance { get; set; }
        public int DepartmentId { get; set; }
        public DateTime HireDate { get; set; }
    }
}