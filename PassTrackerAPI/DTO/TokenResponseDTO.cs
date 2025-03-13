namespace PassTrackerAPI.DTO
{
    public class TokenResponseDTO
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime AccessTokenExpireTime { get; set; }
    }
}
