namespace SPTarkov.Launcher
{
	public class ServerInfo
	{
		public string backendUrl;
		public string name;
		public string[] editions;

		public ServerInfo()
		{
			backendUrl = "https://127.0.0.1";
			name = "Local SPTarkov Server";
			editions = new string[0];
		}
	}
}
