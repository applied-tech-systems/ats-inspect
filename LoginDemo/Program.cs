using System;
using System.Windows.Forms;
using ATS.Inspect.Shared.Logging;

namespace InspectLoginDemo;

internal static class Program
{
   /// <summary>
   /// The main entry point for the application.
   /// </summary>
   [STAThread]
   private static void Main()
   {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);

      //
      // Setup logging
      //

      Logger.Log.WriteInformation("Application started.");

      Application.Run(new FormMain());
   }
}