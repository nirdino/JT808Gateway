using System;
using System.Collections.Generic;
using System.Text;

namespace JT808.Gateway.RabbitMQ.Models
{
    public class MsgNotice : IMsgBase
    {
        public string notice { set; get; }
        public DateTime dateTime { set; get; }
        public string terminalNo { set; get; }

        public dynamic Deconstruct()
        {
            return (notice: notice, terminalNo: terminalNo);
        }
    }
}
