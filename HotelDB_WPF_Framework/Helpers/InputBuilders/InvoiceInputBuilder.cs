using HotelDB_WPF_Framework.Enums;
using HotelDB_WPF_Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelDB_WPF_Framework.Helpers.InputBuilders
{
    /// <summary>
    /// Classe auxiliar responsável por construir e validar um objeto Invoice a partir dos inputs da interface.
    /// </summary>
    public class InvoiceInputBuilder
    {
        /// <summary>
        /// Tenta construir um objeto Invoice a partir dos campos da interface.
        /// </summary>
        public static bool TryBuildInvoice(string txtBookingId, string txtExtrasTotal,
                                           object selectedPaymentMethod,
                                           out Invoice invoice, out string error)
        {
            invoice = null;
            error = "";

            if (!int.TryParse(txtBookingId.Trim(), out int bookingId) || bookingId <= 0)
            {
                error = "Invalid booking ID.";
                return false;
            }

            if (!decimal.TryParse(txtExtrasTotal.Trim(), out decimal extrasTotal) || extrasTotal < 0)
            {
                error = "Extras total must be a non-negative number.";
                return false;
            }

            if (!(selectedPaymentMethod is InvoicePaymentMethod paymentMethod))
            {
                error = "Please select a valid payment method.";
                return false;
            }

            invoice = new Invoice
            {
                BookingId = bookingId,
                ExtrasTotal = extrasTotal,
                PaymentMethod = paymentMethod,
                IssueDate = DateTime.Now,
                StayTotal = 0, // a preencher externamente
                Total = 0      // a preencher externamente
            };

            return ValidationHelper.ValidateInvoice(invoice, out error);
        }
    }
}
