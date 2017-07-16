using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExitGames.Client.Photon;

namespace ChatClient
{
    class Program : IPhotonPeerListener
    {
        static void Main(string[] args)
        {
            var listener = new Program();
            var peer = new PhotonPeer(listener, ConnectionProtocol.Udp);

            if (peer.Connect("127.0.0.1:4530", "ChatServer"))
            {
                do
                {
                    Console.WriteLine(".");

                    // 讓Photon的service可以處理網路資料，這樣OnOperationResponse及OnEvent才會被觸發
                    peer.Service();

                    System.Threading.Thread.Sleep(500);
                } while (true);
            }
        }

        public void DebugReturn(DebugLevel level, string message)
        {
            Console.WriteLine(string.Format("{0}: {1}", level, message));
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
        }

        public void OnStatusChanged(StatusCode statusCode)
        {
            // 連線狀態變化的通知

            Console.WriteLine("Peer status update:" + statusCode.ToString());

            switch (statusCode)
            {
                case StatusCode.Connect:
                    break;
            }
        }
    }
}
