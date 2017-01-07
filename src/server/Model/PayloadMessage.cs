using System;
using Newtonsoft.Json;

namespace Toucan.Server.Model
{
    public class PayloadMessage
    {
        public PayloadMessage()
        {
            this.MessageType = PayloadMessageType.Success;
            this.Text = "";
        }
        public string Text { get; set; }

        public string MessageTypeId
        {
            get
            {
                return Enum.GetNames(typeof(PayloadMessageType)).GetValue((int)this.MessageType).ToString();
            }
            set
            {
                this.MessageType = (PayloadMessageType)Enum.Parse(typeof(PayloadMessageType), value);
            }
        }
        
        [JsonIgnore]
        public PayloadMessageType MessageType { get; set; }
    }
}