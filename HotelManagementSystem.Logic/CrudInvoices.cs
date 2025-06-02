using HotelManagement.Core;

namespace HotelManagementSystem.Logic
{
    /// <summary>
    /// Classe responsável por gerir as operações relacionadas com faturas (Invoice).
    /// </summary>
    public class CrudInvoices
    {
        private string _filePath = @"Data\invoices.txt";

        public CrudInvoices()
        {
            FileStorageHelper.EnsureFileExists(_filePath);
        }

        /// <summary>
        /// Gera um novo ID único para a fatura.
        /// </summary>
        public int GenerateNextId()
        {
            List<Invoice> todas = GetAll();
            if (todas.Count == 0)
                return 1;

            int ultimoId = todas.Max(i => i.InvoiceId);
            return ultimoId + 1;
        }

        /// <summary>
        /// Adiciona uma nova fatura ao ficheiro.
        /// </summary>
        /// <param name="invoice">Fatura a guardar.</param>
        public void Add(Invoice invoice)
        {
            //Formato: ID;BookingId;GuestName;StayTotal;ExtrasTotal;IssueDate;PaymentMethod
            string linha = $"{invoice.InvoiceId};{invoice.BookingId};{invoice.GuestName};{invoice.StayTotal};{invoice.ExtrasTotal};{invoice.IssueDate};{invoice.PaymentMethod}";
            FileStorageHelper.AppendLine(_filePath, linha);
        }

        /// <summary>
        /// Devolve a lista de todas as faturas registadas.
        /// </summary>
        public List<Invoice> GetAll()
        {
            List<Invoice> lista = new List<Invoice>();
            List<string> linhas = FileStorageHelper.ReadAllLines(_filePath);

            foreach (string linha in linhas)
            {
                string[] partes = linha.Split(';');

                if (partes.Length == 7)
                {
                    int id;
                    int bookingId;
                    decimal stayTotal;
                    decimal extrasTotal;
                    DateTime issueDate;
                    PaymentMethod metodo;

                    bool idValido = int.TryParse(partes[0], out id);
                    bool bookingValido = int.TryParse(partes[1], out bookingId);
                    bool stayValido = decimal.TryParse(partes[3], out stayTotal);
                    bool extrasValido = decimal.TryParse(partes[4], out extrasTotal);
                    bool dataValida = DateTime.TryParse(partes[5], out issueDate);
                    bool metodoValido = Enum.TryParse(partes[6], out metodo);

                    if (idValido && bookingValido && stayValido && extrasValido && dataValida && metodoValido)
                    {
                        Invoice inv = new Invoice
                        {
                            InvoiceId = id,
                            BookingId = bookingId,
                            GuestName = partes[2],
                            StayTotal = stayTotal,
                            ExtrasTotal = extrasTotal,
                            IssueDate = issueDate,
                            PaymentMethod = metodo
                        };

                        lista.Add(inv);
                    }
                }
            }

            return lista;
        }

        /// <summary>
        /// Procura uma fatura com base no ID da reserva.
        /// </summary>
        /// <param name="bookingId">ID da reserva associada.</param>
        /// <returns>Fatura correspondente ou null se não for encontrada.</returns>
        public Invoice GetByBookingId(int bookingId)
        {
            List<Invoice> todas = GetAll();

            foreach (Invoice fatura in todas)
            {
                if (fatura.BookingId == bookingId)
                {
                    return fatura;
                }
            }

            return null;
        }

        /// <summary>
        /// Procura uma fatura com base no seu ID.
        /// </summary>
        /// <param name="id">ID da fatura.</param>
        /// <returns>Fatura correspondente ou null se não for encontrada.</returns>
        public Invoice GetById(int id)
        {
            List<Invoice> all = GetAll();

            foreach (Invoice invoice in all)
            {
                if (invoice.InvoiceId == id)
                {
                    return invoice;
                }
            }

            return null;
        }

        /// <summary>
        /// Substitui todas as faturas no ficheiro.
        /// </summary>
        public void SaveAll(List<Invoice> lista)
        {
            var linhas = lista.Select(i =>
                $"{i.InvoiceId};{i.BookingId};{i.GuestName};{i.StayTotal};{i.ExtrasTotal};{i.IssueDate};{i.PaymentMethod}");

            FileStorageHelper.WriteAllLines(_filePath, linhas);
        }

        /// <summary>
        /// Atualiza uma fatura existente no ficheiro.
        /// </summary>
        /// <param name="invoiceAtualizada">Fatura com os dados atualizados.</param>
        public void Update(Invoice invoiceAtualizada)
        {
            List<Invoice> todas = GetAll();

            for (int i = 0; i < todas.Count; i++)
            {
                if (todas[i].InvoiceId == invoiceAtualizada.InvoiceId)
                {
                    todas[i] = invoiceAtualizada;
                    break;
                }
            }

            List<string> linhas = new List<string>();

            foreach (Invoice inv in todas)
            {
                string linha = $"{inv.InvoiceId};{inv.BookingId};{inv.GuestName};{inv.StayTotal};{inv.ExtrasTotal};{inv.IssueDate};{inv.PaymentMethod}";
                linhas.Add(linha);
            }

            FileStorageHelper.WriteAllLines(_filePath, linhas);
        }
    }
}
