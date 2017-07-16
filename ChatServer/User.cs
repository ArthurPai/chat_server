using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    public class User
    {
        private static Random random = new Random();

        public int ID { get; set; }          // User ID
        public string password { get; protected set; }          // User ID
        public string name { get; set; }     // User Name
        public string nickname { get; set; } // Nick Name

        public Guid guid { get; set; }       // Peer GUID
        public string token { get; protected set; }    // Access Token

        public DateTime LoginTime { get; set; } // Login Time
        public short status { get; set; }       // Status: 1(Online), 2(In Game)

        public User(int _id, string _password, string _name, string _nickname)
        {
            ID = _id;
            password = _password;
            name = _name;
            nickname = _nickname;

            guid = Guid.Empty;
            UpdateToken();
        }

        public void SetOnline(Guid _guid)
        {
            guid = _guid;
            UpdateToken();
            LoginTime = System.DateTime.Now;
            status = 1;
        }

        public void SetOffline()
        {
            guid = Guid.Empty;
            UpdateToken();
            status = 0;
        }

        public string UpdateToken()
        {
            token = RandomString(16);
            return token;
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
   }
}
