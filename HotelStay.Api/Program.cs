using HotelStay.Api.Models;
using HotelStay.Api.Providers;
using HotelStay.Api.Services;

var builder = WebApplicationBuilder.CreateBuilder(args);

// Add services to the container
builder.Services.AddScoped<IHotelProvider, PremierStaysProvider>();
builder.Services.AddScoped<IHotelProvider, BudgetNestsProvider>();
builder.Services.AddScoped<IHotelProvider, BoutiqueCollectionProvider>();
builder.Services.AddScoped<HotelAggregationService>();
builder.Services.AddScoped<BookingValidationService>();

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var app = builder.CreateSlateBuilder();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ==================== ENDPOINTS ====================

var hotelGroup = app.MapGroup("/hotels")
    .WithName("Hotels")
    .WithOpenApi();

// GET /hotels/search
hotelGroup.MapGet("/search", SearchHotels)
    .WithName("SearchHotels")
    .WithDescription("Search for available hotels from multiple providers")
    .WithOpenApi()
    .Produces<IEnumerable<HotelSearchResult>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest);

// POST /hotels/book
hotelGroup.MapPost("/book", BookHotel)
    .WithName("BookHotel")
    .WithDescription("Book a hotel with passenger and document validation")
    .WithOpenApi()
    .Produces<BookingConfirmation>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status422UnprocessableEntity);

// GET /hotels/booking/{reference}
hotelGroup.MapGet("/booking/{reference}", GetBookingStatus)
    .WithName("GetBookingStatus")
    .WithDescription("Get the status of a booking")
    .WithOpenApi()
    .Produces<BookingStatus>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound);

app.Run();

// ==================== ROUTE HANDLERS ====================

async Task<IResult> SearchHotels(
    string? destination,
    string? checkIn,
    string? checkOut,
    string? roomType,
    HotelAggregationService aggregationService,
    BookingValidationService validationService)
{
    // Validate input
    if (string.IsNullOrWhiteSpace(destination))
    {
        return Results.BadRequest(new { error = "destination query parameter is required" });
    }

    if (!DateTime.TryParse(checkIn, out var checkInDate))
    {
        return Results.BadRequest(new { error = "checkIn must be a valid date (YYYY-MM-DD)" });
    }

    if (!DateTime.TryParse(checkOut, out var checkOutDate))
    {
        return Results.BadRequest(new { error = "checkOut must be a valid date (YYYY-MM-DD)" });
    }

    var (isValid, errorMessage) = validationService.ValidateSearchParams(destination, checkInDate, checkOutDate, roomType);
    if (!isValid)
    {
        return Results.BadRequest(new { error = errorMessage });
    }

    try
    {
        var results = await aggregationService.SearchAllProvidersAsync(destination, checkInDate, checkOutDate, roomType);
        return Results.Ok(results);
    }
    catch (Exception ex)
    {
        return Results.StatusCode(StatusCodes.Status500InternalServerError);
    }
}

async Task<IResult> BookHotel(
    BookingRequest request,
    IEnumerable<IHotelProvider> providers,
    BookingValidationService validationService)
{
    // Validate booking request including document validation
    var (isValid, statusCode, errorMessage) = validationService.ValidateBookingRequest(
        request.Destination,
        request.PassengerName,
        request.DocumentType,
        request.DocumentNumber);

    if (!isValid)
    {
        return statusCode == 422
            ? Results.UnprocessableEntity(new { error = errorMessage })
            : Results.BadRequest(new { error = errorMessage });
    }

    // Validate dates
    if (request.CheckOut <= request.CheckIn)
    {
        return Results.UnprocessableEntity(new { error = "CheckOut must be after CheckIn" });
    }

    // Find the provider
    var provider = providers.FirstOrDefault(p => p.ProviderName.Equals(request.HotelProvider, StringComparison.OrdinalIgnoreCase));
    if (provider == null)
    {
        return Results.BadRequest(new { error = $"Provider '{request.HotelProvider}' not found" });
    }

    try
    {
        var confirmation = await provider.BookAsync(request);
        return Results.Ok(confirmation);
    }
    catch (Exception ex)
    {
        return Results.UnprocessableEntity(new { error = ex.Message });
    }
}

async Task<IResult> GetBookingStatus(
    string reference,
    IEnumerable<IHotelProvider> providers)
{
    if (string.IsNullOrWhiteSpace(reference))
    {
        return Results.BadRequest(new { error = "Booking reference is required" });
    }

    // Determine provider from booking reference prefix
    var prefix = reference.Split('-').FirstOrDefault();
    IHotelProvider? provider = prefix switch
    {
        "PS" => providers.FirstOrDefault(p => p.ProviderName == "PremierStays"),
        "BN" => providers.FirstOrDefault(p => p.ProviderName == "BudgetNests"),
        "BC" => providers.FirstOrDefault(p => p.ProviderName == "BoutiqueCollection"),
        _ => null
    };

    if (provider == null)
    {
        return Results.NotFound(new { error = "Booking not found" });
    }

    try
    {
        var status = await provider.GetBookingStatusAsync(reference);
        return Results.Ok(status);
    }
    catch (Exception)
    {
        return Results.NotFound(new { error = "Booking not found" });
    }
}
