namespace Api.Middleware
{
    public static class GlobalExceptionExtensions
    {
        //middleware dau tien trong program.cs
        public static IApplicationBuilder UseGlobalExceptionHandler(
            this IApplicationBuilder app) => app.UseMiddleware<GlobalException>();
    }
}
