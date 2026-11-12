
using System.ComponentModel.DataAnnotations;

namespace USMS.Core.DTOs.ServiceRequests
{
    /// <summary>
    /// Represents the data required to create a new student service request.
    /// </summary>
    public class CreateServiceRequestDto
    {
        /// <summary>
        /// Gets or sets the type of the service request.
        /// </summary>
        [Required]
        public string RequestType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the ID of the service associated with the request.
        /// </summary>
        [Range(1, int.MaxValue)]
        public int ServiceId { get; set; }

        /// <summary>
        /// Gets or sets the type of certificate requested, if applicable.
        /// </summary>
        [MaxLength(100)]
        public string? CertificateType { get; set; }

        /// <summary>
        /// Gets or sets the purpose of the service request.
        /// </summary>
        [Required]
        [MaxLength(250)]
        public string Purpose { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the academic year related to the request, if applicable.
        /// </summary>
        [MaxLength(20)]
        public string? AcademicYear { get; set; }

        /// <summary>
        /// Gets or sets the semester related to the request, if applicable.
        /// </summary>
        [MaxLength(50)]
        public string? Semester { get; set; }

        /// <summary>
        /// Gets or sets additional notes about the service request.
        /// </summary>
        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}
