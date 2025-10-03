namespace ScERPA.Models.Exceptions
{
    public class EServiceNotFoundException : Exception
    {
        public EServiceNotFoundException()            
        {
        }

        public EServiceNotFoundException(string message)
            : base(message)
        {
        }

        public EServiceNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
