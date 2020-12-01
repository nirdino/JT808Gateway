﻿
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace JT808.Gateway.Configs.RabbitMQ
{
    public class JT808SessionConsumerConfig : JT808ConsumerConfig, IOptions<JT808SessionConsumerConfig>
    {
        JT808SessionConsumerConfig IOptions<JT808SessionConsumerConfig>.Value => this;
    }
}
