using Application.Common.Interfaces.Services;
using Infrastructure.Hubs; 
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendNotification(string message)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", message);
        }

        public async Task SendLeaveUpdate()
        {
            await _hubContext.Clients.All.SendAsync("ReceiveLeaveUpdate");
        }

        public async Task SendEmployeeUpdate()
        {
            await _hubContext.Clients.All.SendAsync("ReceiveEmployeeUpdate");
        }

        public async Task SendDepartmentUpdate()
        {
            await _hubContext.Clients.All.SendAsync("ReceiveDepartmentUpdate");
        }
    }
}