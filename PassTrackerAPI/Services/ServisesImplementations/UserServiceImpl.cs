using Microsoft.EntityFrameworkCore;
using PassTrackerAPI.Constants;
using PassTrackerAPI.Data;
using PassTrackerAPI.Data.Entities;
using PassTrackerAPI.DTO;
using PassTrackerAPI.Exceptions;
using PassTrackerAPI.Repositories;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Xml.Linq;


namespace PassTrackerAPI.Services.ServisesImplementations
{
    public class UserServiceImpl : IUserService
    {
        private readonly DataContext _context;
        private readonly IHasherService _hasherService;
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository;

        public UserServiceImpl(
            DataContext context,
            IHasherService hasherService,
            ITokenService tokenService,
            IUserRepository userRepository)
        {
            _context = context;
            _hasherService = hasherService;
            _tokenService = tokenService;
            _userRepository = userRepository;
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
            var user = await _userRepository.GetUserById(id);

            var userInfo = new UserProfileDTO
            {
                Name = ConcatName(user.SecondName, user.FirstName, user.MiddleName),
                Group = user.Group,
                Email = user.Email,
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

        // if newUsersOnly == false then get all not-new users
        public async Task<List<UserShortDTO>> GetAllUsers(bool newUsersOnly = false)
        {
            var allUsersQuerable = _context.Users
                .Include(u => u.Roles);

            List<UserDb> allUsers;
            
            if (newUsersOnly)
                allUsers = await allUsersQuerable
                    .Where(u => u.Roles.Select(u => u.Role).Contains(RoleDb.New))
                    .ToListAsync();
            else
                allUsers = await allUsersQuerable
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

        public async Task<RoleResponseDTO> GetUserHighestRole(Guid id)
        {
            var user = await _userRepository.GetUserById(id);

            var userRoles = await _context.UserRoles
                .Where(u => u.User.Equals(user))
                .Select(u => u.Role)
                .OrderByDescending(u => (int) u)
                .ToListAsync();

            if (userRoles.Count() == 0) userRoles.Add(RoleDb.New);

            return new RoleResponseDTO
            {
                Role = userRoles[0]
            };
        }

        public async Task EditUserEmail(Guid id, UserEditEmailDTO email)
        {
            var user = await _userRepository.GetUserById(id);

            user.Email = email.Email;

            await _context.SaveChangesAsync();
        }

        public async Task EditUserPassword(Guid id, UserEditPasswordDTO password)
        {
            var user = await _userRepository.GetUserById(id);

            user.Password = _hasherService.HashPassword(password.Password);

            await _context.SaveChangesAsync();
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
