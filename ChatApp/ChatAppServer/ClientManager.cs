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
    public class ClientManager
    {
        private Dictionary<string, ClientHandler> connectedClientList = new Dictionary<string, ClientHandler>();

        private Dictionary<string, string> idUserMap = new Dictionary<string, string>();

        int _nextID = 1;

        public string RegistClient(ClientHandler clientHandler)
        {
            string assignmentId = _nextID.ToString("000");

            // 接続済みリストに登録
            connectedClientList.Add(assignmentId, clientHandler);

            // ID更新
            this._nextID++;

            return assignmentId;
        }

        public void RegistUserName(string registerID,string preferUserName) 
        {
            if (!this.connectedClientList.ContainsKey(registerID)) throw new Exception($"ID:{registerID} does not exist.");

            this.idUserMap[registerID] = preferUserName;
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
