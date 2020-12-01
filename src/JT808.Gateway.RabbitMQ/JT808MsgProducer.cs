using JT808.Gateway.Configs.RabbitMQ;
using JT808.Gateway.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;
using EasyNetQ.Topology;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using JT808.Gateway.RabbitMQ.Models;

namespace JT808.Gateway.RabbitMQ
{
    public sealed class JT808MsgProducer : IJT808MsgProducer
    {
        private readonly ILogger logger;
        private bool disposed = false;
        private readonly JT808MsgProducerConfig config;

        private readonly string connStr;
        public string TopicName { get; }
        private readonly RabbitMQManage rabbitMQManage;

        public JT808MsgProducer(ILoggerFactory loggerFactory, IOptions<JT808MsgProducerConfig> producerConfigAccessor, RabbitMQManage manageMQ)
        {
            logger = loggerFactory.CreateLogger("JT808MsgProducer");
            config = producerConfigAccessor.Value;
            TopicName = producerConfigAccessor.Value.TopicName;
            connStr = config.ConnectStr;
            rabbitMQManage = manageMQ;
        }

        public async ValueTask ProduceAsync(string terminalNo, byte[] data)
        {
            if (disposed) return;
            var msg = new MsgData() { terminalNo = terminalNo, data = data };
            await rabbitMQManage.PublishAsync<MsgData>(connStr, msg, TopicName);
        }
        private void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                rabbitMQManage.DisposeBus();
            }
            disposed = true;
        }
        ~JT808MsgProducer()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            //必须为true
            Dispose(true);

            //通知垃圾回收机制不再调用终结器（析构器）
            GC.SuppressFinalize(this);
        }
    }
}
