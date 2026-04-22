using Contracts.Dtos;

namespace WebApp.Services;

public sealed class DepartureBagApiClient(HttpClient client)
{
    public async Task<IReadOnlyList<DepartureBagDto>> GetBagsForFlightAsync(int flightId, CancellationToken ct = default)
    {
        try
        {
            var result = await client.GetFromJsonAsync<List<DepartureBagDto>>(
                $"/api/bags?flightKey={flightId}", ct);
            return result ?? [];
        }
        catch
        {
            return [];  
        }
    }
}
