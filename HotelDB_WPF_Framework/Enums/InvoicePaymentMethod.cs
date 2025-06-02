namespace HotelDB_WPF_Framework.Enums
{
    public enum InvoicePaymentMethod
    {
        /// <summary>O meio de pagamento escolhido é o pagamento em dinheiro.</summary>
        Cash = 0,

        /// <summary>O meio de pagamento escolhido é o pagamento com cartão de crédito.</summary>
        CreditCard = 1,

        /// <summary>O meio de pagamento escolhido é o pagamento com cartão de débito.</summary>
        DebitCard = 2,

        /// <summary>O meio de pagamento escolhido é o pagamento por MBWay.</summary>
        MBWay = 3
    }
}
