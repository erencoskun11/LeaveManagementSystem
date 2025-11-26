using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Auth
{
    public class RegisterRequestDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int AnnualLeaveAllowance { get; set; }
        public Role Role { get; set; } // Employee, HR
        public int DepartmentId { get; set; }
        public DateTime HireDate { get; set; }
    }
}
