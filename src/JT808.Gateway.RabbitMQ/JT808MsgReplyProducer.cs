
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

namespace JT808.Gateway.RabbitMQ
{
    public sealed class JT808MsgReplyProducer : IJT808MsgReplyProducer
    {
        private bool disposed = false;
        public string TopicName { get;}
        private readonly EasyNetQ.IBus bus;
        private readonly string connStr = "host=113.141.66.240:31673;virtualHost=JT808-Gateway;username=dino;password=hbgz.123";
        private readonly JT808MsgReplyProducerConfig config;
        System.Threading.Timer timer;
        private readonly ILogger logger;
        public JT808MsgReplyProducer(ILoggerFactory loggerFactory,
          IOptions<JT808MsgReplyProducerConfig> producerConfigAccessor)
        {
            logger = loggerFactory.CreateLogger<JT808MsgReplyProducer>();
            config = producerConfigAccessor.Value;
            TopicName = producerConfigAccessor.Value.TopicName;
            //timer = new Timer(new TimerCallback(s =>
            //{
            //    if (logger.IsEnabled(LogLevel.Trace))
            //        logger.LogTrace($"JT808MsgReplyProducer:{revCount}");
            //}), null, 0, 1000);
            connStr = config.ConnectStr;
            bus = RabbitHutch.CreateBus(connStr);
        }
        long revCount = 0;
        public async ValueTask ProduceAsync(string terminalNo, byte[] data)
        {
            Interlocked.Increment(ref revCount);
            if (disposed) return;
            var msg = new MsgReply() { terminalNo = terminalNo, data = data };
            await bus.PubSub.PublishAsync<MsgReply>(msg, TopicName);
        }

        private void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                bus.Dispose();
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
