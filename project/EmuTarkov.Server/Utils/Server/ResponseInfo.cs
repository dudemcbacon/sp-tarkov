namespace EmuTarkov.Server.Utils.Server
{
    public class ResponseInfo
    {
        public string Body;
        public bool Compressed;
        public bool IsFile;

        public ResponseInfo(string body, bool compressed, bool isFile)
        {
            Body = body;
            Compressed = compressed;
            IsFile = isFile;
        }
    }
}
