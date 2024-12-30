using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

var stationCode = "MPI1"; // <-- REPLACE WITH YOUR STATION CODE
var identifierTypeCode = "IDENTITY-1"; // THIS SHOULD BE THE 'IDENTIFIER TYPE'
var identifier = "4102"; // THIS SHOULD BE YOUR IDENTIFIER

//
// Setup HTTP client
//

using var client = new HttpClient();

client.BaseAddress = new Uri("https://atsusnb191.ats-global.local:8500");

try
{
   //
   // Get access token
   //

   const string pat = "M2ZlOGY0MTAtMDYwYS00MGZiLTdiODItMDhkY2E1YWY1YmQy";

   var url = $"/api/internal/authentication/pat/{pat}";

   using var tokenResponse = await client.GetAsync(url);

   if (! tokenResponse.IsSuccessStatusCode)
   {
      // TODO: Handle error...
      return;
   }

   var accessToken = await tokenResponse.Content.ReadAsStringAsync();

   // Now that we have token, add authorization header
   client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

   //
   // Get station info
   //

   url = $"/api/internal/collector/stations/{stationCode}/data-collect";

   using var stationResponse = await client.GetAsync(url);

   if (! stationResponse.IsSuccessStatusCode)
   {
      // TODO: Handle error here...
      return;
   }

   var stationContent = await stationResponse.Content.ReadFromJsonAsync<CollectorResponse>();

   //
   // Get unit id for identifier
   //

   url = $"/api/v1/units/{identifierTypeCode}/{identifier}/unit-id";

   using var unitResponse = await client.GetAsync(url);

   if (! unitResponse.IsSuccessStatusCode)
   {
      // TODO: Handle error here...
      return;
   }

   var unitContent = await unitResponse.Content.ReadFromJsonAsync<UnitIdResponse>();

   var unitId = unitContent!.UnitId;

   //
   // Get defect statistics (so we can check for open defects)
   //

   url = $"/api/v1/units/{unitId}/defect-statistics";

   using var defectResponse = await client.GetAsync(url);

   if (! defectResponse.IsSuccessStatusCode)
   {
      // TODO: Handle error here...
      return;
   }

   var defectContent = await defectResponse.Content.ReadFromJsonAsync<DefectStatisticsResponse>();

   //
   // Print travel ticket?
   //

   if (defectContent.NumOpenDefects > 0)
   {
      url = $"/api/v1/collector/travel-service/print-ticket";

      // Shorthand
      var properties = stationContent.Properties;

      var request = new PrintTicketRequest
         {
            TravelPrintServiceId = Convert.ToInt32(properties.TravelServiceId.Value),
            PrinterName = properties.TravelServiceDefaultPrinter.Value,
            Qty = 1,
            StationId = stationContent.Id,
            TicketTypeId = Convert.ToInt32(properties.TravelTicketType.Value),
            UnitId = unitContent.UnitId,
            Username = "inspect",
            LanguageId = 1033 // U.S. English
         };

      var jsonBody = JsonSerializer.Serialize(request);

      HttpContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

      using var ticketResponse = await client.PostAsync(url, content);

      if (ticketResponse.IsSuccessStatusCode)
      {
         Console.WriteLine("Print request processed!");
      }
   }
}
catch (Exception ex)
{
   Console.WriteLine(ex);
}

#region Request/Response DTOs

#region Station

internal record CollectorResponse
{
   #region Properties

   #region Public Properties

   [JsonPropertyName("id")]
   public int Id { get; set; }

   [JsonPropertyName("code")]
   public string Code { get; set; }
   
   [JsonPropertyName("description")]
   public string Description { get; set; }
   
   [JsonPropertyName("properties")]
   public Properties Properties { get; set; }
   
   [JsonPropertyName("isActive")]
   public bool IsActive { get; set; }
   
   [JsonPropertyName("setupId")]
   public int SetupId { get; set; }

   #endregion

   #endregion
}

public class Properties
{
   public PropertyValue TravelServiceId { get; set; }
   public PropertyValue TravelServiceDefaultPrinter { get; set; }
   public PropertyValue TravelServiceChecklistMode { get; set; }
   public PropertyValue TrackingPointId { get; set; }
   public PropertyValue TravelTicketType { get; set; }
}

public class PropertyValue
{
   [JsonPropertyName("id")]
   public string Id { get; set; }
   [JsonPropertyName("type")]
   public string Type { get; set; }
   [JsonPropertyName("value")]
   public string Value { get; set; }
}

#endregion

public record PrintTicketRequest
{
   [JsonPropertyName("travelPrintServiceId")]
   public int TravelPrintServiceId { get; set; }
   [JsonPropertyName("printerName")]
   public string PrinterName { get; set; }
   [JsonPropertyName("printGroupId")]
   public object PrintGroupId { get; set; }
   [JsonPropertyName("qty")]
   public int Qty { get; set; }
   [JsonPropertyName("stationId")]
   public int StationId { get; set; }
   [JsonPropertyName("ticketTypeId")]
   public int TicketTypeId { get; set; }
   [JsonPropertyName("unitId")]
   public int UnitId { get; set; }
   [JsonPropertyName("username")]
   public string Username { get; set; }
   [JsonPropertyName("languageId")]
   public int LanguageId { get; set; }
}

public record DefectStatisticsResponse
{
   [JsonPropertyName("numDefects")]
   public int NumDefects { get; set; }
   [JsonPropertyName("numOpenDefects")]
   public int NumOpenDefects { get; set; }
   [JsonPropertyName("numRepairedDefects")]
   public int NumRepairDefects { get; set; }
   [JsonPropertyName("numConfirmedDefects")]
   public int NumConfirmedDefects { get; set; }
}

public record IdResponse
{
   #region Constructors

   public IdResponse(int id = default)
   {
      Id = id;
   }

   public IdResponse()
   {
      // Empty on purpose
   }

   #endregion

   #region Properties

   #region Public Properties

   [JsonPropertyName("id")]
   public int Id { get; set; }

   #endregion

   #endregion
}

public record UnitIdResponse
{
   #region Constructors

   public UnitIdResponse(int id = default)
   {
      UnitId = id;
   }

   public UnitIdResponse()
   {
      // Empty on purpose
   }

   #endregion

   #region Properties

   #region Public Properties

   [JsonPropertyName("unitId")]
   public int UnitId { get; set; }

   #endregion

   #endregion
}

#endregion