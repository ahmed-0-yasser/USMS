
namespace USMS.Core.Entities
{
    /// <summary>
    /// Represents a service request submitted by a student.
    /// </summary>
    public class ServiceRequest
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
        /// Gets or sets the current status of the request. Defaults to Submitted.
        /// </summary>
        public string Status { get; set; } = "Submitted";

        /// <summary>
        /// Gets or sets the reason for rejecting the request, if applicable.
        /// </summary>
        public string? RejectionReason { get; set; }

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
        /// Gets or sets the date and time when the request was created. Defaults to the current UTC time.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the date and time when the request was last updated, if applicable.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        // Foreign Keys

        /// <summary>
        /// Gets or sets the ID of the student who submitted the request.
        /// </summary>
        public int StudentId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the service associated with the request.
        /// </summary>
        public int ServiceId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the employee assigned to the request, if applicable.
        /// </summary>
        public int? AssignedEmployeeId { get; set; }

        // Navigation Properties

        /// <summary>
        /// Gets or sets the student who submitted the request.
        /// </summary>
        public Student Student { get; set; } = null!;

        /// <summary>
        /// Gets or sets the service associated with the request.
        /// </summary>
        public Service Service { get; set; } = null!;

        /// <summary>
        /// Gets or sets the employee assigned to the request, if applicable.
        /// </summary>
        public Employee? AssignedEmployee { get; set; }
    }
}
