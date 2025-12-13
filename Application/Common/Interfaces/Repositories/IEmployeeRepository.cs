using Domain.Entities;
using System.Threading.Tasks;

namespace Application.Common.Interfaces.Repositories
{
    public interface IEmployeeRepository : IGenericRepository<Employee>
    {
        Task<Employee> GetByEmailAsync(string email);
        Task<List<Employee>> GetEmployeesWithDetailsAsync();
    }
}