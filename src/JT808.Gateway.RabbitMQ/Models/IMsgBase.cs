using System;
using System.Collections.Generic;
using System.Text;

namespace JT808.Gateway.RabbitMQ.Models
{
    public interface IMsgBase
    {
        public dynamic Deconstruct();
    }
}
