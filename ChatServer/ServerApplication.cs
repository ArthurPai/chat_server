using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;

namespace ChatServer
{
    public class ServerApplication : ApplicationBase
    {
        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            // 建立與Client端的連線，並將建立好的Peer傳回給Photon Server
            return new ServerPeer(initRequest);
        }

        protected override void Setup()
        {
            // 初始化Game Server
        }

        protected override void TearDown()
        {
            // 關閉Game Server，並釋放資源
        }
    }
}
