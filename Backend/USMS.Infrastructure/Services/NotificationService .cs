using Microsoft.EntityFrameworkCore;
using USMS.Core.DTOs.Notifications;
using USMS.Core.Interfaces;
using USMS.Infrastructure.Persistence;

namespace USMS.Infrastructure.Services
{
    public class NotificationService: INotificationService
    {
        private readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<NotificationResponseDto>> GetMyNotificationsAsync(string userId)
        {
            var notifications = await _context.Notifications
                 .Where(n => n.UserId == userId)
                 .OrderByDescending(n => n.CreatedAt)
                 .Select(n => new NotificationResponseDto
                 {
                     Id = n.Id,
                     Title = n.Title,
                     Message = n.Message,
                     Type = n.Type,
                     IsRead = n.IsRead,
                     CreatedAt = n.CreatedAt
                 })
                 .ToListAsync();

            return notifications;
        }

        public async Task<bool> MarkAsReadAsync(int notificationId, string userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n =>
                    n.Id == notificationId &&
                    n.UserId == userId);

            if (notification == null)
                return false;

            notification.IsRead = true;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
