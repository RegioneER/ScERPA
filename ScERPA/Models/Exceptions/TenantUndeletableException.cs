namespace ScERPA.Models.Exceptions
{
    public class TenantUndeletableException : Exception
    {
        public TenantUndeletableException()            
        {
        }

        public TenantUndeletableException(string message)
            : base(message)
        {
        }

        public TenantUndeletableException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
