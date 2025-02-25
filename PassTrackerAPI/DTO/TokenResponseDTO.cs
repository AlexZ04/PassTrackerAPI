namespace PassTrackerAPI.DTO
{
    public class TokenResponseDTO
    {
        public string Token { get; set; }

        public TokenResponseDTO(string token)
        {
            Token = token;
        }
    }
}
