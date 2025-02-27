namespace PassTrackerAPI.Data.Entities
{
    public class UserRoleDb
    {
        public Guid Id { get; set; }
        public UserDb User { get; set; }
        public RoleDb Role { get; set; }
    }
}
