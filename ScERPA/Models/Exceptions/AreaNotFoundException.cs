namespace ScERPA.Models.Exceptions
{
    public class AreaNotFoundException : Exception
    {
        public AreaNotFoundException()            
        {
        }

        public AreaNotFoundException(string message)
            : base(message)
        {
        }

        public AreaNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
