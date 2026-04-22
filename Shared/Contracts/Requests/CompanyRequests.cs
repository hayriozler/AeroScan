namespace Contracts.Requests;

public sealed record CreateCompanyRequest(
    string Code,
    string Name,
    string Type);

public sealed record UpdateCompanyRequest(
    string Code,
    string Name,
    string Type);
