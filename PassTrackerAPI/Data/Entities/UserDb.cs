namespace PassTrackerAPI.Data.Entities
{
    public class UserDb
    {
        public Guid Id { get; set; }
        public string SecondName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public int? Group { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime CreateTime { get; set; }
        public List<UserRoleDb> Roles { get; set; }
    }
}
