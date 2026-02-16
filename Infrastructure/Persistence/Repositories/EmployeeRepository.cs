using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(ApplicationDbContext context) : base(context)
        {
        }

       

        public async Task<Employee> GetByEmailAsync(string email)
        {
            return await _context.Employees.AsNoTracking()
                .Include(e=>e.Department)
                .FirstOrDefaultAsync(e => e.Email == email);
        }

        public async Task<List<Employee>> GetEmployeesWithDetailsAsync()
        {
            return await _context.Employees.AsNoTracking()
                .Include(x=>x.Department)
                .Where(e => !e.IsDeleted)
                .ToListAsync();

        }
    }
}
