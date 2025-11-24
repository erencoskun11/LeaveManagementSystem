using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Employee>Employees { get; }
        DbSet<LeaveRequest> LeaveRequests { get; }
        DbSet<LeaveType>LeaveTypes { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
