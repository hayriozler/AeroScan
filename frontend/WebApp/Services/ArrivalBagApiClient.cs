using Contracts.Dtos;

namespace WebApp.Services;

public sealed class ArrivalBagApiClient(HttpClient client)
{
    public async Task<IReadOnlyList<DepartureBagDto>> GetBagsForFlightAsync(string flightKey, CancellationToken ct = default)
    {
        try
        {
            var result = await client.GetFromJsonAsync<List<DepartureBagDto>>(
                $"/api/bags?flightKey={Uri.EscapeDataString(flightKey)}", ct);
            return result ?? [];
        }
        catch
        {
            return [];  
        }
    }
}
