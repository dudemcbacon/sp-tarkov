﻿namespace EmuTarkov.Launcher
{
	public class ClientConfig
	{
		public string BackendUrl;
		public string Version;

		public ClientConfig()
		{
			BackendUrl = "https://127.0.0.1";
			Version = "live";
		}
	}
}
