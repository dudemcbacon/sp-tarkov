using System;
using System.Windows.Forms;

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
			Application.Run(new Main());
		}
    }
}
