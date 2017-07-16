using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;

namespace ChatServer
{
    public class ServerPeer : ClientPeer
    {
        public ServerPeer(InitRequest initRequest) : base(initRequest)
        {
            // 建構子
        }

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            // 斷線處理，例如釋放資源
        }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            // 收到Client傳過來的要求，並且加以處理
        }
    }
}
