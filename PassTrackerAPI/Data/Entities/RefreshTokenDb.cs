namespace PassTrackerAPI.Data.Entities
{
    public class RefreshTokenDb
    {
        public Guid Id { get; set; }
        public string Token { get; set; }
        public UserDb User { get; set; }
        public DateTime Expires { get; set; }
    }
}
