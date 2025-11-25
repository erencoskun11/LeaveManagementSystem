using Application.DTOs.LeaveRequest;
using Domain.Entities; 
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Application.Common.Interfaces.Services
{
    public interface ILeaveRequestService
    {
        Task<int> CreateLeaveRequestAsync(CreateLeaveRequestDto request, string userEmail);
        Task<List<LeaveRequestDetailDto>> GetMyLeaveRequestsAsync(string userEmail);
        Task<List<LeaveRequestDetailDto>> GetAllLeaveRequestsAsync();
        Task<bool> ManageLeaveRequestAsync(int requestId, bool approved, string managerEmail);
    }
}