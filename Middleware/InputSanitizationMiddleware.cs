using System.Text;
using System.Text.Encodings.Web;

namespace CleanMvcApp.Middleware
{
    public class InputSanitizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<InputSanitizationMiddleware> _logger;

        public InputSanitizationMiddleware(RequestDelegate next, ILogger<InputSanitizationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Sanitize query string parameters
            if (context.Request.Query.Any())
            {
                var sanitizedQuery = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>();
                foreach (var param in context.Request.Query)
                {
                    var sanitizedValue = HtmlEncoder.Default.Encode(param.Value.ToString());
                    sanitizedQuery[param.Key] = sanitizedValue;
                }
                
                // Note: Query string is read-only, so we log suspicious input instead
                foreach (var param in context.Request.Query)
                {
                    if (ContainsSuspiciousContent(param.Value.ToString()))
                    {
                        _logger.LogWarning("Suspicious input detected in query parameter {Key}: {Value}", 
                            param.Key, param.Value);
                    }
                }
            }

            await _next(context);
        }

        private bool ContainsSuspiciousContent(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            var suspiciousPatterns = new[]
            {
                "<script", "javascript:", "onerror=", "onload=",
                "eval(", "expression(", "vbscript:", "data:text/html"
            };

            return suspiciousPatterns.Any(pattern => 
                input.Contains(pattern, StringComparison.OrdinalIgnoreCase));
        }
    }
}
