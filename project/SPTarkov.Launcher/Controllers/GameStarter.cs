using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using SPTarkov.Common.Utils.App;

namespace SPTarkov.Launcher
{
	public class GameStarter
	{
        private const string clientExecutable = "EscapeFromTarkov.exe";
        private const string registeryInstall = @"Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\EscapeFromTarkov";
        private const string registerySettings = @"Software\Battlestate Games\EscapeFromTarkov";
        private const string tempDir = @"Battlestate Games\EscapeFromTarkov";

        public int LaunchGame(ServerInfo server, AccountInfo account)
		{
            if (IsPiratedCopy())
            {
                return 2;
            }

            if (account.wipe)
            {
                RemoveRegisteryKeys();
                CleanTempFiles();
            }

            if (!File.Exists(clientExecutable))
			{
				return -1;
			}
			
			ProcessStartInfo clientProcess = new ProcessStartInfo(clientExecutable)
			{
				Arguments = string.Format("-bC5vLmcuaS5u={0} -token={1} -config={2}", GenerateToken(account), account.id, Json.Serialize(new ClientConfig(server.backendUrl))),
				UseShellExecute = false,
				WorkingDirectory = Environment.CurrentDirectory
			};

			Process.Start(clientProcess);

			return 1;
		}

        private bool IsPiratedCopy()
        {
            try
            {
                var value = Registry.LocalMachine.OpenSubKey(registeryInstall, false).GetValue("UninstallString");
                string filepath = (value != null) ? value.ToString() : "";

                if (!string.IsNullOrEmpty(filepath) && File.Exists(filepath))
                {
                    return false;
                }
            }
            catch
            {
            }

            // escape from tarkov is not installed
            return true;
        }

		private void RemoveRegisteryKeys()
		{
			try
			{
				RegistryKey key = Registry.CurrentUser.OpenSubKey(registerySettings, true);

				foreach (string value in key.GetValueNames())
				{
					key.DeleteValue(value);
				}
			}
			catch
			{
			}
		}

		private void CleanTempFiles()
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(Path.GetTempPath(), tempDir));

			if (!Directory.Exists(tempDir))
			{
				return;
			}

			foreach (FileInfo file in directoryInfo.GetFiles())
			{
				file.Delete();
			}

			foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
			{
				directory.Delete(true);
			}
		}

		private string GenerateToken(AccountInfo data)
		{
			LoginToken token = new LoginToken(data.email, data.password);
			string serialized = Json.Serialize(token);
			return string.Format("{0}=", Convert.ToBase64String(Encoding.UTF8.GetBytes(serialized)));
		}
	}
}
