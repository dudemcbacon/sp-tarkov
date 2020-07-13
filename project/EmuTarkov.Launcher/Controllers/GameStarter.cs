using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using EmuTarkov.Common.Utils.App;

namespace EmuTarkov.Launcher
{
	public class GameStarter
	{
		public int LaunchGame(ServerInfo server, AccountInfo account)
		{
			string clientExecutable = "EscapeFromTarkov.exe";
			bool legalCopy = false;

			if (account.wipe)
			{
				legalCopy = RemoveRegisteryKeys();
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

			return (legalCopy) ? 1 : 2;
		}

		private bool RemoveRegisteryKeys()
		{
			try
			{
				RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Battlestate Games\EscapeFromTarkov", true);

				foreach (string value in key.GetValueNames())
				{
					key.DeleteValue(value);
				}

				return true;
			}
			catch
			{
				// first time launching tarkov, illegal copy detected.
				return false;
			}
		}

		private void CleanTempFiles()
		{
			string tempDir = @"Battlestate Games\EscapeFromTarkov";
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
