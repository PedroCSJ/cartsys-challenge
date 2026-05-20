namespace Cartsys.Application.Common;

public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T? Value { get; private set; }
    public string? ErrorMessage { get; private set; }
    public int StatusCode { get; private set; }

    private Result() { }

    public static Result<T> Ok(T value)
        => new() { IsSuccess = true, Value = value, StatusCode = 200 };

    public static Result<T> Created(T value)
        => new() { IsSuccess = true, Value = value, StatusCode = 201 };

    public static Result<T> Fail(string message, int statusCode = 400)
        => new() { IsSuccess = false, ErrorMessage = message, StatusCode = statusCode };

    public static Result<T> NotFound(string message = "Registro não encontrado.")
        => new() { IsSuccess = false, ErrorMessage = message, StatusCode = 404 };

    public static Result<T> Conflict(string message)
        => new() { IsSuccess = false, ErrorMessage = message, StatusCode = 409 };
}

public class Result
{
    public bool IsSuccess { get; private set; }
    public string? ErrorMessage { get; private set; }
    public int StatusCode { get; private set; }

    private Result() { }

    public static Result Ok()
        => new() { IsSuccess = true, StatusCode = 200 };

    public static Result Fail(string message, int statusCode = 400)
        => new() { IsSuccess = false, ErrorMessage = message, StatusCode = statusCode };

    public static Result NotFound(string message = "Registro não encontrado.")
        => new() { IsSuccess = false, ErrorMessage = message, StatusCode = 404 };

    public static Result Conflict(string message)
        => new() { IsSuccess = false, ErrorMessage = message, StatusCode = 409 };
}
