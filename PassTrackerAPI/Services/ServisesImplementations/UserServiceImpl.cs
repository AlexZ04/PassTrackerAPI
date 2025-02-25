using Microsoft.AspNetCore.Http.HttpResults;
using PassTrackerAPI.DTO;

namespace PassTrackerAPI.Services.ServisesImplementations
{
    public class UserServiceImpl : IUserService
    {
        public async Task<TokenResponseDTO> RegisterUser()
        {
            return new TokenResponseDTO("1");
        }

        public async Task<TokenResponseDTO> LoginUser()
        {
            // todo
            return new TokenResponseDTO("1");
        }
    }
}
