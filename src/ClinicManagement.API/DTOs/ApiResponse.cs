namespace ClinicManagement.API.DTOs;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Code { get; set; } = string.Empty;
    public T? Data { get; set; }
    public string? Message { get; set; }
}

public class ApiResponse
{
    public bool Success { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? Message { get; set; }

    public static ApiResponse SuccessResponse(string code, string? message = null)
    {
        return new ApiResponse
        {
            Success = true,
            Code = code,
            Message = message
        };
    }

    public static ApiResponse<T> SuccessResponse<T>(string code, T data, string? message = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Code = code,
            Data = data,
            Message = message
        };
    }

    public static ApiResponse ErrorResponse(string code, string? message = null)
    {
        return new ApiResponse
        {
            Success = false,
            Code = code,
            Message = message
        };
    }

    public static ApiResponse<T> ErrorResponse<T>(string code, string? message = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Code = code,
            Message = message
        };
    }
}