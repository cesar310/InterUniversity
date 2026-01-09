namespace StudentEnrollment.Application.DTOs;

/// <summary>
/// Response estándar de la API
/// </summary>
public sealed record ApiResponse<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public string? Message { get; init; }
    public string? ErrorCode { get; init; }
    public object? Errors { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    public static ApiResponse<T> SuccessResponse(T data, string? message = null)
        => new() { Success = true, Data = data, Message = message };

    public static ApiResponse<T> ErrorResponse(string message, string? errorCode = null)
        => new() { Success = false, Message = message, ErrorCode = errorCode };
    
    public static ApiResponse<T> Error(string message, string errorCode, object? errors = null)
        => new() { Success = false, Message = message, ErrorCode = errorCode, Errors = errors };
}

/// <summary>
/// Response sin data (solo éxito/error)
/// </summary>
public sealed record ApiResponse
{
    public bool Success { get; init; }
    public string? Message { get; init; }
    public string? ErrorCode { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    public static ApiResponse SuccessResponse(string? message = null)
        => new() { Success = true, Message = message };

    public static ApiResponse ErrorResponse(string message, string? errorCode = null)
        => new() { Success = false, Message = message, ErrorCode = errorCode };
}
