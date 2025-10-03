namespace ScERPA.Models.Exceptions
{
    public class ConfigurationUndeletableException : Exception
    {
        public ConfigurationUndeletableException()            
        {
        }

        public ConfigurationUndeletableException(string message)
            : base(message)
        {
        }

        public ConfigurationUndeletableException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
