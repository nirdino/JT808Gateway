
using JT808.Gateway.Configs.RabbitMQ;
using JT808.Gateway.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;
using JT808.Gateway.RabbitMQ.Models;
using Microsoft.Extensions.Logging;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace JT808.Gateway.RabbitMQ
{
    public sealed class JT808MsgReplyProducer : IJT808MsgReplyProducer
    {
        private readonly ILogger logger;
        private bool disposed = false;
        public string TopicName { get; }
        private readonly RabbitMQManage rabbitMQManage;
        private readonly string connStr;
        private readonly JT808MsgReplyProducerConfig config;
        CancellationToken cancellationToken = new CancellationToken();
       
        public JT808MsgReplyProducer(ILoggerFactory loggerFactory, IOptions<JT808MsgReplyProducerConfig> producerConfigAccessor, RabbitMQManage manageMQ)
        {
            logger = loggerFactory.CreateLogger("JT808MsgReplyProducer");
            config = producerConfigAccessor.Value;
            TopicName = producerConfigAccessor.Value.TopicName;
            connStr = config.ConnectStr;
            rabbitMQManage = manageMQ;
        }

        public async ValueTask ProduceAsync(string terminalNo, byte[] data)
        {
            if (disposed) return;
            var msg = new MsgReply() { terminalNo = terminalNo, data = data };
            await rabbitMQManage.PublishAsync<MsgReply>(connStr, msg, TopicName);
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
        ~JT808MsgReplyProducer()
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
