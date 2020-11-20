﻿
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace JT808.Gateway.Configs.RabbitMQ
{
    public class JT808MsgConsumerConfig : JT808ConsumerConfig, IOptions<JT808MsgConsumerConfig>
    {
        JT808MsgConsumerConfig IOptions<JT808MsgConsumerConfig>.Value => this;
    }
}
