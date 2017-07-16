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

        public Guid m_Guid;

        private int m_UserID;
        private string m_Token;
        private string m_UserName;
        private string m_NickName;
        private static Random random = new Random();

        public ServerPeer(InitRequest initRequest) : base(initRequest)
        {
            // 建構子
            m_Guid = Guid.NewGuid();
        }

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            // 斷線處理，例如釋放資源
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

            if (name != "test" || password != "111111")
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

            int Ret = 1;
            m_UserID = 1;
            m_Token = RandomString(16);
            m_UserName = name;
            m_NickName = "Tester";
            var parameters = new Dictionary<byte, object>
            {
                { (byte)LoginResponseCode.Ret, Ret },
                { (byte)LoginResponseCode.ID, m_UserID },
                { (byte)LoginResponseCode.Token, m_Token },
                { (byte)LoginResponseCode.Name, m_UserName },
                { (byte)LoginResponseCode.Nickname, m_NickName },
            };

            respone = new OperationResponse((byte)OperationCode.Login, parameters)
            {
                ReturnCode = (short)ErrorCode.Ok,
                DebugMessage = "Login Succeed",
            };

            SendOperationResponse(respone, sendParameters);
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
