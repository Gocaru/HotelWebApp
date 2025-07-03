namespace HotelWebApp.Models
{
    public class Result
    {
        public bool Succeeded { get; protected set; }
        public string Error { get; protected set; }

        public string Warning { get; protected set; }

        public static Result Success() => new Result { Succeeded = true };
        public static Result SuccessWithWarning(string warning) =>
            new Result { Succeeded = true, Warning = warning };

        public static Result Failure(string error) => new Result { Succeeded = false, Error = error };
    }
}
