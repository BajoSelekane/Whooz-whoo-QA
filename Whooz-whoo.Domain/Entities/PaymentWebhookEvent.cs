using System;
using System.Collections.Generic;
using System.Text;

namespace Whooz_whoo.Domain.Entities
{
    public class PaymentWebhookEvent: BaseEntity
    {
        public string Type { get; set; } = string.Empty;
        public PaymentWebhookData? Data { get; set; }
    }
}
