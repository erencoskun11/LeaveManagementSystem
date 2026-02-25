using Application.Common.Interfaces.Services;
using Application.DTOs.Employee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LeaveManagementSystem.API.Controllers
{
    [Authorize]
    [Route("api/LeaveRequests")] 
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet("employees")]
        public async Task<IActionResult> GetEmployees()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            return Ok(employees);
        }

        [Authorize(Roles = "Admin,HR,2,3")] 
        [HttpGet("get-employee/{id}")]
        public async Task<ActionResult> GetEmployee(int id)
        {
            var emp = await _employeeService.GetEmployeeByIdAsync(id);

            if (emp == null) return NotFound();

            return Ok(emp);
        }

        [Authorize(Roles = "Admin,HR,2,3")]
        [HttpPut("update-employee")]
        public async Task<ActionResult> UpdateEmployee([FromBody] UpdateEmployeeDto request)
        {
            await _employeeService.UpdateEmployeeAsync(request);

            return Ok(new { message = "Güncellendi." });
        }

        [Authorize(Roles = "Admin,HR,2,3")]
        [HttpDelete("delete-employee/{id}")]
        public async Task<ActionResult> DeleteEmployee(int id)
        {
            await _employeeService.DeleteEmployeeAsync(id);

            return Ok(new { message = "Silindi." });
        }
    }
}