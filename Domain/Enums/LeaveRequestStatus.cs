using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum LeaveRequestStatus
    {
        Pending = 1, // Bekliyor
        Approved = 2, // Onaylandı
        Rejected = 3  // Reddedildi
    }
}
