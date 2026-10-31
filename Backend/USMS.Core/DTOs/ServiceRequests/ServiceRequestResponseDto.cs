
namespace USMS.Core.DTOs.ServiceRequests
{
    public class ServiceRequestResponseDto
    {
        public int Id { get; set; }

        public string RequestType { get; set; } = string.Empty;

        public string ServiceName { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
