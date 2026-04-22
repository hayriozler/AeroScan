using Contracts.Dtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using WebApp.Services;

namespace WebApp.EndPoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var auth = app.MapGroup("/auth");

        auth.MapPost("/signin", async (
            HttpContext ctx,
            AuthApiClient api,
            string? returnUrl) =>
        {
            var form     = ctx.Request.Form;
            var username = form["username"].ToString().Trim();
            var password = form["password"].ToString();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return Results.Redirect("/login?error=missing");

            LoginResponseDto? response;
            try
            {
                response = await api.LoginAsync(username, password);
            }
            catch
            {
                return Results.Redirect("/login?error=server");
            }

            if (response is null)
                return Results.Redirect("/login?error=invalid");

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name,  response.Username),
                new("display_name",   response.DisplayName),
                new("company_id",     response.CompanyCode.ToString()),
                new("company_name",   response.CompanyName),
                new("company_type",   response.CompanyType),
                new("jwt",            response.Token),   // stored server-side in the encrypted cookie
            };

            // One claim per role — consumed by Blazor AuthorizeView[Roles="..."]
            foreach (var role in response.Roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var identity  = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await ctx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                new AuthenticationProperties
                {
                    IsPersistent = false,
                    ExpiresUtc   = response.ExpiresAt
                });

            var redirect = !string.IsNullOrEmpty(returnUrl) && Uri.IsWellFormedUriString(returnUrl, UriKind.Relative)
                ? returnUrl
                : "/";

            return Results.Redirect(redirect);
        }).AllowAnonymous().DisableAntiforgery();

        auth.MapGet("/signout", async (HttpContext ctx) =>
        {
            await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Results.Redirect("/login");
        }).AllowAnonymous();

        return app;
    }
}
