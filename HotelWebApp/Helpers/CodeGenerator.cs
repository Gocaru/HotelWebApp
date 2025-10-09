namespace HotelWebApp.Helpers
{
    public static class CodeGenerator
    {
        public static string GenerateNumericCode(int length = 6)
        {
            var random = new Random();
            var code = string.Empty;

            for (int i = 0; i < length; i++)
            {
                code += random.Next(0, 10).ToString();
            }

            return code;
        }
    }
}