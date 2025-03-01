using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PassTrackerAPI.Constants;
using PassTrackerAPI.Data;

namespace PassTrackerAPI.Filters
{
    public class CheckTokenLife : Attribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var token = await context.HttpContext.GetTokenAsync("access_token");

            var _context = context.HttpContext.RequestServices.GetService(typeof(DataContext)) as DataContext;

            if (_context == null)
            {
                SetError(context);
                return;
            }

            var blackToken = _context.BlacklistTokens.FirstOrDefault(t => t.Token == token);

            if (blackToken != null)
            {
                SetError(context);
            }
        }

        private void SetError(AuthorizationFilterContext context)
        {
            context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
            context.Result = new UnauthorizedObjectResult(new
            {
                Error = StatusCodes.Status401Unauthorized,
                Message = ErrorTitles.UNAUTHORIZED
            });
        }
    }
}
