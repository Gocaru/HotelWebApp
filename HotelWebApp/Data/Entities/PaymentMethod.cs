namespace HotelWebApp.Data.Entities
{
    /// <summary>
    /// Defines the possible methods a guest can use to make a payment.
    /// </summary>
    public enum PaymentMethod
    {
        /// <summary>
        /// Payment made with physical cash.
        /// </summary>
        Cash = 0,

        /// <summary>
        /// Payment made via a credit card.
        /// </summary>
        CreditCard = 1,

        /// <summary>
        /// Payment made via a debit card.
        /// </summary>
        DebitCard = 2,

        /// <summary>
        /// Payment made via a direct bank transfer.
        /// </summary>
        BankTransfer = 3
    }
}
