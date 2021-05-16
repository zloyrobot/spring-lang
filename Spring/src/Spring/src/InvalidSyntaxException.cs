using System;
using System.Runtime.Serialization;

namespace JetBrains.ReSharper.Plugins.Spring
{
    [Serializable]
    public class InvalidSyntaxException : Exception
    {
        public InvalidSyntaxException()
        {
        }

        public InvalidSyntaxException(string message) : base(message)
        {
        }

        public InvalidSyntaxException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidSyntaxException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

}
