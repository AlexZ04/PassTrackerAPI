using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using PassTrackerAPI.Constants;

namespace PassTrackerAPI.Utitlities
{
    public class ErrorHelper
    {
        public static void SetError(AuthorizationFilterContext context)
        {
            context.Result = new ObjectResult(new
            {
                Message = ErrorMessages.ONLY_FOR_ADMINS
            })
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
        }
    }
}
