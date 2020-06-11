using EmuTarkov.Server.Controllers;
using EmuTarkov.Server.Utils.Server;

namespace EmuTarkov.Server
{
    /// <summary>
    /// Program entry point
    /// </summary>
    internal class Program
    {
        public static void Main(string[] args)
        {
            new AssemblyLoader("EmuTarkov_Data/Managed/");

            // get url from config
            new SelfSigned("localhost", 443);

            Router router = new Router("https://localhost/", RoutesFactory.CreateRoutes);

            router.Start();

            while (router.IsRunning())
            {
                router.Update();
            }

            router.Stop();
        }
    }
}
