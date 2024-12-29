using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

var builder = WebApplication.CreateBuilder(args);

//
// Start as service
//

builder.Host.UseWindowsService();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
   app.UseSwagger();
   app.UseSwaggerUI();
}
//
// Map two endpoints: the "GET" is to handle the 'challenge' message from Data Service
// when you register a webhook. The "POST" is to receive actual webhook events.
//

app.MapGet("/webhook-receive", Helpers.HandleWebhookChallenge)
   .WithName("Webhook-Receive-Challenge")
   .WithSummary("Process challenge request from Data Service")
   .WithTags("Receive")
   .WithOpenApi();

app.MapPost("/webhook-receive", Helpers.HandleWebhookEvent)
   .WithName("Webhook-Receive")
   .WithSummary("Receive webhook event from Data Service")
   .WithTags("Receive")
   .WithOpenApi();

app.Run(); // Swagger can be found by entering: http://[machine_name]:[port]/swagger in your browser.

#region Methods

public static class Helpers
{
   #region Variables

   //
   // Standard definitions
   //

   private const string _webhookSecret = "None of your business!";
   private const string _webhookSignatureHeader = "X-Webhook-Signature";

   #endregion

   #region Methods

   #region Public Methods

   public static Ok<string> HandleWebhookChallenge(string challenge)
   {
      return TypedResults.Ok(challenge);
   }

   public static Results<Ok<string>, BadRequest<string>> HandleWebhookEvent(HttpRequest httpRequest,
      WebHookEventRequest request)
   {
      // Build hash of request object
      var shaHash = CreateHmac(request.Data, _webhookSecret);

      // Extract signature header
      var signatureHash = string.Empty;

      if (httpRequest.Headers.TryGetValue(_webhookSignatureHeader, out var signatureHeader))
      {
         signatureHash = signatureHeader.FirstOrDefault();
      }

      if (shaHash == signatureHash)
      {
         Debug.WriteLine($"Webhook event received: Type: {request.EventType}, Timestamp: {request.Timestamp}.");

         switch (request.EventType)
         {
            //
            // Defect-related
            //
           
            case "defect-added":
            case "defect-modified":
            {
               var defectEvent = JsonSerializer.Deserialize<WebhookDefectEventDto>(request.Data);

               break;
            }

            case "defect-removed":
            {
               var defectEvent = JsonSerializer.Deserialize<WebhookDefectsRemovedEventDto>(request.Data);

               break;
            }

            //
            // Unit Checklist Questions-related
            //

            case "unit-question-answered":
            case "unit-question-comments-modified":
            case "unit-question-images-modified":
            case "unit-question-reset":

               var questionEvent = JsonSerializer.Deserialize<WebhookUnitQuestionEventDto>(request.Data);

               break;

            default:

               break;
         }

         return TypedResults.Ok("Webhook event received.");
      }

      Debug.WriteLine($"Webhook event received but hash value incorrect.");

      return TypedResults.BadRequest("Hash does not meet expected value.");
   }

   #endregion

   #region Private Methods

   /// <summary>
   /// Create a hash of the response body using the secret as a salt value.
   /// </summary>
   private static string CreateHmac(string message, string secret)
   {
      var encoding = new UTF8Encoding();
      var keyBytes = encoding.GetBytes(secret);
      var messageBytes = encoding.GetBytes(message);

      using var hmacSHA1 = new HMACSHA1(keyBytes);

      var hashMessage = hmacSHA1.ComputeHash(messageBytes);

      return Convert.ToBase64String(hashMessage);
   }

   #endregion

   #endregion
}

#endregion

#region Classes

/// <summary>
/// This is the format of the data that you will receive for a webhook event.
/// The JSON data you want is in the "Data" property.
/// </summary>

public record WebHookEventRequest
{
   public long WebHookId { get; init; }

   public string EventType { get; init; }

   public string Data { get; init; }

   public DateTime Timestamp { get; init; }
}

#region Webhook Events

// The following classes will be the 'Data' property in the 'WebHookEventRequest'
// that you will receive from Data Service.

/// <summary>
/// For 'DefectAdded' and 'DefectModified' events
/// </summary>
public record WebhookDefectEventDto
{
   #region Properties

   #region Public Properties

   public DateTime Timestamp { get; set; } = DateTime.Now;

   public int DefectId { get; set; }

   public int UnitId { get; set; }

   public int InspectId { get; set; }

   public int StationId { get; set; }

   public string? Username { get; set; }

   #endregion

   #endregion
}

/// <summary>
/// For 'DefectRemoved' events
/// </summary>
public record WebhookDefectsRemovedEventDto
{
   #region Constructors

   public WebhookDefectsRemovedEventDto(List<int> defectIds)
   {
      DefectIds = defectIds;
   }

   public WebhookDefectsRemovedEventDto()
   {
      // Empty on purpose
   }

   #endregion

   #region Properties

   #region Public Properties

   public List<int> DefectIds { get; set; } = new();

   public DateTime Timestamp { get; set; } = DateTime.Now;

   #endregion

   #endregion
}

/// <summary>
/// For 'UnitQuestionAnswered', 'UnitQuestionCommentsModified', 'UnitQuestionImagesModified' and 'UnitQuestionReset' events
/// </summary>
public record WebhookUnitQuestionEventDto
{
   #region Properties

   #region Public Properties

   public int UnitQuestionId { get; set; }

   public int UnitId { get; set; }

   public int QuestionId { get; set; }

   public string QuestionCode { get; set; }

   public int? StationId { get; set; }

   public string? StationCode { get; set; }

   public int InspectId { get; set; }

   public string? Value { get; set; }

   public DateTime Timestamp { get; set; }

   #endregion

   #endregion
}

#endregion

#endregion

#region Enums

/// <summary>
/// The defined webhook events that Inspect Data Service supports.
/// </summary>
/// <remarks>This can be obtained via Data Service Swagger.</remarks>
public enum WebhookEventType
{
   //
   // Defects
   //

   DefectAdded = 1,

   DefectModified = 2,

   DefectRemoved = 3,

   //
   // Unit Checklist Questions
   //

   UnitQuestionAnswered = 4,

   UnitQuestionCommentsModified = 5,

   UnitQuestionImagesModified = 6,

   UnitQuestionReset = 7
}

#endregion