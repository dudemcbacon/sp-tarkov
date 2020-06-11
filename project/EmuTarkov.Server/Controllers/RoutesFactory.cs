using EmuTarkov.Server.Controllers.Launcher;
using EmuTarkov.Server.Utils.Server;

namespace EmuTarkov.Server.Controllers
{
    public static class RoutesFactory
    {
        public static void CreateRoutes(Router router)
        {
            router.AddStaticRoute("/server/connect", new ConnectRoute());
        }
    }
}
