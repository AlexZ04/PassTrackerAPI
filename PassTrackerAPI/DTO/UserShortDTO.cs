using PassTrackerAPI.Data.Entities;

namespace PassTrackerAPI.DTO
{
    public class UserShortDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int? Group { get; set; }
    }
}
