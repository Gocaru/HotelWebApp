using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Core
{
    public enum PaymentMethod { Cash, CreditCard, DebitCard }
    public class Invoice
    {
        public int InvoiceId { get; set; }
        public int BookingId { get; set; }

        [NotMapped]     //para não tentar guardar na base de dados
        public string GuestName { get; set; } = string.Empty; //Propreidade apenas para visualização (não para gravação na base de dados)
                                                              //A tabela Invoices não tem uma coluna GuestName, porque o nome do hóspede está armazenado na tabela Guests (relacionada com Bookings)

        public decimal StayTotal { get; set; }       //Total calculado com base no número de noites × preço por noite
        
        public decimal ExtrasTotal { get; set; }
        
        public decimal Total => StayTotal + ExtrasTotal;
        
        public DateTime IssueDate { get; set; }
        
        public PaymentMethod PaymentMethod { get; set; }
    }

}
