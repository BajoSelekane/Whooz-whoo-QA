using System;
using System.Collections.Generic;
using System.Text;

namespace Whooz_whoo.Domain.Exceptions
{
    public class BusinessException : DomainException
    {
        public BusinessException(string message) : base(message) { }
        public BusinessException(string message, Exception innerException) : base(message, innerException) { }
    }
}
