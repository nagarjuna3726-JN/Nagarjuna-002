namespace HotelStay.Api.Providers;

using HotelStay.Api.Models;

/// <summary>
/// Abstracts hotel provider integration for search and booking.
/// </summary>
public interface IHotelProvider
{
    string ProviderName { get; }

    /// <summary>
    /// Searches for available hotels.
    /// </summary>
    Task<IEnumerable<HotelSearchResult>> SearchAsync(
        string destination,
        DateTime checkIn,
        DateTime checkOut,
        string? roomType = null);

    /// <summary>
    /// Books a hotel room.
    /// </summary>
    Task<BookingConfirmation> BookAsync(BookingRequest request);

    /// <summary>
    /// Retrieves booking status.
    /// </summary>
    Task<BookingStatus> GetBookingStatusAsync(string bookingReference);
}
