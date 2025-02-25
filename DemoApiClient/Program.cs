using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable UnusedMember.Local
// ReSharper disable ClassNeverInstantiated.Local

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

//
// Data Service endpoint
//

const string dataServiceEndpoint = "https://atsusnb191.ats-global.local:8500";   // Adjust this value to match your local environment

//
// Security info
//

// You need to have a user set up in Security Manager, assign rights, and create a personal access token (PAT). For
// more info, see here: https://ats-help.com/security-configuration/#t=ATS_Configuration%2FSystem_Configuration%2FPersonal_Access_Tokens.htm&rhsearch=access%20token&rhhlterm=access%20tokens%20token
const string personalAccessToken = "M2ZlOGY0MTAtMDYwYS00MGZiLTdiODItMDhkY2E1YWY1YmQy";

//
// API arguments
//

var stationCode = "MPI1"; // <-- REPLACE WITH YOUR STATION CODE (stored in the 'stations' table')
var identifierTypeCode = "ROTATION"; // THIS SHOULD BE THE 'IDENTIFIER TYPE' (stored in the 'unit_identifier_types' table)
var identifier = "4102"; // THIS SHOULD BE YOUR IDENTIFIER (stored in the 'unit_identifiers' table)

//
// Setup HTTP client
//

using var client = new HttpClient();

client.BaseAddress = new Uri(dataServiceEndpoint);

try
{
   //
   // Get access token
   //

   var url = $"/api/internal/authentication/pat/{personalAccessToken}";

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

   if (defectContent?.NumOpenDefects > 0)
   {
      url = $"/api/v1/collector/travel-service/print-ticket";

      // Shorthand
      var properties = stationContent!.Properties;

      var request = new PrintTicketRequest
         {
            TravelPrintServiceId = Convert.ToInt32(properties.TravelServiceId.Value),
            PrinterName = properties.TravelServiceDefaultPrinter.Value,
            Qty = 1,
            StationId = stationContent.Id,
            TicketTypeId = Convert.ToInt32(properties.TravelServiceTicketTypeId.Value),
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

file record CollectorResponse
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

file record Properties
{
   public PropertyValue TravelServiceId { get; set; }
   public PropertyValue TravelServiceDefaultPrinter { get; set; }
   public PropertyValue TravelServiceChecklistMode { get; set; }
   public PropertyValue TravelServiceTicketTypeId { get; set; }
   public PropertyValue TrackingPointId { get; set; }
}

file record PropertyValue
{
   [JsonPropertyName("id")]
   public string Id { get; set; }
   [JsonPropertyName("type")]
   public string Type { get; set; }
   [JsonPropertyName("value")]
   public string Value { get; set; }
}

#endregion

file record PrintTicketRequest
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

file record DefectStatisticsResponse
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

file record UnitIdResponse
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