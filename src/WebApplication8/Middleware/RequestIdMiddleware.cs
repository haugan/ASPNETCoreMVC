using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication8.Services;

namespace WebApplication8.Middleware
{
    public class RequestIdMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestIdMiddleware> _logger;

        public RequestIdMiddleware(RequestDelegate next, ILogger<RequestIdMiddleware> logger, IRequestId requestId)
        {
            _next = next;
            _logger = logger;
        }

        public Task Invoke(HttpContext context, IRequestId requestId)
        {
            _logger.LogInformation($"Request {requestId.Id}");
            return _next(context);
        }
    }
}
