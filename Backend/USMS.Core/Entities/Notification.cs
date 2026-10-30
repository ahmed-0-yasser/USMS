using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace USMS.Core.Entities;
    public class Notification
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string Type { get; set; } = "General";

        public string UserId { get; set; } = string.Empty;

        public ApplicationUser User { get; set; } = null!;
    }

