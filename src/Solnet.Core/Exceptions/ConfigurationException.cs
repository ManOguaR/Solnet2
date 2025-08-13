using System.Runtime.Serialization;

namespace Solnet.Core.Exceptions
{
    [Serializable]
    public class ConfigurationException :
        SolnetCoreException
    {
        public ConfigurationException()
        {
        }

        public ConfigurationException(IEnumerable<ValidationResult> results, string message)
            : base(message)
        {
            Results = results;
        }

        public ConfigurationException(IEnumerable<ValidationResult> results, string message, Exception innerException)
            : base(message, innerException)
        {
            Results = results;
        }

        public ConfigurationException(string message)
            : base(message)
        {
        }

        public ConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

#if NET8_0_OR_GREATER
        [Obsolete("Formatter-based serialization is obsolete and should not be used.")]
#endif
        protected ConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public IEnumerable<ValidationResult> Results { get; protected set; } = [];
    }
}
