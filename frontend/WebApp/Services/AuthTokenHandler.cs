using System.Net.Http.Headers;

namespace WebApp.Services;

/// <summary>
/// Reads the JWT stored in the server-side cookie claim and attaches it
/// as a Bearer token on every outgoing request to the API Gateway.
/// The JWT never leaves the server — the browser only sees the HttpOnly cookie.
/// </summary>
public sealed class AuthTokenHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var jwt = httpContextAccessor.HttpContext?.User.FindFirst("jwt")?.Value;
        if (!string.IsNullOrEmpty(jwt))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

        return base.SendAsync(request, cancellationToken);
    }
}
