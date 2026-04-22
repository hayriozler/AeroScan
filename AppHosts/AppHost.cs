var builder = DistributedApplication.CreateBuilder(args);

// ── Infrastructure ─────────────────────────────────────────────────────────
var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin()
    .AddDatabase("aeroscan");

// ── Backend ────────────────────────────────────────────────────────────────
var baggageService = builder.AddProject<Projects.BaggageService>("project-aeroscan")
    .WithReference(postgres)
    .WaitFor(postgres);

// ── API Gateway ────────────────────────────────────────────────────────────
var apiGateway = builder.AddProject<Projects.ApiGateway>("api-gateway")
    .WithReference(baggageService)
    .WaitFor(baggageService);

// ── Web Frontend ───────────────────────────────────────────────────────────
builder.AddProject<Projects.WebApp>("web-app")
    .WithReference(apiGateway)
    .WaitFor(apiGateway);

builder.Build().Run();
