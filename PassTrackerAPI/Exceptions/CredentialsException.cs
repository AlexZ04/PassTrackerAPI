namespace PassTrackerAPI.Exceptions
{
    public class CredentialsException : CustomException
    {
        public CredentialsException(string error, string message) : 
            base(StatusCodes.Status400BadRequest, error, message)
        {
        }
    }
}
