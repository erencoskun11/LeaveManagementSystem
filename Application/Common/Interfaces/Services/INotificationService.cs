using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces.Services
{
    public interface INotificationService
    {//bu listeleri yeniliyoruz
        Task SendNotification(string message);
        Task SendEmployeeUpdate();
        Task SendDepartmentUpdate();
        Task SendLeaveUpdate();
    }
}
