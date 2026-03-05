using System.Collections.Concurrent;

namespace CleanMvcApp.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimitingMiddleware> _logger;
        private static readonly ConcurrentDictionary<string, (DateTime FirstRequest, int Count)> _requestCounts = new();
        private const int MaxRequestsPerMinute = 60;
        private const int TimeWindowSeconds = 60;

        public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var now = DateTime.UtcNow;

            // Clean up old entries
            CleanupOldEntries(now);

            // Check rate limit
            if (_requestCounts.TryGetValue(ipAddress, out var requestInfo))
            {
                var timeSinceFirstRequest = (now - requestInfo.FirstRequest).TotalSeconds;

                if (timeSinceFirstRequest < TimeWindowSeconds)
                {
                    if (requestInfo.Count >= MaxRequestsPerMinute)
                    {
                        _logger.LogWarning("Rate limit exceeded for IP: {IpAddress}", ipAddress);
                        context.Response.StatusCode = 429; // Too Many Requests
                        await context.Response.WriteAsync("Rate limit exceeded. Please try again later.");
                        return;
                    }

                    _requestCounts[ipAddress] = (requestInfo.FirstRequest, requestInfo.Count + 1);
                }
                else
                {
                    // Reset counter for new time window
                    _requestCounts[ipAddress] = (now, 1);
                }
            }
            else
            {
                // First request from this IP
                _requestCounts[ipAddress] = (now, 1);
            }

            await _next(context);
        }

        private void CleanupOldEntries(DateTime now)
        {
            var keysToRemove = _requestCounts
                .Where(kvp => (now - kvp.Value.FirstRequest).TotalSeconds > TimeWindowSeconds)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in keysToRemove)
            {
                _requestCounts.TryRemove(key, out _);
            }
        }
    }
}
