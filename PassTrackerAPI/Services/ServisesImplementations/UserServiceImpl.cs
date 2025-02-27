﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using PassTrackerAPI.Constants;
using PassTrackerAPI.Data;
using PassTrackerAPI.Data.Entities;
using PassTrackerAPI.DTO;
using PassTrackerAPI.Exceptions;


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

        public async Task CheckEmail(string email)
        {
            var foundUserByEmail = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (foundUserByEmail != null)
                throw new CredentialsException(ErrorTitles.CREDENTIALS_EXCEPTION, ErrorMessages.EMAIL_IS_ALREADY_USED);
        }

        public async Task<TokenResponseDTO> RegisterUser(UserRegisterDTO user)
        {
            await CheckEmail(user.Email);

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
                throw new CredentialsException(ErrorTitles.CREDENTIALS_EXCEPTION, ErrorMessages.INVALID_CREDENTIALS);

            string token = _tokenService.CreateAccessTokenById(foundUser.Id);

            return new TokenResponseDTO(token);
        }

    }
}
