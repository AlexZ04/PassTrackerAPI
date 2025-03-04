using PassTrackerAPI.Data.Entities;

namespace PassTrackerAPI.DTO
{
    public class UserProfileDTO
    {
        public string Name { get; set; }
        public int? Group { get; set; }
        public string Email { get; set; }
        public List<RoleDb> Roles { get; set; } 
    }
}
