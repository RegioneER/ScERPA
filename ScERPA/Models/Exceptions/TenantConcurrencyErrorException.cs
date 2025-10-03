namespace ScERPA.Models.Exceptions
{
    public class TenantConcurrencyErrorException : Exception
    {
        public TenantConcurrencyErrorException()            
        {
        }

        public TenantConcurrencyErrorException(string message)
            : base(message)
        {
        }

        public TenantConcurrencyErrorException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
