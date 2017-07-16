using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using ChatProtocol;

namespace ChatClient
{
    enum GameState
    {
        Initiation,
        Connected,
        OnLogin,
        Chatting,
        Disconnected,
    }

    class Program : IPhotonPeerListener
    {
        private GameState m_State;
        private PhotonPeer m_Peer;
        
        public Program()
        {
            m_State = GameState.Initiation;
            m_Peer = new PhotonPeer(this, ConnectionProtocol.Udp);
        }

        static void Main(string[] args)
        {
            new Program().Run();
        }

        public void Run()
        {
            if (m_Peer.Connect("127.0.0.1:4530", "ChatServer"))
            {
                do
                {
                    // 讓Photon的service可以處理網路資料，這樣OnOperationResponse及OnEvent才會被觸發
                    m_Peer.Service();

                    switch(m_State)
                    {
                        case GameState.Initiation:
                            break;
                        case GameState.Connected:
                            LoginToServer();
                            m_State = GameState.OnLogin;
                            break;
                        case GameState.OnLogin:
                            break;
                        case GameState.Chatting:
                            break;
                        case GameState.Disconnected:
                            break;
                    }

                    System.Threading.Thread.Sleep(500);
                } while (true);
            }
        }

        public void DebugReturn(DebugLevel level, string message)
        {
            Console.WriteLine(string.Format("[{0}] {1}", level, message));
        }

        public void OnEvent(EventData eventData)
        {
            // 事件處理
        }

        public void OnMessage(object messages)
        {
            // 訊息處理
        }

        public void OnOperationResponse(OperationResponse operationResponse)
        {
            // 取得Server對Request的回應
            switch (operationResponse.OperationCode)
            {
                case (byte)OperationCode.Login:
                    HandleLoggedIn(operationResponse);
                    break;
                default:
                    DebugReturn(DebugLevel.WARNING, "Unknown Response: " + operationResponse.OperationCode);
                    break;
            }
        }

        public void OnStatusChanged(StatusCode statusCode)
        {
            // 連線狀態變化的通知

            DebugReturn(DebugLevel.INFO, "Peer status update:" + statusCode.ToString());

            switch (statusCode)
            {
                case StatusCode.Connect:
                    m_State = GameState.Connected;
                    break;
                case StatusCode.Disconnect:
                    m_State = GameState.Disconnected;
                    break;
            }
        }

        private void LoginToServer()
        {
            Console.WriteLine("\n請輸入帳號：");
            string name = Console.ReadLine();

            Console.WriteLine("\n請輸入密碼：");
            string password = Console.ReadLine();

            var parameters = new Dictionary<byte, object>
            {
                { (byte)LoginParameterCode.Name, name },
                { (byte)LoginParameterCode.Password, password },
            };

            m_Peer.OpCustom((byte)OperationCode.Login, parameters, true);
        }

        private void HandleLoggedIn(OperationResponse operationResponse)
        {
            switch (operationResponse.ReturnCode)
            {
                case (short)ErrorCode.Ok:
                    {
                        int ret = Convert.ToInt16(operationResponse.Parameters[(byte)LoginResponseCode.Ret]);
                        int id = Convert.ToInt16(operationResponse.Parameters[(byte)LoginResponseCode.ID]);
                        string token = Convert.ToString(operationResponse.Parameters[(byte)LoginResponseCode.Token]);
                        string name = Convert.ToString(operationResponse.Parameters[(byte)LoginResponseCode.Name]);
                        string nick_name = Convert.ToString(operationResponse.Parameters[(byte)LoginResponseCode.Nickname]);

                        Console.WriteLine("Login succeed ({0}) \nUser ID: {1} \nToken: {2} \nName: {3} \nNickName: {4}", ret, id, token, name, nick_name);
                        m_State = GameState.Chatting;
                    }
                    break;
                case (short)ErrorCode.InvalidParameter: // 參數錯誤
                    DebugReturn(DebugLevel.INFO, operationResponse.DebugMessage);
                    m_State = GameState.Connected;
                    break;
                case (short)ErrorCode.InvalidAccountOrPassword: // 帳號密碼錯誤
                    DebugReturn(DebugLevel.INFO, operationResponse.DebugMessage);
                    m_State = GameState.Connected;
                    break;
                case (short)ErrorCode.DuplicateLogin: // 重複登入
                    DebugReturn(DebugLevel.INFO, operationResponse.DebugMessage);
                    m_State = GameState.Connected;
                    break;
                default:
                    DebugReturn(DebugLevel.WARNING, "Unknown RetureCode: " + operationResponse.ReturnCode);
                    m_State = GameState.Connected;
                    break;
            }
        }
    }
}
