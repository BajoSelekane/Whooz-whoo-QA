using System;
using System.Collections.Generic;
using System.Text;

namespace Whooz_whoo.Domain.Enums
{
    public enum PaymentMethodType
    {
        Card = 0,
        BankTransfer = 1,
        MobileMoney = 2,
        Crypto = 3,
        PayPal = 4,
        ApplePay = 5,
        GooglePay = 6,
        Coupon = 7,
        Other = 99
    }

}
