namespace Contracts.Dtos;

public sealed record CompanyDto(
    string Code,
    string Name,
    string Type,
    DateTime CreatedAt);
