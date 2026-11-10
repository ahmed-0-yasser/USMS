using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USMS.Core.DTOs.Notifications;

namespace USMS.Core.Interfaces
{
    /// <summary>
    /// Defines operations for retrieving and managing user notifications.
    /// </summary>
    public interface INotificationService
    {
        Task<List<NotificationResponseDto>> GetMyNotificationsAsync(
           string userId);

        Task<bool> MarkAsReadAsync(
            int notificationId,
            string userId);
    }
}
