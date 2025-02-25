using PassTrackerAPI.DTO;

namespace PassTrackerAPI.Services
{
    public interface IUserService
    {
        public Task<TokenResponseDTO> RegisterUser();
        public Task<TokenResponseDTO> LoginUser();
    }
}
