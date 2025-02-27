using PassTrackerAPI.DTO;

namespace PassTrackerAPI.Services
{
    public interface IUserService
    {
        public Task<TokenResponseDTO> RegisterUser(UserRegisterDTO user);
        public Task<TokenResponseDTO> LoginUser(UserLoginDTO user);
        public Task<UserProfileDTO> GetUserProfileById(Guid id);
    }
}
