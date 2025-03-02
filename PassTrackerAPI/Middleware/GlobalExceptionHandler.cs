using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PassTrackerAPI.Constants;
using PassTrackerAPI.Exceptions;

namespace PassTrackerAPI.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Exception occured: {Message}", exception.Message);

            ProblemDetails problemDetails = new ProblemDetails { Status = 400 };

            if (exception is CustomException custExc)
                problemDetails = BuildDetails(custExc.Code, custExc.Error, exception.Message);

            else if (exception is KeyNotFoundException)
                problemDetails = BuildDetails(StatusCodes.Status404NotFound, ErrorTitles.KEY_NOT_FOUND);

            else if (exception is UnauthorizedAccessException)
                problemDetails = BuildDetails(StatusCodes.Status401Unauthorized, ErrorTitles.UNAUTHORIZED);

            else
                problemDetails = BuildDetails(StatusCodes.Status500InternalServerError, ErrorTitles.SERVER_ERROR);

            httpContext.Response.StatusCode = problemDetails.Status.Value;

            await httpContext.Response
                .WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }

        private ProblemDetails BuildDetails(int code, string title, string? detail = null)
        {
            var problemDetails = new ProblemDetails
            {
                Status = code,
                Title = title
            };

            if (detail != null)
                problemDetails.Detail = detail;

            return problemDetails;
        }
    }
}
