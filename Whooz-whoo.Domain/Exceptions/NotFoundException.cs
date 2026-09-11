using System;
using System.Collections.Generic;
using System.Text;

namespace Whooz_whoo.Domain.Exceptions
{
    public class NotFoundException : DomainException
    {
        public string EntityName { get; }
        public object EntityId { get; }

        public NotFoundException(string entityName, object entityId)
            : base($"Entity {entityName} with id {entityId} was not found")
        {
            EntityName = entityName;
            EntityId = entityId;
        }
    }
}
