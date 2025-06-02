namespace HotelManagementSystem.Logic
{
    public class FileStorageHelper
    {
        /// <summary>
        /// Garante que um ficheiro e a sua pasta existem antes de serem usados
        /// </summary>
        /// <param name="filePath"></param>
        public static void EnsureFileExists(string filePath)
        {
            string folder = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }

        }

        /// <summary>
        /// Lê todas as linhas de um ficheiro de txt e devolve-as como uma Lista de strings.
        /// Permite que se usem métodos da lista como .Add(), .Remove() ou .Where().
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static List<string> ReadAllLines(string filePath)
        {
            EnsureFileExists(filePath);

            string[] lines = File.ReadAllLines(filePath);
            return new List<string>(lines);
        }

        /// <summary>
        /// Escreve todas as linhas fornecidas no ficheiro indicado, substituindo qualquer conteúdo existente.
        /// </summary>
        /// <param name="filePath">Caminho completo do ficheiro onde as linhas serão escritas</param>
        /// <param name="linhas">Coleção de strings a escrever. Cada elemento corresponde a uma linha no ficheiro</param>
        public static void WriteAllLines(string filePath, IEnumerable<string> lines)
        {
            EnsureFileExists(filePath);
            File.WriteAllLines(filePath, lines); //Chama o método "WriteAllLines" e escreve no ficheiro especificado pelo filePath todas as linhas contidas na coleção linhas
        }

        /// <summary>
        /// Acrescenta uma nova linha no final de um ficheiro de texto.
        /// </summary>
        /// <param name="filePath">Caminho completo do ficheiro onde a linha será adicionada.</param>
        /// <param name="linha">Texto a ser escrito como nova linha no final do ficheiro.</param>
        public static void AppendLine(string filePath, string line)
        {
            EnsureFileExists(filePath);
            File.AppendAllText(filePath, line + Environment.NewLine); //O método "AppendAllText" serve para escrever texto no final de um ficheiro existente.
        }

    }
}
