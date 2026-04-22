using BaggageService.Filters;
using BaggageService.Persistence;
using BaggageService.Services;
using Contracts.Dtos;
using Contracts.Requests;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BaggageService.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var auth = app.MapGroup("/api/auth");

        auth.MapPost("/login", Login)
            .WithName("Login")
            .AllowAnonymous()
            .AddEndpointFilter<ValidationFilter<LoginRequest>>()
            .Produces<LoginResponseDto>()
            .ProducesProblem(401);

        auth.MapPost("/change-password", ChangePassword)
            .WithName("ChangePassword")
            .RequireAuthorization()
            .AddEndpointFilter<ValidationFilter<ChangePasswordRequest>>()
            .Produces(204)
            .ProducesProblem(400)
            .ProducesProblem(401);

        auth.MapPost("/logout", Logout)
            .WithName("Logout")
            .AllowAnonymous()
            .Produces(204);

        //// DEBUG — remove after investigating auth issue
        //auth.MapGet("/debug-claims", (HttpContext ctx) =>
        //{
        //    var claims = ctx.User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        //    var isAuthenticated = ctx.User.Identity?.IsAuthenticated ?? false;
        //    var roles = ctx.User.Claims
        //        .Where(c => c.Type == System.Security.Claims.ClaimTypes.Role)
        //        .Select(c => c.Value).ToList();
        //    var isAdmin = ctx.User.IsInRole(Contracts.Consts.SystemDefinedRoles.Admin);
        //    var companyType = ctx.User.FindFirst("company_type")?.Value;
        //    return Results.Ok(new { isAuthenticated, roles, isAdmin, companyType, claims });
        //})
        //.RequireAuthorization()
        //.WithName("DebugClaims");

        return app;
    }

    private static async Task<Results<Ok<LoginResponseDto>, UnauthorizedHttpResult>> Login(
        LoginRequest request,
        AeroScanDataContext db,
        JwtTokenService tokenService,
        CancellationToken ct)
    {
        var user = await db.UserSet
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .Include(u => u.Company)
            .FirstOrDefaultAsync(
                u => u.Username == request.Username.ToLowerInvariant().Trim() , ct);

        if (user is null || !PasswordHasher.Verify(request.Password, user.PasswordHash))
            return TypedResults.Unauthorized();

        user.RecordLogin();
        await db.SaveChangesAsync(ct);

        var (token, expiresAt) = tokenService.GenerateToken(user);

        return TypedResults.Ok(new LoginResponseDto(
            Token:       token,
            Username:    user.Username,
            DisplayName: user.DisplayName,
            Roles:       [.. user.GetRoleNames()],
            CompanyCode:   user.CompanyCode,
            CompanyName: user.Company.Name,
            CompanyType: user.Company.Type.ToString(),
            ExpiresAt:   expiresAt));
    }

    private static async Task<Results<NoContent, BadRequest<string>, UnauthorizedHttpResult>> ChangePassword(
        ChangePasswordRequest request,
        AeroScanDataContext db,
        HttpContext httpContext,
        CancellationToken ct)
    {
        var subClaim = httpContext.User.FindFirst(JwtRegisteredClaimNames.Sub)
                    ?? httpContext.User.FindFirst(ClaimTypes.NameIdentifier);

        if (subClaim is null || !int.TryParse(subClaim.Value, out var userId))
            return TypedResults.Unauthorized();

        var user = await db.UserSet.FindAsync([userId], ct);
        if (user is null) return TypedResults.Unauthorized();

        if (!PasswordHasher.Verify(request.CurrentPassword, user.PasswordHash))
            return TypedResults.BadRequest("Current password is incorrect.");

        var result = user.UpdatePassword(PasswordHasher.Hash(request.NewPassword));
        if (result.IsFailure) return TypedResults.BadRequest(result.Error!);

        await db.SaveChangesAsync(ct);
        return TypedResults.NoContent();
    }

    private static NoContent Logout() => TypedResults.NoContent();
}
