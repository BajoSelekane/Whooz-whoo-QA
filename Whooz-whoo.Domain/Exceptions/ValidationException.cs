using System;
using System.Collections.Generic;
using System.Text;

namespace Whooz_whoo.Domain.Exceptions
{
    public class ValidationException : DomainException
    {
        public List<string> Errors { get; }

        public ValidationException(string message, List<string> errors = null) : base(message)
        {
            Errors = errors ?? [];
        }
    }
}
