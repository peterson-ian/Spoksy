using System.Text.Json.Serialization;

namespace Spoksy.API.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; private set; }
        public T Data { get; private set; }
        public string Message { get; private set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<string> Errors { get; private set; }

        public ApiResponse(T data, string message = null)
        {
            Success = true;
            Data = data;
            Message = message;
        }

        public ApiResponse(string message)
        {
            Success = false;
            Message = message;
        }

        public ApiResponse(string message, IEnumerable<string> errors)
        {
            Success = false;
            Message = message;
            Errors = errors;
        }

        public ApiResponse( IEnumerable<string> errors)
        {
            Success = false;
            Message = "";
            Errors = errors;
        }
    }
} 