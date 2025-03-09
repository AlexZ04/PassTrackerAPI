namespace PassTrackerAPI.Services
{
    public interface ITokenService
    {
        public string CreateAccessTokenById(Guid id, List<string> roles);
        public string GenerateRefreshToken();
        public Task HandleTokens(Guid userId, Guid tokenId);
    }
}
