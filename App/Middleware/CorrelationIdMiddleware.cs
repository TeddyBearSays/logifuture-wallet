using System.Globalization;
using WalletSytem.BusinessLayer;

namespace WalletSytem.Middleware;
public static class CorrelationIdMiddlewareExtensions
{
    public static IApplicationBuilder CorrelationIdMiddleware(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CorrelationIdMiddleware>();
    }
}

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private ICorrelationIdProvider correlationIdProvider;

    public CorrelationIdMiddleware(RequestDelegate next, IServiceProvider provider)
    {
        _next = next;
        Provider = provider;
    }

    public IServiceProvider Provider { get; }

    public async Task InvokeAsync(HttpContext context)
    {
        this.correlationIdProvider = (ICorrelationIdProvider)Provider.GetRequiredService(typeof(ICorrelationIdProvider));


        context.Request.Headers.TryGetValue("CorrelationId", out var value);

        Guid.TryParse(value, out Guid correlationId);
        this.correlationIdProvider.CorrelationId = correlationId;

       
        await _next(context);
    }
}