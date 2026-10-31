
using System.ComponentModel.DataAnnotations;

namespace USMS.Core.DTOs.ServiceRequests
{
    public class UpdateServiceRequestStatusDto
    {
        [Required]
        public string Status { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? RejectionReason { get; set; }
    }
}
