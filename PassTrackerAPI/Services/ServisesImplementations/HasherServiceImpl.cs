using System.Web.Helpers;

namespace PassTrackerAPI.Services.ServisesImplementations
{
    public class HasherServiceImpl : IHasherService
    {
        public string HashPassword(string password)
        {
            return Crypto.HashPassword(password);
        }

        public bool CheckPassword(string hashedPassword, string enteredPassword)
        {
            return Crypto.VerifyHashedPassword(hashedPassword, enteredPassword);
        }
    }
}
