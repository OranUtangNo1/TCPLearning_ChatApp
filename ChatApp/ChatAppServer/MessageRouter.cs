using ChatAppCore.Data;
using ChatAppCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;

namespace ChatAppServer
{
    public class MessageRouter
    {
        private ClientManager _clientManager;

        public MessageRouter(ClientManager clientManager)
        {
            _clientManager = clientManager;
        }

        public void MessageRouting(string jsonString) 
        {
            var envelope = EnvelopeSerializer.Deserialize(jsonString);
            if (envelope == null)throw new ArgumentException("message Invalid.");

            if(envelope.AddressID == "000") 
            {
                // 上層へ通知
            }
            else 
            {
                // ルーティング
                this.MessageRouting(envelope);
            }
        }

        private void MessageRouting(MessageEnvelope envelope)
        {
            if (envelope.AddressID == "000") return;

            var _jsonString = EnvelopeSerializer.Serialize(envelope);
            this._clientManager.GetClient(envelope.AddressID).SendMessage(_jsonString);
        }
    }
}
