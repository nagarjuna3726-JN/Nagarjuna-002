namespace HotelStay.Api.Providers;

using HotelStay.Api.Models;

/// <summary>
/// PremierStays provider with deterministic stubbed responses (PascalCase JSON).
/// </summary>
public class PremierStaysProvider : IHotelProvider
{
    public string ProviderName => "PremierStays";

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
                RoomType = "Standard",
                PerNightRate = 100m,
                TotalPrice = 100m * nights,
                CancellationPolicy = "Free cancellation up to 7 days before",
                ProviderName = ProviderName,
                Amenities = new[] { "WiFi", "Air Conditioning", "TV" },
                StarRating = 4,
                NumberOfNights = nights
            },
            new()
            {
                RoomType = "Deluxe",
                PerNightRate = 150m,
                TotalPrice = 150m * nights,
                CancellationPolicy = "Free cancellation up to 7 days before",
                ProviderName = ProviderName,
                Amenities = new[] { "WiFi", "Air Conditioning", "TV", "Minibar", "Gym Access" },
                StarRating = 4,
                NumberOfNights = nights
            },
            new()
            {
                RoomType = "Suite",
                PerNightRate = 250m,
                TotalPrice = 250m * nights,
                CancellationPolicy = "Free cancellation up to 7 days before",
                ProviderName = ProviderName,
                Amenities = new[] { "WiFi", "Air Conditioning", "TV", "Minibar", "Gym Access", "Spa Access", "Premium Breakfast" },
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
        var confirmation = new BookingConfirmation
        {
            BookingReference = $"PS-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}",
            Provider = ProviderName,
            TotalPrice = 500m, // Stub value
            CancellationPolicy = "Free cancellation up to 7 days before",
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
            Destination = "London",
            CheckIn = DateTime.UtcNow.AddDays(5),
            CheckOut = DateTime.UtcNow.AddDays(10),
            RoomType = "Deluxe",
            Provider = ProviderName,
            TotalPrice = 750m
        };

        return Task.FromResult(status);
    }
}
