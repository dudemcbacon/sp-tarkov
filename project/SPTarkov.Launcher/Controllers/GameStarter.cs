﻿using Microsoft.Win32;
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
            if (IsPiratedCopy() > 0)
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

        private int IsPiratedCopy()
        {
            var value0 = 2;

            try
            {
                var value1 = Registry.LocalMachine.OpenSubKey(registeryInstall, false).GetValue("UninstallString");
                var value2 = (value1 != null) ? value1.ToString() : "";
                var value3 = new FileInfo(value2);
                var value4 = new FileInfo[3]
                {
                    value3,
                    new FileInfo(value2.Replace(value3.Name, @"BattlEye/BEClient_x64.dll")),
                    new FileInfo(value2.Replace(value3.Name, @"BattlEye/BEService_x64.dll"))
                };

                foreach (var value in value4)
                {
                    if (File.Exists(value.FullName))
                    {
                        --value0;
                    }
                }
            }
            catch
            {
            }

            return value0;
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
