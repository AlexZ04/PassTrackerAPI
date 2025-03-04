using PassTrackerAPI.Data.Entities;

namespace PassTrackerAPI.DTO
{
    public class RequestShortDTO
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public TypeRequestDB TypeRequest { get; set; }
        public StatusRequestDB StatusRequest { get; set; }
    }
}
