using Domain.Enums;

namespace Contracts.Requests;

public sealed record SubmitScanRequest(
    string DeviceId,
    string TagNumber,
    string LocationCode,
    ScanType ScanType,
    string FlightKey,
    DateTime DeviceTimestamp,
    string OperatorId,
    bool IsOffline,
    double? Latitude = null,
    double? Longitude = null);
