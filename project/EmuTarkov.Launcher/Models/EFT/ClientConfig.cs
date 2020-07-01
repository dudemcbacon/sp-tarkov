namespace EmuTarkov.Launcher
{
	public class ClientConfig
	{
		public string BackendUrl;
		public string Version;

		public ClientConfig(string backendUrl)
		{
			BackendUrl = backendUrl;
			Version = "live";
		}
	}
}
