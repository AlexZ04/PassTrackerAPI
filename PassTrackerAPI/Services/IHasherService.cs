namespace PassTrackerAPI.Services
{
    public interface IHasherService
    {
        public string HashPassword(string password);
        public bool CheckPassword(string hashedPassword, string enteredPassword);
    }
}
