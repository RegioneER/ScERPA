namespace ScERPA.Models.Exceptions
{
    public class ThresholdException : Exception
    {

            public ThresholdException()
            {
            }

            public ThresholdException(string message)
                : base(message)
            {
            }

            public ThresholdException(string message, Exception inner)
                : base(message, inner)
            {
            }
    }    
}
