using Application.DTOs.Department;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces.Services
{
    public interface IDepartmentService
    {
        Task<List<DepartmentDto>> GetAllDepartmentsAsync();

        Task<DepartmentDto> GetDepartmentByIdAsync(int departmentId);
        Task CreateDepartmentAsync(CreateDepartmentDto departmentDto);

        Task UpdateDepartmentAsync(UpdateDepartmentDto departmentDto);

        Task DeleteDepartmentAsync(int departmentId);

    }
}
