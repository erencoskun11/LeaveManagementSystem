using Application.DTOs.Employee; 
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Common.Interfaces.Services
{
    public interface IEmployeeService
    {
        Task<List<EmployeeListDto>> GetAllEmployeesAsync();

        Task<EmployeeDetailDto> GetEmployeeByIdAsync(int id);

        Task UpdateEmployeeAsync(UpdateEmployeeDto request);

        // Silme işlemi (Soft Delete)
        Task DeleteEmployeeAsync(int id);
    }
}