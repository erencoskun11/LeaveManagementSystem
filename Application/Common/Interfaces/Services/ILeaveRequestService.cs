using Application.DTOs.LeaveRequest;
using Domain.Entities; // Eğer Entity kullanıyorsan
using System.Collections.Generic;
using System.Threading.Tasks;

// HATA BURADAYDI: Muhtemelen sende "Application.Services" yazıyor.
// AŞAĞIDAKİ GİBİ OLMALI:
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