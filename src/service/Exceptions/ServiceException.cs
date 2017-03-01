using System;

namespace Toucan.Service
{
    public class ServiceException : Exception
    {
        private readonly string title;
        public ServiceException(string message, string title = null) : base(message)
        {
            this.title = title;
        }

        public ServiceException(string message, Exception innerException, string title = null) : base(message, innerException)
        {
            this.title = title;
        }

        public string Title { get; }
    }
}
