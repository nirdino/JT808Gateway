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

namespace JT808.Gateway.RabbitMQ
{
    public sealed class JT808SessionProducer : IJT808SessionProducer
    {
        private bool disposed = false;
        public string TopicName { get; }
        private readonly EasyNetQ.IBus bus;
       
        private readonly string connStr = "host=113.141.66.240:31673;virtualHost=JT808-Gateway;username=dino;password=hbgz.123";
        private readonly JT808SessionProducerConfig config;
        private readonly ILogger logger;
        public JT808SessionProducer(IOptions<JT808SessionProducerConfig> producerConfigAccessor)
        {
            config = producerConfigAccessor.Value; 
            TopicName = producerConfigAccessor.Value.TopicName;
            connStr = config.ConnectStr;
            bus = RabbitHutch.CreateBus(connStr);
        }

        public async ValueTask ProduceAsync(string notice, string terminalNo)
        {
            if (disposed) return;
            var msg = new MsgNotice() { notice = notice, terminalNo = terminalNo };
            await bus.PubSub.PublishAsync<MsgNotice>(msg, TopicName);
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
        ~JT808SessionProducer()
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
