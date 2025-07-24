namespace HotelWebApp.Models
{
    public class Result
    {
        public bool Succeeded { get; protected set; }
        
        public string Error { get; protected set; } = string.Empty;
        
        public string Warning { get; protected set; } = string.Empty;

        public string Message { get; protected set; } = string.Empty;

        public static Result Success(string message = "") => 
            new Result { Succeeded = true, Message = message };

        public static Result SuccessWithWarning(string warning) =>
            new Result { Succeeded = true, Warning = warning };

        public static Result Failure(string error) 
            => new Result { Succeeded = false, Error = error };
    }

    public class Result<T> : Result
    {
        public T? Data { get; private set; }

        public static Result<T> Success(T data, string message = "")
        {
            var result = new Result<T>
            {
                Succeeded = true,
                Data = data,
                Message = message
            };
            return result;
        }

        public new static Result<T> Failure(string error)
        {
            var result = new Result<T>
            {
                Succeeded = false,
                Error = error     
            };
            return result;
        }
    }
}
