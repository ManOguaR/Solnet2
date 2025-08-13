using System.Runtime.Serialization;

namespace Solnet.Core.Exceptions
{
    [Serializable]
    public class SolnetCoreException :
            Exception
    {
        public SolnetCoreException()
        {
        }

        public SolnetCoreException(string? message)
            : base(message)
        {
        }

        public SolnetCoreException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }

#if NET8_0_OR_GREATER
        [Obsolete("Formatter-based serialization is obsolete and should not be used.")]
#endif
        protected SolnetCoreException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}