using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;
using ATS.Inspect.Demo.Login.Properties;
using ATS.Inspect.Shared.Security;
using ATS.Inspect.Shared.Security.Enums;
using ATS.Inspect.Shared.Security.Extensions;
using IdentityModel.Client;
using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;
using InspectLoginDemo.Browser;

namespace InspectLoginDemo;

public partial class FormMain : Form
{
   #region Enums

   public enum LoginMethod
   {
      Browser = 0,
      Password = 1,
      ExternalCode = 2
   }

   #endregion

   #region Variables

   private readonly OidcClient _oidcClient;

   private string _externalCode, _username, _password;

   //
   // Scope
   //

   private string _accessToken, _apiToken;

   private string _userId;

   #endregion

   #region Constructors

   public FormMain()
   {
      InitializeComponent();

      var options = new OidcClientOptions
         {
            Authority = SecurityManagerEndpoint,
            ClientId = Client.ClientId,
            ClientSecret = Client.FirstSecret,
            Scope = $"openid profile email offline_access {Client.GetScopes()} SecurityManager.SecuritySettingsScope",
            RedirectUri = "urn:ietf:wg:oauth:2.0:oob", // "oob://localhost/winforms.client",
            Browser = new WinFormsWebView2()
         };

      _oidcClient = new OidcClient(options);
   }

   #endregion

   #region Properties

   #region Private Properties

   private static string ApplicationId => Settings.Default.ApplicationId;

   private static Client Client => ApplicationClient.Configuration;

   private bool LoggedIn { get; set; }

   private LoginMethod LoginType
   {
      get
      {
         switch (_comboLoginMethod.SelectedIndex)
         {
            case 0:
               return LoginMethod.Browser;
            case 1:
               return LoginMethod.Password;
            default:
               return LoginMethod.ExternalCode;
         }
      }

      set => _comboLoginMethod.SelectedIndex = (int)value;
   }

   private static string SecurityManagerEndpoint => Settings.Default.SecurityManagerEndpoint.ToLowerInvariant();

   #endregion

   #endregion

   #region Event Handlers

   #region Button Events

   private async void _btnLogin_Click(object sender, EventArgs e)
   {
      try
      {
         switch (LoginType)
         {
            case LoginMethod.Browser:

               await LoginViaBrowserAsync();
               return;

            case LoginMethod.Password:

               await LoginViaPasswordAsync();
               return;

            case LoginMethod.ExternalCode:
            default:

               await LoginViaCodeAsync();
               return;
         }
      }
      catch (Exception ex)
      {
         MessageBox.Show(ex.Message);
      }
   }

   private async void _btnLogout_Click(object sender, EventArgs e)
   {
      if (! LoggedIn)
      {
         return;
      }

      await LogoutAsync();
   }

   private async void _btnGetUserInfo_Click(object sender, EventArgs e)
   {
      try
      {
         var client = new HttpClient();

         client.SetBearerToken(_accessToken);

         _memoUserInfo.Text =
            await client.GetStringAsync($"{SecurityManagerEndpoint}/api/userSettings/getUser/{_userId}");
      }
      catch (Exception ex)
      {
         MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
   }

   private async void _btnGetUserCustomFields_Click(object sender, EventArgs e)
   {
      try
      {
         var client = new HttpClient();

         client.SetBearerToken(_accessToken);

         // https://atsusnb191.ats-global.local:5000/api/userSettings/customUserFields?userId=46e373bd-0412-452a-83c5-1610432d9163&applicationId=1ba93dc2-1462-4005-0b0b-08da1baa2f1e%22%5C

         var url =
            $"{SecurityManagerEndpoint}/api/userSettings/customUserFields?userId={_userId}&applicationId={ApplicationId}";

         _memoCustomFields.Text = await client.GetStringAsync(url);
      }
      catch (Exception ex)
      {
         MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
   }

   private async void _btnGetTokenViaClientCredentials_Click(object sender, EventArgs e)
   {
      try
      {
         // Clear text box
         _textApiToken.Text = string.Empty;

         using var httpClient = new HttpClient();

         //
         // Discover endpoint
         //

         var discoClient = await httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
               Address = SecurityManagerEndpoint,
               Policy = { RequireHttps = true, ValidateIssuerName = false }
            });

         if (discoClient.IsError)
         {
            MessageBox.Show(discoClient.Error, @"Error obtaining authority server information");
            return;
         }

         //
         // Request PAT via Client Credentials
         //

         var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
               Address = discoClient.TokenEndpoint,
               ClientId = Client.ClientId,
               ClientSecret = Client.FirstSecret,
               Scope = $"{Client.GetScopes()} SecurityManager.SecuritySettingsScope"
            });

         _textApiToken.Text = tokenResponse.IsError ? tokenResponse.Error : tokenResponse.AccessToken;

         _apiToken = (! tokenResponse.IsError) ? tokenResponse.AccessToken : string.Empty;

         _textApiToken.Text = _apiToken;
      }
      catch (Exception ex)
      {
         Debug.WriteLine(ex);
      }
   }

   private void _btnEncode_Click(object sender, EventArgs e)
   {
      _textEncoded.Text = Sha256(_textUnencoded.Text);
   }

   private void _btnCopyToClipboard_Click(object sender, EventArgs e)
   {
      _textAccessToken.SelectionStart = 0;
      _textAccessToken.SelectionLength = _textAccessToken.Text.Length;

      if (_textAccessToken.SelectionLength > 0)
      {
         _textAccessToken.Copy();
      }
   }

   #endregion

   #endregion

   #region Methods

   #region Override Methods

   protected override void OnLoad(EventArgs e)
   {
      base.OnLoad(e);

      // Initialize settings
      _textSecMgrEndpoint.Text = Settings.Default.SecurityManagerEndpoint;
      _textApplicationId.Text = Settings.Default.ApplicationId;

      // Set default login type
      LoginType = (LoginMethod)Settings.Default.DefaultLoginType;
   }

   #endregion

   #region Private Methods

   private async Task LoginViaBrowserAsync()
   {
      if (LoggedIn)
      {
         return;
      }

      try
      {
         var result = await _oidcClient.LoginAsync();

         if (result.IsError)
         {
            if (result.Error != "UserCancel")
            {
               MessageBox.Show(this, result.Error, @"Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
         }
         else
         {
            _btnLogin.Text = @"Logout";

            _btnGetUserInfo.Enabled = true;
            _btnGetUserCustomFields.Enabled = true;

            foreach (var claim in result.User.Claims) _listboxClientInfo.Items.Add($"{claim.Type}: {claim.Value}");

            _userId = result.User.FindFirst("sub")?.Value;

            if (! string.IsNullOrWhiteSpace(result.RefreshToken))
            {
               _listboxClientInfo.Items.Add($"refresh token: {result.RefreshToken}");
            }

            if (! string.IsNullOrWhiteSpace(result.IdentityToken))
            {
               _listboxClientInfo.Items.Add($"identity token: {result.IdentityToken}");
            }

            if (! string.IsNullOrWhiteSpace(result.AccessToken))
            {
               _textAccessToken.Text = _accessToken = result.AccessToken;
            }

            _userId = result.User.FindFirst("sub")?.Value;
         }
      }
      catch (Exception ex)
      {
         MessageBox.Show(ex.Message);
      }
      finally
      {
         CheckControls();
      }
   }

   private async Task LoginViaCodeAsync()
   {
      if (LoggedIn)
      {
         return;
      }

      try
      {
         // Prompt for code
         using (var dlg = new DlgExternalCode())
         {
            dlg.ExternalCode = _externalCode;

            var result = dlg.ShowDialog();

            if (result != DialogResult.OK) return;

            _externalCode = dlg.ExternalCode;
         }

         using var httpClient = new HttpClient();

         //
         // Discover endpoint
         //

         var discoClient = await httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
               Address = SecurityManagerEndpoint,
               Policy = { RequireHttps = true, ValidateIssuerName = false }
            });

         if (discoClient.IsError)
         {
            MessageBox.Show(discoClient.Error, @"Error obtaining authority server information");
         }

         //
         // Request token via external code
         //

         var parameters = new Parameters();

         parameters.Add("id_code", _externalCode);

         var tokenRequest = new TokenRequest
            {
               GrantType = "external_code",
               Address = discoClient.TokenEndpoint,
               ClientId = Client.ClientId,
               ClientSecret = Client.FirstSecret,
               Parameters = parameters
            };

         var tokenResponse = await httpClient.RequestTokenAsync(tokenRequest);

         if (tokenResponse.IsError)
         {
            MessageBox.Show(this, tokenResponse.ErrorDescription, @"Login", MessageBoxButtons.OK,
               MessageBoxIcon.Error);
         }
         else
         {
            LoggedIn = true;

            _btnGetUserInfo.Enabled = true;
            _btnGetUserCustomFields.Enabled = true;

            Debug.WriteLine($"Access Token: {tokenResponse.AccessToken}");

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadToken(tokenResponse.AccessToken) as JwtSecurityToken;

            foreach (var claim in token!.Claims)
            {
               _listboxClientInfo.Items.Add($"{claim.Type}: {claim.Value}");
            }

            _userId = token.Claims.SingleOrDefault(x => x.Type == "sub")?.Value;

            if (! string.IsNullOrWhiteSpace(tokenResponse.AccessToken))
            {
               _textAccessToken.Text = _accessToken = tokenResponse.AccessToken;
            }
         }
      }
      catch (Exception ex)
      {
         MessageBox.Show(ex.Message);
      }
      finally
      {
         CheckControls();
      }
   }

   private async Task LoginViaPasswordAsync()
   {
      if (LoggedIn)
      {
         return;
      }

      try
      {
         // Prompt for code
         using (var dlg = new DlgUsernamePassword())
         {
            dlg.Username = _username;
            dlg.Password = _password;

            var result = dlg.ShowDialog();

            if (result != DialogResult.OK)
            {
               return;
            }

            _username = dlg.Username;
            _password = dlg.Password;
         }

         using var httpClient = new HttpClient();

         //
         // Discover endpoint
         //

         var discoClient = await httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
               Address = SecurityManagerEndpoint,
               Policy = { RequireHttps = true, ValidateIssuerName = false }
            });

         if (discoClient.IsError)
         {
            MessageBox.Show(discoClient.Error, @"Error obtaining authority server information");
         }

         var requestValues = new Dictionary<string, string>
            {
               { "token_endpoint", SecurityManagerEndpoint + Endpoints.SecurityManager.Token },
               { "grant_type", GrantTypes.Password },
               { "username", _username },
               { "password", _password },
               { "client_id", Client.ClientId },
               { "client_secret", Client.FirstSecret },
               { "scope", Client.GetScopes() + " SecurityManager.SecuritySettingsScope" },
               { "refresh_token", string.Empty },
               { "device_code", string.Empty }
            };

         //   var form = serializer.Deserialize<Dictionary<string, string>>(new JsonTextReader(new StringReader(requestValues)));

         httpClient.DefaultRequestHeaders.Accept.Clear();
         httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-US"));
         httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

         using var tokenResponse =
            await httpClient.PostAsync(requestValues["token_endpoint"],
               new FormUrlEncodedContent(requestValues));

         if (! tokenResponse.IsSuccessStatusCode)
         {
            var details = tokenResponse.Content.ReadAsStringAsync().Result;

            MessageBox.Show(
               $@"Authentication failed. Token response : {tokenResponse.ReasonPhrase}, details: {details}");
         }
         else
         {
            var jsonContent = await tokenResponse.Content.ReadFromJsonAsync<PasswordTokenResponse>();

            _btnLogin.Text = @"Logout";

            _btnGetUserInfo.Enabled = true;
            _btnGetUserCustomFields.Enabled = true;

            Debug.WriteLine($"Access Token: {jsonContent.AccessToken}");

            var stream = jsonContent.AccessToken;
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadToken(stream) as JwtSecurityToken;

            foreach (var claim in token!.Claims)
            {
               _listboxClientInfo.Items.Add($"{claim.Type}: {claim.Value}");
            }

            _userId = token.Claims.SingleOrDefault(x => x.Type == "sub")?.Value;

            if (! string.IsNullOrWhiteSpace(jsonContent.AccessToken))
            {
               _textAccessToken.Text = _accessToken = jsonContent.AccessToken;
            }
         }
      }
      catch (Exception ex)
      {
         MessageBox.Show(ex.Message);
      }
      finally
      {
         CheckControls();
      }
   }

   private async Task LogoutAsync()
   {
      try
      {
         var logoutRequest = new LogoutRequest { BrowserDisplayMode = DisplayMode.Hidden };

         await _oidcClient.LogoutAsync(logoutRequest);

         _accessToken = string.Empty;

         _btnLogin.Text = @"Login";

         // Clear data
         _listboxClientInfo.Items.Clear();
         _memoUserInfo.Text = _memoCustomFields.Text = string.Empty;

         _btnGetUserInfo.Enabled = false;
         _btnGetUserCustomFields.Enabled = false;
      }
      catch (Exception ex)
      {
         MessageBox.Show(ex.Message);
      }
      finally
      {
         LoggedIn = false;

         CheckControls();
      }
   }

   private void CheckControls()
   {
      _btnLogin.Enabled = ! LoggedIn;
      _btnLogout.Enabled = LoggedIn;
   }

   private static string Sha256(string input)
   {
      if (string.IsNullOrWhiteSpace(input)) return string.Empty;

      using var sha = SHA256.Create();

      var bytes = Encoding.UTF8.GetBytes(input);
      var hash = sha.ComputeHash(bytes);

      return Convert.ToBase64String(hash);
   }

   #endregion

   #endregion

   public record PasswordTokenResponse
   {
      #region Properties

      #region Public Properties

      [JsonPropertyName("access_token")] public string AccessToken { get; set; }

      [JsonPropertyName("expires_in")] public int ExpiresIn { get; set; }

      [JsonPropertyName("token_type")] public string TokenType { get; set; }

      [JsonPropertyName("scope")] public string Scope { get; set; }

      #endregion

      #endregion
   }
}