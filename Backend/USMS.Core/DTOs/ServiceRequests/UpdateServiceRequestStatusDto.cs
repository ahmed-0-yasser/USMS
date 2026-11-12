
using System.ComponentModel.DataAnnotations;

namespace USMS.Core.DTOs.ServiceRequests
{
    /// <summary>
    /// Represents the data required to update the status of a service request.
    /// </summary>
    public class UpdateServiceRequestStatusDto
    {
        /// <summary>
        /// Gets or sets the new status of the service request.
        /// </summary>
        [Required]
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the reason for rejecting the request, if applicable.
        /// </summary>
        [MaxLength(500)]
        public string? RejectionReason { get; set; }
    }
}
