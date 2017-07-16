using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    public class UserResult
    {
        public byte ReturnCode { get; set; }
        public string DebugMessage { get; set; }
    }

    public class UserCollection
    {
        // Peer List
        protected Dictionary<Guid, ServerPeer> ConnectedClients { get; set; }

        // Map Peer Guid to UserID
        protected Dictionary<Guid, int> Guid2UserID { get; set; }
        // Map UserName to UserID
        protected Dictionary<string, int> Name2UserID { get; set; }
        protected Dictionary<int, User> UserList { get; set; }

        public UserCollection()
        {
            ConnectedClients = new Dictionary<Guid, ServerPeer>();
            Guid2UserID = new Dictionary<Guid, int>();
            Name2UserID = new Dictionary<string, int>();
            UserList = new Dictionary<int, User>();
        }

        public void AddPeer(Guid guid, ServerPeer peer)
        {
            ConnectedClients.Add(guid, peer);
        }

        public ServerPeer GetPeer(Guid guid)
        {
            ServerPeer peer;
            ConnectedClients.TryGetValue(guid, out peer);
            return peer;
        }

        public void RemovePeer(Guid guid)
        {
            if (ConnectedClients.ContainsKey(guid))
            {
                ConnectedClients.Remove(guid);
            }
        }

        public void AddUser(int id, string password, string name, string nick_name)
        {
            UserList.Add(id, new User(id, password, name, nick_name));
            Name2UserID.Add(name, id);
        }

        public User GetUser(int id)
        {
            User user = null;
            UserList.TryGetValue(id, out user);
            return user;
        }

        public User GetUser(Guid guid)
        {
            if (Guid2UserID.ContainsKey(guid))
            {
                int userID = Guid2UserID[guid];

                User user = GetUser(userID);
                return user;
            }
            else
            {
                return null;
            }
        }

        public User GetUserByName(string name)
        {
            if (!Name2UserID.ContainsKey(name))
            {
                return null;
            } else
            {
                return GetUser(Name2UserID[name]);
            }
        }

        public UserResult UserOnline(Guid guid, int id)
        {
            UserResult result = new UserResult();
            result.ReturnCode = 1;

            if (!UserList.ContainsKey(id))
            {
                result.ReturnCode = (byte)ChatProtocol.ErrorCode.InvalidAccountOrPassword;
                result.DebugMessage = "帳號不存在";
                return result;
            }

            lock(this)
            {
                // 檢查是否有重複登入
                if (UserList[id].guid != Guid.Empty)
                {
                    result.ReturnCode = (byte)ChatProtocol.ErrorCode.DuplicateLogin;
                    result.DebugMessage = "重複登入";
                    return result;
                }

                Guid2UserID.Add(guid, id);
                UserList[id].SetOnline(guid);

                result.ReturnCode = (byte)ChatProtocol.ErrorCode.Ok;
                result.DebugMessage = "";
                return result;
            }
        }

        public void UserOffline(Guid guid)
        {
            lock(this)
            {
                RemovePeer(guid);

                if (Guid2UserID.ContainsKey(guid))
                {
                    int userID = Guid2UserID[guid];
                    Guid2UserID.Remove(guid);

                    User user = GetUser(userID);
                    user.SetOffline();
                }
            }
        }
    }
}
