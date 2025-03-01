using Microsoft.EntityFrameworkCore;
using PassTrackerAPI.Constants;
using PassTrackerAPI.Data;
using PassTrackerAPI.Data.Entities;
using PassTrackerAPI.DTO;
using PassTrackerAPI.Exceptions;
using System.Security.Claims;
using System.Xml.Linq;


namespace PassTrackerAPI.Services.ServisesImplementations
{
    public class UserServiceImpl : IUserService
    {
        private readonly DataContext _context;
        private readonly IHasherService _hasherService;
        private readonly ITokenService _tokenService;

        public UserServiceImpl(DataContext context, IHasherService hasherService, ITokenService tokenService)
        {
            _context = context;
            _hasherService = hasherService;
            _tokenService = tokenService;
        }

        public async Task<TokenResponseDTO> RegisterUser(UserRegisterDTO user)
        {
            await CheckEmailIfUsed(user.Email);

            UserDb newUser = new UserDb
            {
                Id = Guid.NewGuid(),
                FirstName = user.FirstName,
                SecondName = user.SecondName,
                MiddleName = user.MiddleName,
                Group = user.Group,
                Email = user.Email,
                Password = _hasherService.HashPassword(user.Password),
                CreateTime = DateTime.Now.ToUniversalTime(),
                Roles = new List<UserRoleDb>()
            };

            UserRoleDb newRole = new UserRoleDb
            {
                User = newUser,
                Role = RoleDb.New
            };

            newUser.Roles.Add(newRole);

            _context.UserRoles.Add(newRole);
            _context.Users.Add(newUser);

            await _context.SaveChangesAsync();

            string token = _tokenService.CreateAccessTokenById(newUser.Id);

            return new TokenResponseDTO(token);
        }

        public async Task<TokenResponseDTO> LoginUser(UserLoginDTO user)
        {
            var foundUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);

            if (foundUser == null || !_hasherService.CheckPassword(foundUser.Password, user.Password))
                throw new CredentialsException(ErrorTitles.CREDENTIALS, ErrorMessages.INVALID_CREDENTIALS);

            string token = _tokenService.CreateAccessTokenById(foundUser.Id);

            return new TokenResponseDTO(token);
        }

        public async Task Logout(string? token)
        {
            await CheckToken(token);

            var blacklistToken = new BlacklistTokenDb
            {
                Token = token,
                AddTime = DateTime.Now.ToUniversalTime(),
            };

            _context.BlacklistTokens.Add(blacklistToken);
            await _context.SaveChangesAsync();
        }

        public async Task<UserProfileDTO> GetUserProfileById(Guid id)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                throw new KeyNotFoundException();

            var userInfo = new UserProfileDTO
            {
                Name = ConcatName(user.SecondName, user.FirstName, user.MiddleName),
                Group = user.Group,
                Roles = new List<RoleDb>()
            };

            foreach (var elem in user.Roles)
                userInfo.Roles.Add(elem.Role);

            return userInfo;
        }

        public Task<UserProfileDTO> GetProfile(ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                throw new UnauthorizedAccessException();

            return GetUserProfileById(new Guid(userId));
        }

        // get all not-new (confirmed) users
        public async Task<List<UserShortDTO>> GetAllUsers()
        {
            var allUsers = await _context.Users
                .Include(u => u.Roles)
                .Where(u => !u.Roles.Select(u => u.Role).Contains(RoleDb.New))
                .ToListAsync();

            List<UserShortDTO> res = new List<UserShortDTO>();

            foreach (var user in allUsers)
            {
                res.Add(new UserShortDTO
                {
                    Id = user.Id,
                    Name = ConcatName(user.SecondName, user.FirstName, user.MiddleName),
                    Group = user.Group
                });
            }

            return res;
        }

        private async Task CheckEmailIfUsed(string email)
        {
            var foundUserByEmail = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (foundUserByEmail != null)
                throw new CredentialsException(ErrorTitles.CREDENTIALS, ErrorMessages.EMAIL_IS_ALREADY_USED);
        }

        private async Task CheckToken(string? token)
        {
            if (token == null || await _context.BlacklistTokens.FindAsync(token) != null)
                throw new UnauthorizedAccessException();
        }

        private string ConcatName(string secondName, string firstName, string thirdName)
        {
            return secondName + " " + firstName + " " + thirdName; 
        }
    }
}
