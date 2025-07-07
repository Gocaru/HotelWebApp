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

    public class Result<T> : Result
    {
        public T Data { get; private set; }

        // Um novo método "Success" que aceita o dado como argumento.
        public static Result<T> Success(T data)
            => new Result<T> { Succeeded = true, Data = data };

        // Usamos 'new' para criar um novo método 'Failure' que retorna o tipo correto (Result<T>)
        public new static Result<T> Failure(string error)
            => new Result<T> { Succeeded = false, Error = error };
    }
}
