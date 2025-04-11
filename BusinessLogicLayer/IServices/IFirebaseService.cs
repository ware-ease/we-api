using BusinessLogicLayer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.IServices
{
    public interface IFirebaseService
    {
        Task SendNotificationToUsersAsync(List<string> userIds, string title, string message, NotificationType type, string? warehouseId);
    }
}
