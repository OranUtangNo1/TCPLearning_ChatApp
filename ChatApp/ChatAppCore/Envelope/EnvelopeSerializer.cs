using ChatAppCore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChatAppServer
{
    public static class EnvelopeSerializer
    {
        public static MessageEnvelope Deserialize(string jsonString) 
        {
            string cleanedJson = new string(jsonString.Where(c => c != '\uFEFF').ToArray());
            var envelope = JsonSerializer.Deserialize<MessageEnvelope>(cleanedJson) ;
            return envelope;
        }

        public static string Serialize(MessageEnvelope envelope)
        {
            var jsonString = JsonSerializer.Serialize(envelope);
            return jsonString;
        }
    }
}
