using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Requests
{
    public class ApiResponse<T>
    {
        public int Code { get; private set; }
        public string Message { get; private set; }
        public T? Result { get; private set; }

        private ApiResponse() { Message = string.Empty; }

        public static ApiResponse<T> Success(T result, string message = "Success")
            => new() { Code = 200, Message = message, Result = result };

        // Non-200 response with no payload
        public static ApiResponse<T> Fail(int code, string message)
            => new() { Code = code, Message = message, Result = default };
    }
}
