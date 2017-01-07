using System;

namespace Toucan.Server.Model
{
    public class Payload<T>
    {
        public Payload()
        {
            this.Message = new PayloadMessage();
        }

        public T Data { get; set; }
        public PayloadMessage Message { get; set; }
    }

}