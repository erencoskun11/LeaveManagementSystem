using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class DepartmentRepository : GenericRepository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(ApplicationDbContext context) : base(context)
        {
        }

      

        public async Task<bool> HasEmployeesAsync(int departmentId)
        {
            return await _context.Employees.AsNoTracking().AnyAsync(x => x.DepartmentId == departmentId && !x.IsDeleted);
        }
    }
}