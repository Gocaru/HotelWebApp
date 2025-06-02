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
    /// Classe auxiliar responsável por construir e validar um objeto Room a partir dos inputs da interface.
    /// </summary>
    public class RoomInputBuilder
    {

        /// <summary>
        /// Tenta construir um objeto Room a partir dos campos de texto e das seleções feitas pelo utilizador.
        /// Valida os dados e devolve um objeto Room válido ou uma mensagem de erro.
        /// </summary>
        /// <param name="txtNumber">Texto com o número do quarto</param>
        /// <param name="txtCapacity">Texto com a capacidade</param>
        /// <param name="txtPrice">Texto com o preço por noite</param>
        /// <param name="selectedType">Objeto selecionado no ComboBox do tipo</param>
        /// <param name="selectedStatus">Objeto selecionado no ComboBox do estado</param>
        /// <param name="room">Objeto Room construído, se bem-sucedido</param>
        /// <param name="error">Mensagem de erro, se houver</param>
        /// <returns>True se a construção e validação forem bem-sucedidas; false caso contrário</returns>
        public static bool TryBuildRoom(string txtNumber, string txtCapacity, string txtPrice,
                                        object selectedType, object selectedStatus,
                                        out Room room, out string error)
        {
            room = null;
            error = string.Empty;

            if (!int.TryParse(txtNumber.Trim(), out int number) || number <= 0)
            {
                error = "Room number must be a positive integer.";
                return false;
            }

            if (!int.TryParse(txtCapacity.Trim(), out int capacity) || capacity <= 0)
            {
                error = "Room capacity must be a positive integer.";
                return false;
            }

            if (!decimal.TryParse(txtPrice.Trim(), out decimal price) || price < 0)
            {
                error = "Price per night must be a valid non-negative number.";
                return false;
            }

            if (!(selectedType is RoomType type))
            {
                error = "Please select a valid room type.";
                return false;
            }

            if (!(selectedStatus is RoomStatus status))
            {
                error = "Please select a valid room status.";
                return false;
            }

            room = new Room
            {
                Number = number,
                Capacity = capacity,
                PricePerNight = price,
                Type = type,
                Status = status
            };

            return ValidationHelper.ValidateRoom(room, out error);  //Chama o resultado do método "ValidateRoom" (se for true, o room está válido, se for false a "error" vai conter a mensagem descritiva)
        }
    }
}
