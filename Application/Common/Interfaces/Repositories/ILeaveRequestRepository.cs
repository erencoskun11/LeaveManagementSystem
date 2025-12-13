using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces.Repositories
{
    public interface ILeaveRequestRepository : IGenericRepository<LeaveRequest>
    {
        Task<List<LeaveRequest>> GetLeaveRequestsWithDetailsAsync();
        Task<LeaveRequest> GetLeaveRequestWithDetailsByIdAsync(int id);
        Task<List<LeaveRequest>> GetLeaveRequestsByEmployeeIdAsync(int employeeId);
        Task<List<LeaveRequest>> GetApprovedLeaveRequestsByDateRangeAsync(DateTime startDate, DateTime endDate, int? employeeId);
    }
}
