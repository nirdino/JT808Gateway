
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace JT808.Gateway.Configs.RabbitMQ
{
    public class JT808MsgReplyConsumerConfig : JT808ConsumerConfig, IOptions<JT808MsgReplyConsumerConfig>
    {
        JT808MsgReplyConsumerConfig IOptions<JT808MsgReplyConsumerConfig>.Value => this;
    }
}
