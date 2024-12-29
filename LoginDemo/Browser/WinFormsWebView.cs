// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using IdentityModel.OidcClient.Browser;
using Microsoft.Web.WebView2.WinForms;

namespace InspectLoginDemo.Browser;

//public class WinFormsWebView : IBrowser
//{
//   private readonly Func<Form> _formFactory;

//   private WinFormsWebView(Func<Form> formFactory)
//   {
//      _formFactory = formFactory;
//   }

//   public WinFormsWebView(string title = "Authenticating...", int width = 1024, int height = 768)
//      : this(() => new Form
//         {
//            Name = "WebAuthentication",
//            Text = title,
//            Width = width,
//            Height = height
//         })
//   {
//   }

//   public async Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken token = default)
//   {
//      using (var form = _formFactory.Invoke())
//      using (var browser = new ExtendedWebBrowser()
//                {
//                   Dock = DockStyle.Fill
//                })
//      {
//         var signal = new SemaphoreSlim(0, 1);

//         var result = new BrowserResult
//            {
//               ResultType = BrowserResultType.UserCancel
//            };

//         form.FormClosed += (_, _) => { signal.Release(); };

//         browser.NavigateError += (_, e) =>
//            {
//               if (! e.Url.StartsWith(options.EndUrl))
//               {
//                  return;
//               }

//               e.Cancel = true;
//               result.ResultType = BrowserResultType.Success;
//               result.Response = e.Url;
//               signal.Release();
//            };

//         browser.DocumentCompleted += (_, e) =>
//            {
//               if (! (e.Url?.AbsoluteUri.StartsWith(options.EndUrl) ?? false))
//               {
//                  return;
//               }

//               result.ResultType = BrowserResultType.Success;
//               result.Response = e.Url.AbsoluteUri;
//               signal.Release();
//            };

//         try
//         {
//            form.Controls.Add(browser);
//            browser.Show();

//            form.Show();
//            browser.Navigate(options.StartUrl);

//            await signal.WaitAsync(token);
//         }
//         finally
//         {
//            form.Hide();
//            browser.Hide();
//         }

//         return result;
//      }
//   }
//}

public class WinFormsWebView2 : IBrowser
{
   #region Variables

   private readonly Func<Form> _formFactory;
   private BrowserOptions _options;

   #endregion

   #region Constructors

   public WinFormsWebView2(Func<Form> formFactory)
   {
      _formFactory = formFactory;
   }

   public WinFormsWebView2(string title = "Authenticating ...", int width = 1024, int height = 768)
      : this(() => new Form
         {
            Name = "WebAuthentication",
            Text = title,
            Width = width,
            Height = height
         })
   {
   }

   #endregion

   #region Methods

   #region Implements Interface

   public async Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken token = default)
   {
      _options = options;

      var browserResult = new BrowserResult
         {
            ResultType = BrowserResultType.UserCancel
         };

      if (options.DisplayMode == DisplayMode.Hidden)
      {
         return browserResult;
      }

      using var form = _formFactory.Invoke();

      using var webView = new WebView2();

      webView.Dock = DockStyle.Fill;

      var signal = new SemaphoreSlim(0, 1);

      form.FormClosed += (_, _) => { signal.Release(); };

      webView.NavigationStarting += (_, e) =>
      {
         if (! IsBrowserNavigatingToRedirectUri(new Uri(e.Uri)))
         {
            return;
         }

         e.Cancel = true;

         browserResult = new BrowserResult
         {
            ResultType = BrowserResultType.Success,
            Response = new Uri(e.Uri).AbsoluteUri
         };

         signal.Release();

         form.Close();
      };

      try
      {
         form.Controls.Add(webView);

         webView.Show();

         form.Show();

         // Initialization
         await webView.EnsureCoreWebView2Async(null);

         // Delete existing Cookies so previous logins won't remembered
         webView.CoreWebView2.CookieManager.DeleteAllCookies();

         // Navigate
         webView.CoreWebView2.Navigate(_options.StartUrl);

         await signal.WaitAsync(token);
      }
      finally
      {
         form.Hide();
         webView.Hide();
      }

      return browserResult;
   }

   #endregion

   #region Private Methods

   private bool IsBrowserNavigatingToRedirectUri(Uri uri)
   {
      return uri.AbsoluteUri.StartsWith(_options?.EndUrl!);
   }

   #endregion

   #endregion
}