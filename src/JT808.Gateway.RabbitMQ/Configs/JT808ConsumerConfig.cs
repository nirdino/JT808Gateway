
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace JT808.Gateway.Configs.RabbitMQ
{
    public class JT808ConsumerConfig:  IOptions<JT808ConsumerConfig>
    {
        public string TopicName { get; set; }
        public string ConnectStr { set; get; }
        public JT808ConsumerConfig Value => this;
    }
}
