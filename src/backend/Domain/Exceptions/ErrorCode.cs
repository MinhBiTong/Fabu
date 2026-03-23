using System.Net;

namespace Domain.Exceptions
{
    public enum ErrorCode
    {
        INTERNAL_SERVER_ERROR = 9998,
        UNCATEDORIZED_EXCEPTION = 9999,
        INVALID_KEY = 1001,
        USER_EXISTED = 1002,
        USERNAME_INVALID = 1003,
        PASSWORD_INVALID = 1004,
        USER_NOT_EXISTED = 1005,
        UNAUTHENTICATED = 1006,
        UNAUTHORIZED = 1007,
        INVALID_DOB = 1008,
    }

    public class ErrorDetails
    {
        public int Code { get; }
        public string Message { get; }
        public HttpStatusCode StatusCode { get; }

        public ErrorDetails(int code, string message, HttpStatusCode statusCode)
        {
            Code = code;
            Message = message;
            StatusCode = statusCode;
        }
    }

    public static class ErrorCodeExtensions
    {
        public static ErrorDetails GetDetails(this ErrorCode errorCode)
        {
            return errorCode switch
            {
                ErrorCode.INTERNAL_SERVER_ERROR => new ErrorDetails(9998, "Internal server error", HttpStatusCode.InternalServerError),
                ErrorCode.UNCATEDORIZED_EXCEPTION => new ErrorDetails(9999, "Uncategorized error", HttpStatusCode.InternalServerError),
                ErrorCode.INVALID_KEY => new ErrorDetails(1001, "Invalid message key", HttpStatusCode.BadRequest),
                ErrorCode.USER_EXISTED => new ErrorDetails(1002, "User already existed", HttpStatusCode.BadRequest),
                ErrorCode.USERNAME_INVALID => new ErrorDetails(1003, "Username must be at least {min} characters", HttpStatusCode.BadRequest),
                ErrorCode.PASSWORD_INVALID => new ErrorDetails(1004, "Password must be at least {min} characters", HttpStatusCode.BadRequest),
                ErrorCode.USER_NOT_EXISTED => new ErrorDetails(1005, "User not existed", HttpStatusCode.NotFound),
                ErrorCode.UNAUTHENTICATED => new ErrorDetails(1006, "Unauthenticated", HttpStatusCode.Unauthorized),
                ErrorCode.UNAUTHORIZED => new ErrorDetails(1007, "You do not have permission", HttpStatusCode.Forbidden),
                ErrorCode.INVALID_DOB => new ErrorDetails(1008, "Your age must be at least {min} years old", HttpStatusCode.BadRequest),
                _ => throw new ArgumentOutOfRangeException(nameof(errorCode), errorCode, null)
            };
        }
    }
}
