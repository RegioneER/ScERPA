namespace ScERPA.Models.Exceptions
{
    public class UserFinalityNotFoundException : Exception
    {
        public UserFinalityNotFoundException()            
        {
        }

        public UserFinalityNotFoundException(string message)
            : base(message)
        {
        }

        public UserFinalityNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
