
using System.ComponentModel.DataAnnotations;


namespace USMS.Core.DTOs.Complaints
{
    public record CreateComplaintRequestDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Category { get; set; } = string.Empty;
    }

}
