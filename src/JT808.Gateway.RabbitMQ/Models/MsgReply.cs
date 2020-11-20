using System;
using System.Collections.Generic;
using System.Text;

namespace JT808.Gateway.RabbitMQ.Models
{
    public class MsgReply
    {
        
        public string terminalNo { set; get; }
        public DateTime dateTime { set; get; }
        public byte[] data { set; get; }
    }
}
