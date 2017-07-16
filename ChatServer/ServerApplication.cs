using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using ExitGames.Logging;
using ExitGames.Logging.Log4Net;
using log4net.Config;
using System.IO;

namespace ChatServer
{
    public class ServerApplication : ApplicationBase
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            // 建立與Client端的連線，並將建立好的Peer傳回給Photon Server
            return new ServerPeer(initRequest);
        }

        protected override void Setup()
        {
            // 初始化Game Server
            log4net.GlobalContext.Properties["Photon:ApplicationLogPath"] = Path.Combine(this.ApplicationRootPath, "log");
            log4net.GlobalContext.Properties["LogFileName"] = "ChatServer";

            var configFileInfo = new FileInfo(Path.Combine(this.BinaryPath, "log4net.config"));
            if (configFileInfo.Exists)
            {
                LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);
                XmlConfigurator.ConfigureAndWatch(configFileInfo);
            }

            Log.Info("Chat Server is running....");
        }

        protected override void TearDown()
        {
            // 關閉Game Server，並釋放資源
        }
    }
}
