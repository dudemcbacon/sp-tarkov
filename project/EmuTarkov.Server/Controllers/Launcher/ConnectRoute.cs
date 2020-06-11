using EmuTarkov.Common.Utils.App;
using EmuTarkov.Server.Models.Launcher;
using EmuTarkov.Server.Utils.Server;

namespace EmuTarkov.Server.Controllers.Launcher
{
    public class ConnectRoute : IResponse
    {
        public ResponseInfo HandleResponse(RequestInfo reqInfo)
        {
            return new ResponseInfo(Json.Serialize(new ServerInfo()), false, false);
        }
    }
}
