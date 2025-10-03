namespace ScERPA.Models.Exceptions
{
    public class FinalityUndeletableException : Exception
    {
        public FinalityUndeletableException()            
        {
        }

        public FinalityUndeletableException(string message)
            : base(message)
        {
        }

        public FinalityUndeletableException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
