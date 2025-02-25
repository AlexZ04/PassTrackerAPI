namespace PassTrackerAPI.Services
{
    public interface ITokenService
    {
        public string CreateAccessTokenById(Guid id);
    }
}
