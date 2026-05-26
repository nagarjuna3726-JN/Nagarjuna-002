namespace HotelStay.Api.Providers;

using HotelStay.Api.Models;

/// <summary>
/// BoutiqueCollection provider with deterministic stubbed responses.
/// Offers Deluxe and Suite only with £15/night boutique fee.
/// </summary>
public class BoutiqueCollectionProvider : IHotelProvider
{
    private const decimal BoutiqueFeePerNight = 15m;

    public string ProviderName => "BoutiqueCollection";

    public Task<IEnumerable<HotelSearchResult>> SearchAsync(
        string destination,
        DateTime checkIn,
        DateTime checkOut,
        string? roomType = null)
    {
        var nights = (checkOut - checkIn).Days;
        var results = new List<HotelSearchResult>
        {
            new()
            {
                RoomType = "Deluxe",
                PerNightRate = 180m + BoutiqueFeePerNight,
                TotalPrice = (180m + BoutiqueFeePerNight) * nights,
                CancellationPolicy = "Free cancellation up to 72 hours before",
                ProviderName = ProviderName,
                Amenities = new[] { "WiFi", "Air Conditioning", "Minibar", "Gym Access", "Concierge Service", "Premium Toiletries" },
                StarRating = 5,
                NumberOfNights = nights
            },
            new()
            {
                RoomType = "Suite",
                PerNightRate = 300m + BoutiqueFeePerNight,
                TotalPrice = (300m + BoutiqueFeePerNight) * nights,
                CancellationPolicy = "Free cancellation up to 72 hours before",
                ProviderName = ProviderName,
                Amenities = new[] { "WiFi", "Air Conditioning", "Minibar", "Gym Access", "Spa Access", "Concierge Service", "Premium Toiletries", "Personalized Service" },
                StarRating = 5,
                NumberOfNights = nights
            }
        };

        if (!string.IsNullOrEmpty(roomType))
        {
            results = results.Where(r => r.RoomType.Equals(roomType, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        return Task.FromResult(results.AsEnumerable());
    }

    public Task<BookingConfirmation> BookAsync(BookingRequest request)
    {
        // Validate that only Deluxe or Suite are requested
        if (!request.RoomType.Equals("Deluxe", StringComparison.OrdinalIgnoreCase) &&
            !request.RoomType.Equals("Suite", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("BoutiqueCollection only offers Deluxe and Suite room types.");
        }

        var confirmation = new BookingConfirmation
        {
            BookingReference = $"BC-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}",
            Provider = ProviderName,
            TotalPrice = 600m, // Stub value
            CancellationPolicy = "Free cancellation up to 72 hours before",
            Status = "Confirmed",
            CreatedAt = DateTime.UtcNow
        };

        return Task.FromResult(confirmation);
    }

    public Task<BookingStatus> GetBookingStatusAsync(string bookingReference)
    {
        var status = new BookingStatus
        {
            BookingReference = bookingReference,
            Status = "Confirmed",
            Destination = "Barcelona",
            CheckIn = DateTime.UtcNow.AddDays(2),
            CheckOut = DateTime.UtcNow.AddDays(6),
            RoomType = "Suite",
            Provider = ProviderName,
            TotalPrice = 1200m
        };

        return Task.FromResult(status);
    }
}
