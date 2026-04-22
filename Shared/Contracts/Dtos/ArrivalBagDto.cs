using Domain.Enums;

namespace Contracts.Dtos;

public sealed record ArrivalBagDto(
    long Id,
    string TagNumber,
    ArrivalBaggageStatus ArrivalBaggageStatus,
    BaggageClass Class,
    int? PassengerId,
    bool IsTransfer
    );