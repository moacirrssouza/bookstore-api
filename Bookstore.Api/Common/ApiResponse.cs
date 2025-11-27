namespace Bookstore.Api.Common;

public class ApiResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public IEnumerable<string>? Errors { get; set; }


    public ApiResponse(bool success = true, string? message = null)
    {
        Success = success; Message = message;
    }
    public ApiResponse(string message) { Success = false; Message = message; }
}


public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; set; }
    public ApiResponse(T data) : base(true, null) { Data = data; }
}