namespace Domain.Exceptions
{
    public class AppException : Exception
    {
        private ErrorCode _errorCode;

        public AppException(ErrorCode errorCode)
        {
            _errorCode = errorCode;
        }

        public AppException(ErrorCode errorCode, string message)
              : base(message)
        {
            _errorCode = errorCode;
        }

        public ErrorCode GetErrorCode()
        {
            return _errorCode;
        }

        public void SetErrorCode(ErrorCode errorCode)
        {
            _errorCode = errorCode;
        }
    }
}
