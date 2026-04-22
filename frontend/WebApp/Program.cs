using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using MudBlazor.Services;
using System.Globalization;
using WebApp.EndPoints;
using WebApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
// ── Cookie Authentication ──────────────────────────────────────────────────
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.AccessDeniedPath = "/login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.Name = "brs.session";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.Strict;
    });

builder.Services.AddAuthorization();

// ── Localization ──────────────────────────────────────────────────────────
builder.Services.AddLocalization();

// ── Blazor ────────────────────────────────────────────────────────────────
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options =>
    {
        // Show real exception details in the browser console during development.
        options.DetailedErrors = builder.Environment.IsDevelopment();
    });

// ── HTTP Client → API Gateway ─────────────────────────────────────────────
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<AuthTokenHandler>();
builder.Services.AddScoped<ThemeService>();
builder.Services.AddScoped<UserSettingsService>();
builder.Services.AddScoped<ClientTimeService>();
builder.Services.AddMudServices();

static void ConfigureGatewayClient(HttpClient client) =>
    client.BaseAddress = new Uri("https+http://api-gateway");

builder.Services.AddHttpClient<AuthApiClient>(ConfigureGatewayClient)
    .AddHttpMessageHandler<AuthTokenHandler>();

builder.Services.AddHttpClient<DashboardApiClient>(ConfigureGatewayClient)
    .AddHttpMessageHandler<AuthTokenHandler>();

builder.Services.AddHttpClient<DepartureFlightApiClient>(ConfigureGatewayClient)
    .AddHttpMessageHandler<AuthTokenHandler>();

builder.Services.AddHttpClient<ArrivalFlightApiClient>(ConfigureGatewayClient)
    .AddHttpMessageHandler<AuthTokenHandler>();

builder.Services.AddHttpClient<DepartureBagApiClient>(ConfigureGatewayClient)
    .AddHttpMessageHandler<AuthTokenHandler>();

builder.Services.AddHttpClient<HandlingContractApiClient>(ConfigureGatewayClient)
    .AddHttpMessageHandler<AuthTokenHandler>();

builder.Services.AddHttpClient<CompanyApiClient>(ConfigureGatewayClient)
    .AddHttpMessageHandler<AuthTokenHandler>();

builder.Services.AddHttpClient<RoleApiClient>(ConfigureGatewayClient)
    .AddHttpMessageHandler<AuthTokenHandler>();

builder.Services.AddHttpClient<UserApiClient>(ConfigureGatewayClient)
    .AddHttpMessageHandler<AuthTokenHandler>();

builder.Services.AddHttpClient<HhtApiClient>(ConfigureGatewayClient)
    .AddHttpMessageHandler<AuthTokenHandler>();

builder.Services.AddHttpClient<ContainerApiClient>(ConfigureGatewayClient)
    .AddHttpMessageHandler<AuthTokenHandler>();

builder.Services.AddHttpClient<AuditApiClient>(ConfigureGatewayClient)
    .AddHttpMessageHandler<AuthTokenHandler>();


builder.Services.AddHttpClient<SystemConfigurationApiClient>(ConfigureGatewayClient)
    .AddHttpMessageHandler<AuthTokenHandler>();

// ── Antiforgery (for SSR forms) ───────────────────────────────────────────
builder.Services.AddAntiforgery();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseAntiforgery();

// ── Request Localization — reads .AspNetCore.Culture cookie ───────────────
var supportedCultures = new[] { new CultureInfo("en"), new CultureInfo("tr") };
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en"),
    SupportedCultures     = supportedCultures,
    SupportedUICultures   = supportedCultures,
    RequestCultureProviders = [new CookieRequestCultureProvider()]
});

app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();

app.MapDefaultEndpoints();

app.MapRazorComponents<WebApp.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
