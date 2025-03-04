using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Filters;
using PassTrackerAPI.Data;
using PassTrackerAPI.Services;
using PassTrackerAPI.Utitlities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PassTrackerAPI.Filters
{
    public class Admin : Attribute, IAsyncAuthorizationFilter
    {
        private readonly JwtSecurityTokenHandler _tokenHandler = new JwtSecurityTokenHandler();

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var token = await context.HttpContext.GetTokenAsync("access_token");

            var _context = context.HttpContext.RequestServices.GetService(typeof(DataContext)) as DataContext;

            if (_context == null)
            {
                ErrorHelper.SetError(context);
                return;
            }

            var jwtToken = _tokenHandler.ReadJwtToken(token);

            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                ErrorHelper.SetError(context);
                return;
            }

            var userGuidId = new Guid(userId);

            var isUserAdmin = await _context.Admins.FindAsync(userGuidId);

            if (isUserAdmin == null) ErrorHelper.SetError(context);
        }
    }
}
