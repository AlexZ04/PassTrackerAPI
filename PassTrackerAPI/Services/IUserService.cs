﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
using PassTrackerAPI.DTO;

namespace PassTrackerAPI.Services
{
    public interface IUserService
    {
        public Task<TokenResponseDTO> RegisterUser(UserRegisterDTO user, ModelStateDictionary modelState);
        public Task<TokenResponseDTO> LoginUser(UserLoginDTO user);
    }
}
