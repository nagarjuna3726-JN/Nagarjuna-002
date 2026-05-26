namespace HotelStay.Api.Providers;

using HotelStay.Api.Models;

/// <summary>
/// BudgetNests provider with deterministic stubbed responses (snake_case JSON).
/// </summary>
public class BudgetNestsProvider : IHotelProvider
{
    public string ProviderName => "BudgetNests";

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
                PerNightRate = 75m,
                TotalPrice = 75m * nights,
                CancellationPolicy = "Non-refundable",
                ProviderName = ProviderName,
                Amenities = new[] { "WiFi", "Basic Breakfast" },
                StarRating = 3,
                NumberOfNights = nights
            },
            new()
            {
                RoomType = "Deluxe",
                PerNightRate = 120m,
                TotalPrice = 120m * nights,
                CancellationPolicy = "Free cancellation up to 3 days before",
                ProviderName = ProviderName,
                Amenities = new[] { "WiFi", "Air Conditioning", "Breakfast", "Fitness Center" },
                StarRating = 3,
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
            BookingReference = $"BN-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}",
            Provider = ProviderName,
            TotalPrice = 400m, // Stub value
            CancellationPolicy = "Non-refundable",
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
            Destination = "Paris",
            CheckIn = DateTime.UtcNow.AddDays(3),
            CheckOut = DateTime.UtcNow.AddDays(7),
            RoomType = "Standard",
            Provider = ProviderName,
            TotalPrice = 300m
        };

        return Task.FromResult(status);
    }
}
