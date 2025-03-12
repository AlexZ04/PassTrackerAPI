namespace PassTrackerAPI.Exceptions
{
    public class NotFoundException : CustomException
    {
        public NotFoundException(string error, string message) :
            base(StatusCodes.Status404NotFound, error, message)
        {
        }
    }
}
