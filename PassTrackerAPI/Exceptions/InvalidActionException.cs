namespace PassTrackerAPI.Exceptions
{
    public class InvalidActionException : CustomException
    {
        public InvalidActionException(string error, string message) :
            base(StatusCodes.Status400BadRequest, error, message)
        {
        }
    }
}
