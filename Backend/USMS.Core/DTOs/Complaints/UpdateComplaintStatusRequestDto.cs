using System.ComponentModel.DataAnnotations;

namespace USMS.Core.DTOs.Complaints
{
    public record UpdateComplaintStatusRequestDto
    {
        [Required]
        public string Status { get; set; } = string.Empty;
    }


}
