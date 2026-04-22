namespace AeroScan.Contracts.Dtos;

public sealed record PassengerDto(
    int Id,
    string PaxName,
    string BookingReference,
    string SeatNumber,
    bool CheckedIn,
    DateTime? CheckInTime);
