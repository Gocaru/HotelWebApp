using HotelDB_WPF_Framework.Enums;
using HotelDB_WPF_Framework.Models;

namespace HotelDB_WPF_Framework.Helpers.InputBuilders
{
    /// <summary>
    /// Classe auxiliar responsável por construir e validar um objeto ExtraService a partir dos inputs da interface.
    /// </summary>
    public class ExtraServiceInputBuilder
    {
        /// <summary>
        /// Tenta construir um objeto ExtraService a partir dos campos da interface.
        /// </summary>
        public static bool TryBuildExtraService(string txtBookingId, object selectedType,
                                                string txtPrice, out ExtraService extra, out string error)
        {
            extra = null;
            error = "";

            if (!int.TryParse(txtBookingId.Trim(), out int bookingId) || bookingId <= 0)
            {
                error = "Invalid booking ID.";
                return false;
            }

            if (!(selectedType is ExtraServiceType type))
            {
                error = "Please select a valid service type.";
                return false;
            }

            if (!decimal.TryParse(txtPrice.Trim(), out decimal price) || price < 0)
            {
                error = "Price must be a valid non-negative number.";
                return false;
            }

            extra = new ExtraService
            {
                BookingId = bookingId,
                ServiceType = type,
                Price = price
            };

            return ValidationHelper.ValidateExtraService(extra, out error);
        }
    }
}
