namespace NutriMind.Web.Services;

public class ApiResult
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }

    public static ApiResult Success() => new() { IsSuccess = true };
    public static ApiResult Failure(string message) => new() { IsSuccess = false, ErrorMessage = message };
}

public class ApiResult<T>
{
    public bool IsSuccess { get; init; }
    public T? Data { get; init; }
    public string? ErrorMessage { get; init; }

    public static ApiResult<T> Success(T data) => new() { IsSuccess = true, Data = data };
    public static ApiResult<T> Failure(string message) => new() { IsSuccess = false, ErrorMessage = message };
}
