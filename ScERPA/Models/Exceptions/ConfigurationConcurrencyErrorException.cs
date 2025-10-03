namespace ScERPA.Models.Exceptions
{
    public class ConfigurationConcurrencyErrorException : Exception
    {
        public ConfigurationConcurrencyErrorException()            
        {
        }

        public ConfigurationConcurrencyErrorException(string message)
            : base(message)
        {
        }

        public ConfigurationConcurrencyErrorException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
