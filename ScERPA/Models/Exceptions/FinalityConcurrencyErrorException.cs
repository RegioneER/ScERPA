namespace ScERPA.Models.Exceptions
{
    public class FinalityConcurrencyErrorException : Exception
    {
        public FinalityConcurrencyErrorException()            
        {
        }

        public FinalityConcurrencyErrorException(string message)
            : base(message)
        {
        }

        public FinalityConcurrencyErrorException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
