
using System.ComponentModel.DataAnnotations;

namespace USMS.Core.DTOs.ServiceRequests
{
    public class CreateServiceRequestDto
    {
        [Required]
        public string RequestType { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int ServiceId { get; set; }

        [MaxLength(100)]
        public string? CertificateType { get; set; }

        [Required]
        [MaxLength(250)]
        public string Purpose { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? AcademicYear { get; set; }

        [MaxLength(50)]
        public string? Semester { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}
