using Domain.Enums;

namespace Contracts.Requests;

public sealed record OffloadBagRequest(
    OffloadReason Reason,
    string OperatorId,
    string DeviceId,
    string LocationCode);
