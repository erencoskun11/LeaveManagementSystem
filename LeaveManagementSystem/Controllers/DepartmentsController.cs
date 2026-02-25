using Application.Common.Interfaces.Services;
using Application.DTOs.Department;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LeaveManagementSystem.API.Controllers
{
    [Authorize]
    [Route("api/LeaveRequests")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentsController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet("departments")]
        public async Task<IActionResult> GetDepartments()
        {
            var depts = await _departmentService.GetAllDepartmentsAsync();
            return Ok(depts);
        }

        [Authorize(Roles = "Admin,2")]
        [HttpPost("create-department")]
        public async Task<ActionResult> CreateDepartment([FromBody] CreateDepartmentDto request)
        {
            await _departmentService.CreateDepartmentAsync(request);
            return Ok(new { message = "Departman eklendi." });
        }

        [Authorize(Roles = "Admin,2")]
        [HttpPut("update-department")]
        public async Task<ActionResult> UpdateDepartment([FromBody] UpdateDepartmentDto request)
        {
            await _departmentService.UpdateDepartmentAsync(request);
            return Ok(new { message = "Güncellendi." });
        }

        [Authorize(Roles = "Admin,2")]
        [HttpDelete("delete-department/{id}")]
        public async Task<ActionResult> DeleteDepartment(int id)
        {
            await _departmentService.DeleteDepartmentAsync(id);
            return Ok(new { message = "Silindi." });
        }

        [Authorize(Roles = "Admin,2")]
        [HttpGet("get-department/{id}")]
        public async Task<ActionResult> GetDepartment(int id)
        {
            var dept = await _departmentService.GetDepartmentByIdAsync(id);

          
            if (dept == null) return NotFound();

            return Ok(dept);
        }
    }
}