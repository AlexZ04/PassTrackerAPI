namespace PassTrackerAPI.Data.Entities
{
    public class RequestDB
    {
        public Guid Id { get; set; }
        public UserDb User { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public TypeRequestDB TypeRequest { get; set; }
        public StatusRequestDB StatusRequest { get; set; }
        public bool InDeanery { get; set; }
        public string? Comment { get; set; }
        public byte[]? Photo { get; set; }

    }
}
