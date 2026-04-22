using Domain.Enums;

namespace Contracts.Dtos;

public sealed record ReconciliationDto(
    int Id,
    ReconciliationStatus Status,
    int TotalChecked,
    int TotalLoaded,
    int TotalOffloaded,
    int TotalTransferred,
    IReadOnlyList<string> UnmatchedBags,
    IReadOnlyList<string> OffloadedBags,
    IReadOnlyList<string> RushBags,
    DateTime? ClosedAt,
    string? ClosedBy);
