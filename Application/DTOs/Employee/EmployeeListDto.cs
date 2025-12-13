using System;

namespace Application.DTOs.Employee
{
    public class EmployeeListDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int Role { get; set; }
        public string DepartmentName { get; set; } // İlişkili tablodan gelecek
        public DateTime HireDate { get; set; }
    }
}