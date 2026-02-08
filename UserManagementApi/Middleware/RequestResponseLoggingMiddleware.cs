using System.Text;

namespace UserManagementAPI.Middleware
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Request.EnableBuffering();

            var requestBody = await ReadStreamByChunks(context.Request.Body);
            _logger.LogInformation($"[Incoming Request] {context.Request.Method} {context.Request.Path} Body: {requestBody}");
            context.Request.Body.Position = 0;

            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            _logger.LogInformation($"[Outgoing Response] Status: {context.Response.StatusCode} Body: {responseText}");

            await responseBody.CopyToAsync(originalBodyStream);
        }

        private static async Task<string> ReadStreamByChunks(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            using var textWriter = new StringWriter();
            using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 1024, leaveOpen: true);

            var buffer = new char[1024];
            int read;
            while ((read = await reader.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                textWriter.Write(buffer, 0, read);
            }
            return textWriter.ToString();
        }
    }
}