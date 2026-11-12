
namespace USMS.Core.DTOs.ServiceRequests
{
    /// <summary>
    /// Represents the summary information returned for a student service request.
    /// </summary>
    public class ServiceRequestResponseDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the service request.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the type of the service request.
        /// </summary>
        public string RequestType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the requested service.
        /// </summary>
        public string ServiceName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the current status of the service request.
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date and time when the request was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the request was last updated, if applicable.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}
