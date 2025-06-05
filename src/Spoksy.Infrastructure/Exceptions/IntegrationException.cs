namespace Spoksy.Domain.Exceptions
{
    public class IntegrationException : Exception
    {
        public IntegrationException(string message) : base(message) { }

        public IntegrationException(string message, Exception innerException) : base(message, innerException) { }

        public static void When(bool hasError, string message)
        {
            if (hasError)
                throw new IntegrationException(message);
        }
    }
}