using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using USMS.Core.Interfaces;

namespace USMS.API.Controllers
{
    /// <summary>
    /// Handles user notifications and provides endpoints to retrieve and update notification status.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _service;

        public NotificationsController(INotificationService service)
        {
            _service = service;
        }

        [HttpGet("my-notifications")]
        public async Task<IActionResult> GetMyNotifications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            var notifications = await _service.GetMyNotificationsAsync(userId);

            return Ok(notifications);
        }



        [HttpPut("{id:int}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            var result = await _service.MarkAsReadAsync(id, userId);

            if (!result)
                return NotFound(new
                {
                    message = "Notification not found."
                });

            return Ok(new
            {
                message = "Notification marked as read successfully."
            });
        }

    }
}
