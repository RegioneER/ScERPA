namespace ScERPA.Models.Exceptions
{
    public class UserPermissionNotValidException : Exception
    {
        public UserPermissionNotValidException()            
        {
        }

        public UserPermissionNotValidException(string message)
            : base(message)
        {
        }

        public UserPermissionNotValidException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
