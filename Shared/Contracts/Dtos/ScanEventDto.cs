using Domain.Enums;

namespace Contracts.Dtos;

public sealed record ScanEventDto(
    int Id,
    string TagNumber,
    ScanType Type,
    string LocationCode,
    string DeviceId,
    string OperatorId,
    DateTime DeviceTimestamp,
    DateTime ServerTimestamp);
