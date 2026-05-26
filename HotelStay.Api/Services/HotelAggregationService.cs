namespace HotelStay.Api.Services;

using HotelStay.Api.Models;
using HotelStay.Api.Providers;

/// <summary>
/// Aggregates results from multiple hotel providers and normalizes them.
/// </summary>
public class HotelAggregationService
{
    private readonly IEnumerable<IHotelProvider> _providers;

    public HotelAggregationService(IEnumerable<IHotelProvider> providers)
    {
        _providers = providers;
    }

    /// <summary>
    /// Searches all providers and returns aggregated, normalized results.
    /// </summary>
    public async Task<IEnumerable<HotelSearchResult>> SearchAllProvidersAsync(
        string destination,
        DateTime checkIn,
        DateTime checkOut,
        string? roomType = null)
    {
        var tasks = _providers.Select(provider =>
            provider.SearchAsync(destination, checkIn, checkOut, roomType));

        var results = await Task.WhenAll(tasks);

        // Aggregate and flatten results
        var aggregated = results
            .SelectMany(r => r)
            .OrderBy(r => r.TotalPrice)
            .ThenBy(r => r.ProviderName)
            .ToList();

        return aggregated;
    }
}
