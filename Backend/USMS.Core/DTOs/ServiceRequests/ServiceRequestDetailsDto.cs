
namespace USMS.Core.DTOs.ServiceRequests
{
    /// <summary>
    /// Represents the detailed information of a student service request.
    /// </summary>
    public class ServiceRequestDetailsDto
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
        /// Gets or sets the requested certificate type, if applicable.
        /// </summary>
        public string? CertificateType { get; set; }

        /// <summary>
        /// Gets or sets the purpose of the service request.
        /// </summary>
        public string Purpose { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the academic year related to the request, if applicable.
        /// </summary>
        public string? AcademicYear { get; set; }

        /// <summary>
        /// Gets or sets the semester related to the request, if applicable.
        /// </summary>
        public string? Semester { get; set; }

        /// <summary>
        /// Gets or sets additional notes about the service request.
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// Gets or sets the reason provided when the request is rejected, if applicable.
        /// </summary>
        public string? RejectionReason { get; set; }

        /// <summary>
        /// Gets or sets the name of the employee assigned to the request, if applicable.
        /// </summary>
        public string? AssignedEmployeeName { get; set; }

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
