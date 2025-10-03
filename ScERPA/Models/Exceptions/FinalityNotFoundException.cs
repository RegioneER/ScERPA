namespace ScERPA.Models.Exceptions
{
    public class FinalityNotFoundException : Exception
    {
        public FinalityNotFoundException()            
        {
        }

        public FinalityNotFoundException(string message)
            : base(message)
        {
        }

        public FinalityNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
