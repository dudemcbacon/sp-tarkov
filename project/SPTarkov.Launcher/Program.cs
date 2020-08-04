using System;
using System.Threading;
using System.Windows.Forms;
using SPTarkov.Launcher.Controllers;

namespace SPTarkov.Launcher
{
	public static class Program
	{
		[STAThread]
		private static void Main()
		{
			AssemblyLoader.Run("EscapeFromTarkov_Data/Managed/");

			// make sure assembly is resolved
			Run();
		}

		private static void Run()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.ThreadException += (sender,args) => HandleException(args.Exception);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
			Application.Run(new Main());
		}

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exception)
            {
                HandleException(exception);
            }
            else
            {
                HandleException(new Exception("Unknown Exception!"));
            }
        }

        private static void HandleException(Exception exception)
        {
            var text = $"Exception Message:{exception.Message}{Environment.NewLine}StackTrace:{exception.StackTrace}";
			LogManager.Instance.Error(text);
            MessageBox.Show(text, "Exception", MessageBoxButtons.OK,MessageBoxIcon.Error);
        }
    }
}
