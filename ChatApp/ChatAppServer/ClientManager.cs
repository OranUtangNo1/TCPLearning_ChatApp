using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatAppServer
{
    internal class ClientManager
    {
        private Dictionary<string, ClientHandler> connectedClientList = new Dictionary<string, ClientHandler>();

        private Dictionary<string, string> idUserMap = new Dictionary<string, string>();

        int _nextID = 0;

        //public string RegisterClient(ClientHandler clientHandler, string userName)
        public string RegisterClient(ClientHandler clientHandler)
        {
            string assignmentId = _nextID.ToString();

            // 接続済みリストに登録
            connectedClientList.Add(assignmentId, clientHandler);
            // ID/ユーザー名を登録
            //idUserMap.Add(assignmentId, userName);

            // ID更新
            this._nextID++;

            return assignmentId.ToString();
        }

        public bool RemoveClient(string removeId)
        {
            if (this.IsRegitUser(removeId))
            {
                this.connectedClientList.Remove(removeId);
                this.idUserMap.Remove(removeId);
                
                return true;
            }

            return false;
        }

        private bool IsRegitUser(string userID)
        {
            if (this.connectedClientList.ContainsKey(userID) && this.idUserMap.ContainsKey(userID))
            {
                return true;
            }

            return false;
        }


        public  ClientHandler GetClient(string userID)
        {
            return this.connectedClientList[userID];
        }

        public IEnumerable<ClientHandler> GetAllClients()
        {
            return connectedClientList.Values;
        }

        public IReadOnlyDictionary<string,string> GetUserList() 
        {
            return new ReadOnlyDictionary<string, string>(idUserMap);
        }
    }
}
