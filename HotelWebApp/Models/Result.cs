namespace HotelWebApp.Models
{
    /// <summary>
    /// Represents the outcome of an operation. This is a non-generic version used for operations
    /// that do not return a specific data payload (e.g., Delete or Update).
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Gets a value indicating whether the operation was successful.
        /// </summary>
        public bool Succeeded { get; protected set; }

        /// <summary>
        /// Gets the error message if the operation failed. This will be empty on success.
        /// </summary>
        public string Error { get; protected set; } = string.Empty;

        /// <summary>
        /// Gets an optional warning message for operations that succeeded but have additional information.
        /// </summary>
        public string Warning { get; protected set; } = string.Empty;

        /// <summary>
        /// Gets an optional success message providing more details about the successful operation.
        /// </summary>
        public string Message { get; protected set; } = string.Empty;

        /// <summary>
        /// Creates a new successful result.
        /// </summary>
        /// <param name="message">An optional message describing the success.</param>
        /// <returns>A successful Result object.</returns>
        public static Result Success(string message = "") => 
            new Result { Succeeded = true, Message = message };

        /// <summary>
        /// Creates a new successful result that includes a warning.
        /// </summary>
        /// <param name="warning">The warning message.</param>
        /// <returns>A successful Result object with a warning.</returns>
        public static Result SuccessWithWarning(string warning) =>
            new Result { Succeeded = true, Warning = warning };

        /// <summary>
        /// Creates a new failed result.
        /// </summary>
        /// <param name="error">The error message describing the failure.</param>
        /// <returns>A failed Result object.</returns>
        public static Result Failure(string error) 
            => new Result { Succeeded = false, Error = error };
    }

    /// <summary>
    /// Represents the outcome of an operation that returns a data payload of type T.
    /// This generic version inherits from the base Result class.
    /// </summary>
    /// <typeparam name="T">The type of the data payload.</typeparam>
    public class Result<T> : Result
    {
        /// <summary>
        /// Gets the data payload from a successful operation.
        /// This will be null or default if the operation failed.
        /// </summary>
        public T? Data { get; private set; }

        /// <summary>
        /// Creates a new successful result with a data payload.
        /// </summary>
        /// <param name="data">The data payload to return.</param>
        /// <param name="message">An optional success message.</param>
        /// <returns>A successful Result object containing the data.</returns>
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

        /// <summary>
        /// Creates a new failed result for an operation that was expected to return data.
        /// The 'new' keyword is used to hide the base class's Failure method.
        /// </summary>
        /// <param name="error">The error message describing the failure.</param>
        /// <returns>A failed Result object with a null data payload.</returns>
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
