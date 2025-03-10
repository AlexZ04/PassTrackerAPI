using PassTrackerAPI.Data.Entities;

namespace PassTrackerAPI.DTO
{
    public class RequestExcelDTO
    {

        public string UserName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public TypeRequestDB TypeRequest { get; set; }
        public StatusRequestDB StatusRequest { get; set; }
        public string? Comment { get; set; }
        public byte[]? Photo { get; set; }
        public int? Group { get; set; }
    }
}
