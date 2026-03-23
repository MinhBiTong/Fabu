using Application.DTOs.Requests;
using Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Serilog;
using System.Net;
using System.Text.Json;

namespace Api.Middleware
{
    public class GlobalException
    {
        private static string MIN_ATTRIBUTE = "min";
        //define cac loi
        //bat loi
        //tra loi
        private readonly RequestDelegate _next;
        private readonly Serilog.ILogger _logger = Log.ForContext<GlobalException>();

        public GlobalException(RequestDelegate next)
        {
            _next = next;
        }

        //middleware entry point
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (AppException appEx)
            {
                await HandleAppExceptionAsync(context, appEx);
            } catch (Exception ex)
            {
                await HandleUnexpectedExceptionAsync(context, ex);
            }
        }

        //xu ly logic
        private async Task HandleAppExceptionAsync(HttpContext context, AppException appEx)
        {
            var errorCode = appEx.GetErrorCode();
            var errorDetail = errorCode.GetDetails();

            //enrich log with error code and details
            _logger
                .ForContext("ErrorCode", errorCode.ToString())
                .ForContext("ErrorMessage", errorDetail.Message)
                .ForContext("HttpStatus", (int)errorDetail.StatusCode)
                .ForContext("RequestPath", context.Request.Path)
                .ForContext("RequestMethod", context.Request.Method)
                .Warning(
                    "[AppException] {ErrorCode} - {ErrorMessage} | Path: {RequestPath}",
                    errorCode,
                    errorDetail.Message,
                    context.Request.Path
                );
            var response = ApiResponse<object>.Fail(
                    code: errorDetail.Code,
                    message: ResolveMessage(errorDetail.Message)
                );

            await WriteJsonResponseAsync(context, errorDetail.StatusCode, response);
        }

        //unexpected exception handler - catch all
        private async Task HandleUnexpectedExceptionAsync(HttpContext context, Exception ex)
        {
            var errorDetail = ErrorCode.UNCATEDORIZED_EXCEPTION.GetDetails();

            //FULL exception detail dc gui den seq de quan sat
            _logger
                .ForContext("RequestPath", context.Request.Path)
                .ForContext("RequestMethod", context.Request.Method)
                .ForContext("ExceptionType", ex.GetType().Name)
                .Error(
                    ex,
                    "[UnhandledException] {ExceptionType} | Path: {RequestPath}",
                    ex.GetType().Name,
                    context.Request.Path);
            var response = ApiResponse<object>.Fail(
                code: errorDetail.Code,
                message: errorDetail.Message);

            await WriteJsonResponseAsync(context, errorDetail.StatusCode, response);
        }

        //Thay thế các phần giữ chỗ mẫu như {min} trong thông báo lỗi.
        private static string ResolveMessage(string template)
        {
            var placeholder = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { MIN_ATTRIBUTE, "6" } //default value for min, có thể mở rộng để hỗ trợ nhiều placeholder khác nếu cần
            };

            foreach (var (key, value) in placeholder)
                template = template.Replace($"{{{key}", value, StringComparison.OrdinalIgnoreCase);

            return template;
        }

        //Writes a JSON-serialised <see cref="ApiResponse{T}"/> to the HTTP response.
        private static async Task WriteJsonResponseAsync<T>(
                HttpContext context,
                HttpStatusCode statusCode,
                ApiResponse<T> body)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var json = JsonSerializer.Serialize(body, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
             });

            await context.Response.WriteAsync(json);
        }
    }
}
