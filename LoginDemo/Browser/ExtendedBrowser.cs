// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace InspectLoginDemo.Browser;

internal class ExtendedWebBrowser : WebBrowser
{
   #region Variables

   private AxHost.ConnectionPointCookie _cookie;
   private ExtendedWebBrowserEventHelper _helper;

   #endregion

   #region Events

   internal event EventHandler<BeforeNavigate2EventArgs> BeforeNavigate2;

   internal event EventHandler<NavigateErrorEventArgs> NavigateError;

   #endregion

   #region Methods

   #region Override Methods

   protected override void CreateSink()
   {
      base.CreateSink();

      _helper = new ExtendedWebBrowserEventHelper(this);
      _cookie = new AxHost.ConnectionPointCookie(ActiveXInstance, _helper, typeof(DWebBrowserEvents2));
   }

   protected override void DetachSink()
   {
      if (_cookie != null)
      {
         _cookie.Disconnect();
         _cookie = null;
      }

      base.DetachSink();
   }

   #endregion

   #region Private Methods

   private void OnBeforeNavigate2(object pDisp, ref object url, ref object flags, ref object targetFrameName, ref object postData, ref object headers, ref bool cancel)
   {
      var handler = BeforeNavigate2;

      if (handler != null)
      {
         var args = new BeforeNavigate2EventArgs((string)url, (string)targetFrameName, (byte[])postData, (string)headers);

         handler(this, args);

         cancel = args.Cancel;
      }
   }

   private void OnNavigateError(object pDisp, ref object url, ref object frame, ref object statusCode, ref bool cancel)
   {
      var handler = NavigateError;

      if (handler != null)
      {
         var args = new NavigateErrorEventArgs((string)url, (string)frame, (int)statusCode);

         handler(this, args);

         cancel = args.Cancel;
      }
   }

   #endregion

   #endregion

   internal class BeforeNavigate2EventArgs : EventArgs
   {
      #region Variables

      #endregion

      #region Constructors

      public BeforeNavigate2EventArgs(string url, string targetFrameName, byte[] postData, string headers)
      {
         Url = url;
         TargetFrameName = targetFrameName;
         PostData = postData;
         Headers = headers;
         Cancel = false;
      }

      #endregion

      #region Properties

      #region Public Properties

      public string Url { get; }

      public string TargetFrameName { get; }

      public byte[] PostData { get; }

      public string Headers { get; }

      public bool Cancel { get; }

      #endregion

      #endregion
   }

   internal class NavigateErrorEventArgs : EventArgs
   {
      #region Variables

      #endregion

      #region Constructors

      public NavigateErrorEventArgs(string url, string frame, int statusCode)
      {
         Url = url;
         Frame = frame;
         StatusCode = statusCode;
         Cancel = false;
      }

      #endregion

      #region Properties

      #region Public Properties

      public string Url { get; }

      public string Frame { get; }

      public int StatusCode { get; }

      public bool Cancel { get; set; }

      #endregion

      #endregion
   }

   [ComImport]
   [Guid("34A715A0-6587-11D0-924A-0020AFC7AC4D")]
   [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
   [TypeLibType(TypeLibTypeFlags.FHidden)]
   private interface DWebBrowserEvents2
   {
      #region Methods

      #region Abstract Methods

      [PreserveSig]
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      [DispId(250)]
      void BeforeNavigate2([In] [MarshalAs(UnmanagedType.IDispatch)] object pDisp,
                           [In] [MarshalAs(UnmanagedType.Struct)] ref object url,
                           [In] [MarshalAs(UnmanagedType.Struct)] ref object flags,
                           [In] [MarshalAs(UnmanagedType.Struct)] ref object targetFrameName,
                           [In] [MarshalAs(UnmanagedType.Struct)] ref object postData,
                           [In] [MarshalAs(UnmanagedType.Struct)] ref object headers,
                           [In] [Out] ref bool cancel);

      [PreserveSig]
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      [DispId(271)]
      void NavigateError([In] [MarshalAs(UnmanagedType.IDispatch)] object pDisp,
                         [In] [MarshalAs(UnmanagedType.Struct)] ref object url,
                         [In] [MarshalAs(UnmanagedType.Struct)] ref object frame,
                         [In] [MarshalAs(UnmanagedType.Struct)] ref object statusCode,
                         [In] [Out] ref bool cancel);

      #endregion

      #endregion
   }

   private class ExtendedWebBrowserEventHelper : StandardOleMarshalObject, DWebBrowserEvents2
   {
      #region Variables

      private readonly ExtendedWebBrowser _parent;

      #endregion

      #region Constructors

      public ExtendedWebBrowserEventHelper(ExtendedWebBrowser parent)
      {
         this._parent = parent;
      }

      #endregion

      #region Methods

      #region Implements Interface

      public void BeforeNavigate2(object pDisp, ref object url, ref object flags, ref object targetFrameName, ref object postData, ref object headers, ref bool cancel)
      {
         _parent.OnBeforeNavigate2(pDisp, ref url, ref flags, ref targetFrameName, ref postData, ref headers, ref cancel);
      }

      public void NavigateError(object pDisp, ref object url, ref object frame, ref object statusCode, ref bool cancel)
      {
         _parent.OnNavigateError(pDisp, ref url, ref frame, ref statusCode, ref cancel);
      }

      #endregion

      #endregion
   }
}