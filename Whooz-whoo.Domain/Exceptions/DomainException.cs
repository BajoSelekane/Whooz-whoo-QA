using System;
using System.Collections.Generic;
using System.Text;

namespace Whooz_whoo.Domain.Exceptions
{
    public abstract class DomainException : Exception
    {
        protected DomainException(string message) : base(message) { }
        protected DomainException(string message, Exception innerException) : base(message, innerException) { }
    }
}
