namespace HotelStay.Api.Models;

/// <summary>
/// Represents a normalized hotel room offering across providers.
/// </summary>
public class HotelSearchResult
{
    public string RoomType { get; set; } = string.Empty; // Standard, Deluxe, Suite
    public decimal PerNightRate { get; set; }
    public decimal TotalPrice { get; set; }
    public string CancellationPolicy { get; set; } = string.Empty;
    public string ProviderName { get; set; } = string.Empty;
    public string[]? Amenities { get; set; }
    public int? StarRating { get; set; }
    public int NumberOfNights { get; set; }
}

/// <summary>
/// Booking request model with passenger and document information.
/// </summary>
public class BookingRequest
{
    public string Destination { get; set; } = string.Empty;
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public string RoomType { get; set; } = string.Empty;
    public string HotelProvider { get; set; } = string.Empty;
    public string PassengerName { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty; // Passport or National ID
    public string DocumentNumber { get; set; } = string.Empty;
}

/// <summary>
/// Booking confirmation response.
/// </summary>
public class BookingConfirmation
{
    public string BookingReference { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
    public string CancellationPolicy { get; set; } = string.Empty;
    public string Status { get; set; } = "Confirmed";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Booking status response.
/// </summary>
public class BookingStatus
{
    public string BookingReference { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public string RoomType { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
}
