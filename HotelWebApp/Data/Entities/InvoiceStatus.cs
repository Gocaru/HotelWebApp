namespace HotelWebApp.Data.Entities
{
    /// <summary>
    /// Defines the possible payment statuses for an Invoice.
    /// </summary>
    public enum InvoiceStatus
    {
        /// <summary>
        /// The invoice has been generated, but no payment has been received yet.
        /// </summary>
        Unpaid = 0,

        /// <summary>
        /// The invoice has been fully paid. The total amount has been received.
        /// </summary>
        Paid = 1,

        /// <summary>
        /// One or more payments have been made, but the full amount has not yet been received.
        /// </summary>
        PartiallyPaid = 2,

        /// <summary>
        /// The invoice has been voided or cancelled, usually because the associated reservation was cancelled.
        /// </summary>
        Cancelled = 3
    }
}
