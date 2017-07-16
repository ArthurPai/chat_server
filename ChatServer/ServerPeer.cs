using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using ChatProtocol;
using ExitGames.Logging;

namespace ChatServer
{
    public class ServerPeer : ClientPeer
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private ServerApplication m_Server;

        public Guid m_Guid;

        public ServerPeer(InitRequest initRequest, ServerApplication _server) : base(initRequest)
        {
            // 建構子
            m_Guid = Guid.NewGuid();
            m_Server = _server;

            m_Server.Users.AddPeer(m_Guid, this);
        }

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            // 斷線處理，例如釋放資源
            m_Server.Users.UserOffline(m_Guid);
        }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            // 收到Client傳過來的要求，並且加以處理
            if (Log.IsDebugEnabled)
            {
                Log.Debug("OperationRequest");
                foreach (KeyValuePair<byte, object> item in operationRequest.Parameters)
                {
                    Log.DebugFormat("{0} : {1}", item.Key, item.Value.ToString());
                }
            }

            switch(operationRequest.OperationCode)
            {
                case (byte)OperationCode.Login:
                    HandleLogin(operationRequest, sendParameters);
                    break;
            }
        }

        private void HandleLogin(OperationRequest operationRequest, SendParameters sendParameters)
        {
            OperationResponse respone;

            if (operationRequest.Parameters.Count < 2)
            {
                // 參數錯誤
                respone = new OperationResponse((byte)OperationCode.Login)
                {
                    ReturnCode = (short)ErrorCode.InvalidParameter,
                    DebugMessage = "Login Failed",
                };

                SendOperationResponse(respone, sendParameters);
                return;
            }

            var name = (string)operationRequest.Parameters[(byte)LoginParameterCode.Name];
            var password = (string)operationRequest.Parameters[(byte)LoginParameterCode.Password];

            User user = m_Server.Users.GetUserByName(name);
            if (user == null || user.password != password)
            {
                // 帳號密碼錯誤
                respone = new OperationResponse((byte)OperationCode.Login)
                {
                    ReturnCode = (short)ErrorCode.InvalidAccountOrPassword,
                    DebugMessage = "Invalid Name or Password",
                };

                SendOperationResponse(respone, sendParameters);
                return;
            }

            Log.Debug("guid 1: " + user.guid);
            UserResult result = m_Server.Users.UserOnline(m_Guid, user.ID);
            Log.Debug("guid 2: " + user.guid);
            if (result.ReturnCode != (byte)ErrorCode.Ok) {
                respone = new OperationResponse((byte)OperationCode.Login)
                {
                    ReturnCode = result.ReturnCode,
                    DebugMessage = result.DebugMessage,
                };

                SendOperationResponse(respone, sendParameters);
                return;
            }

            int Ret = 1;
            var parameters = new Dictionary<byte, object>
            {
                { (byte)LoginResponseCode.Ret, Ret },
                { (byte)LoginResponseCode.ID, user.ID },
                { (byte)LoginResponseCode.Token, user.token },
                { (byte)LoginResponseCode.Name, user.name },
                { (byte)LoginResponseCode.Nickname, user.nickname },
            };

            respone = new OperationResponse((byte)OperationCode.Login, parameters)
            {
                ReturnCode = (short)ErrorCode.Ok,
                DebugMessage = "Login Succeed",
            };

            SendOperationResponse(respone, sendParameters);
        }
    }
}
