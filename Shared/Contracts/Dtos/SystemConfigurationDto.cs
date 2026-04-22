namespace Contracts.Dtos;

public sealed record SystemConfigurationDto(
    string Key,
    string Value,
    string? Description);
