namespace HotelStay.Api.Services;

/// <summary>
/// Validates hotel booking requests.
/// </summary>
public class BookingValidationService
{
    // Domestic destinations
    private static readonly HashSet<string> DomesticDestinations = new()
    {
        "London", "Manchester", "Birmingham", "Leeds"
    };

    // International destinations
    private static readonly HashSet<string> InternationalDestinations = new()
    {
        "Paris", "Barcelona", "Amsterdam", "Berlin", "Rome"
    };

    /// <summary>
    /// Validates search parameters.
    /// </summary>
    public (bool IsValid, string? ErrorMessage) ValidateSearchParams(
        string destination,
        DateTime checkIn,
        DateTime checkOut,
        string? roomType = null)
    {
        if (string.IsNullOrWhiteSpace(destination))
        {
            return (false, "Destination is required.");
        }

        if (checkOut <= checkIn)
        {
            return (false, "CheckOut date must be after CheckIn date.");
        }

        return (true, null);
    }

    /// <summary>
    /// Validates booking request including document requirements.
    /// Returns (IsValid, StatusCode, ErrorMessage).
    /// </summary>
    public (bool IsValid, int StatusCode, string? ErrorMessage) ValidateBookingRequest(
        string destination,
        string passengerName,
        string documentType,
        string documentNumber)
    {
        if (string.IsNullOrWhiteSpace(destination))
        {
            return (false, 422, "Destination is required.");
        }

        if (string.IsNullOrWhiteSpace(passengerName))
        {
            return (false, 422, "Passenger name is required.");
        }

        if (string.IsNullOrWhiteSpace(documentType))
        {
            return (false, 422, "Document type is required.");
        }

        if (string.IsNullOrWhiteSpace(documentNumber))
        {
            return (false, 422, "Document number is required.");
        }

        var isInternational = IsInternationalDestination(destination);
        var isDomestic = IsDomesticDestination(destination);

        if (!isInternational && !isDomestic)
        {
            return (false, 422, "Destination is not recognized.");
        }

        // Document validation rules
        if (isInternational && !documentType.Equals("Passport", StringComparison.OrdinalIgnoreCase))
        {
            return (false, 422, "Passport is required for international destinations.");
        }

        if (isDomestic && !documentType.Equals("National ID", StringComparison.OrdinalIgnoreCase) &&
            !documentType.Equals("Passport", StringComparison.OrdinalIgnoreCase))
        {
            return (false, 422, "National ID or Passport is required for domestic destinations.");
        }

        // Validate document number format (basic check)
        if (string.IsNullOrWhiteSpace(documentNumber) || documentNumber.Length < 5)
        {
            return (false, 422, "Document number format is invalid.");
        }

        return (true, 200, null);
    }

    public bool IsInternationalDestination(string destination)
    {
        return InternationalDestinations.Contains(destination, StringComparer.OrdinalIgnoreCase);
    }

    public bool IsDomesticDestination(string destination)
    {
        return DomesticDestinations.Contains(destination, StringComparer.OrdinalIgnoreCase);
    }

    public IEnumerable<string> GetDomesticDestinations() => DomesticDestinations;

    public IEnumerable<string> GetInternationalDestinations() => InternationalDestinations;
}
