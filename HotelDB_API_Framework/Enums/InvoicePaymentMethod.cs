using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelDB_API_Framework.Enums
{
    /// <summary>
    /// Representa os meios de pagamento disponíveis
    /// </summary>
    public enum InvoicePaymentMethod
    {
        /// <summary>O meio de pagamento escolhido é o pagamento em dinheiro.</summary>
        Cash = 0,

        /// <summary>O meio de pagamento escolhido é o pagamento com cartão de crédito.</summary>
        CreditCard = 1,

        /// <summary>O meio de pagamento escolhido é o pagamento com cartão de débito.</summary>
        DebitCard = 2,
    }
}