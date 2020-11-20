using JT808.Gateway.Configs.RabbitMQ;
using JT808.Gateway.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;
using EasyNetQ.Topology;
using JT808.Gateway.RabbitMQ.Models;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace JT808.Gateway.RabbitMQ
{
    public sealed class JT808MsgProducer : IJT808MsgProducer
    {
        private bool disposed = false;
        public string TopicName { get; }
        private readonly EasyNetQ.IBus bus;
        private readonly string connStr = "host=113.141.66.240:31673;virtualHost=JT808-Gateway;username=dino;password=hbgz.123";
        private readonly JT808MsgProducerConfig config;
       
        private long CountNum=0;
 private readonly ILogger logger;
        public JT808MsgProducer(
          IOptions<JT808MsgProducerConfig> producerConfigAccessor)
        {
            config = producerConfigAccessor.Value;
            TopicName = producerConfigAccessor.Value.TopicName;
            connStr = config.ConnectStr;
            bus = RabbitHutch.CreateBus(connStr);
        }

        public async ValueTask ProduceAsync(string terminalNo, byte[] data)
        {
            if (disposed) return;
            var msg = new MsgData() { terminalNo = terminalNo, data= data };
            await bus.PubSub.PublishAsync<MsgData>(msg, TopicName);
            Interlocked.Increment(ref CountNum);
            logger.LogInformation($"发送到MQ总数:{CountNum}");
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
