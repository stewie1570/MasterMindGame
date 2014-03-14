using System;

namespace MasterMind.Web.Exceptions
{
    public class InvalidRequestException : Exception
    {
        public InvalidRequestException(string message) : base(message) { }
    }
}