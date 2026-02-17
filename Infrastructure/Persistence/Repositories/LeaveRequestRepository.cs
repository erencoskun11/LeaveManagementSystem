using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class LeaveRequestRepository : GenericRepository<LeaveRequest>, ILeaveRequestRepository
    {
        public LeaveRequestRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<LeaveRequest>> GetApprovedLeaveRequestsByDateRangeAsync(DateTime startDate, DateTime endDate, int? employeeId)
        {
            var query = _context.LeaveRequests
                .AsNoTracking()
                .Include(q => q.Employee)
                .ThenInclude(e => e.Department)
                .Include(q => q.LeaveType)
                // Sadece onaylı, silinmemiş ve tarih aralığına uygun olanları DB'de filtrele
                .Where(q => !q.IsDeleted && q.Status == Domain.Enums.LeaveRequestStatus.Approved)
                .Where(q => q.StartDate.Date >= startDate.Date && q.EndDate.Date <= endDate.Date);

            // Eğer personel seçildiyse onu da filtrele
            if (employeeId.HasValue && employeeId.Value > 0)
            {
                query = query.Where(q => q.EmployeeId == employeeId.Value);
            }

            return await query.ToListAsync(); // Veritabanı sorgusu burada çalışır
        }

        // DÜZELTME: Metot ismine "Id" eklendi (Interface ile aynı oldu).
        public async Task<List<LeaveRequest>> GetLeaveRequestsByEmployeeIdAsync(int employeeId)
        {
            return await _context.LeaveRequests
                .AsNoTracking()
                .Include(q => q.Employee)
                .Include(q => q.LeaveType)
                .Where(q => q.EmployeeId == employeeId)
                .ToListAsync();
        }

        public async Task<List<LeaveRequest>> GetLeaveRequestsWithDetailsAsync()
        {
            return await _context.LeaveRequests
                .AsNoTracking()
                .Include(q => q.Employee)
                .ThenInclude(x => x.Department)
                .Include(q => q.LeaveType)
                .ToListAsync();
        }

       
        public async Task<LeaveRequest> GetLeaveRequestWithDetailsByIdAsync(int id)
        {
            return await _context.LeaveRequests
                .Include(q => q.Employee)
                .Include(q => q.LeaveType)
                .FirstOrDefaultAsync(q => q.Id == id);
        }
    }
}