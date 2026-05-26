namespace HotelStay.Api.Models;

/// <summary>
/// Represents PremierStays API response (PascalCase JSON).
/// </summary>
public class PremierStaysRoom
{
    public string RoomType { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public string Cancellation { get; set; } = string.Empty;
    public string[] Facilities { get; set; } = [];
    public int Rating { get; set; }
}

/// <summary>
/// Represents BudgetNests API response (snake_case JSON).
/// </summary>
public class BudgetNestsRoom
{
    public string room_type { get; set; } = string.Empty;
    public decimal price_per_night { get; set; }
    public bool is_available { get; set; }
    public string cancellation_policy { get; set; } = string.Empty;
    public string[] amenities { get; set; } = [];
}

/// <summary>
/// Represents BoutiqueCollection API response.
/// </summary>
public class BoutiqueRoom
{
    public string RoomClass { get; set; } = string.Empty; // Deluxe, Suite
    public decimal NightlyRate { get; set; }
    public decimal BoutiqueFee { get; set; }
    public string CancellationTerms { get; set; } = string.Empty;
    public string[] Amenities { get; set; } = [];
}
