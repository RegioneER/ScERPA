namespace ScERPA.Models.Exceptions
{
    public class FinalityReferenceErrorException : Exception
    {
        public FinalityReferenceErrorException()            
        {
        }

        public FinalityReferenceErrorException(string message)
            : base(message)
        {
        }

        public FinalityReferenceErrorException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
