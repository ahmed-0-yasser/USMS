namespace USMS.Entities
{
    public class ServiceInformation
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }

        public string Description { get; set; } = string.Empty;
        public string EligibilityConditions { get; set; } = string.Empty;
        public string RequiredDocuments { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
        public string ExpectedProcessingDuration { get; set; }= string.Empty;

        public Service Service { get; set; } = null!;
    }
}
