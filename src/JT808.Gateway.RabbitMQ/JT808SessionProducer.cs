using JT808.Gateway.Configs.RabbitMQ;
using JT808.Gateway.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using JT808.Gateway.RabbitMQ.Models;

namespace JT808.Gateway.RabbitMQ
{
    public sealed class JT808SessionProducer : IJT808SessionProducer
    {
        private readonly ILogger logger;
        private bool disposed = false;
        public string TopicName { get; }
        private readonly RabbitMQManage rabbitMQManage;
        private readonly string connStr;
        private readonly JT808SessionProducerConfig config;
       
        public JT808SessionProducer(ILoggerFactory loggerFactory, IOptions<JT808SessionProducerConfig> producerConfigAccessor, RabbitMQManage manageMQ)
        {
            logger = loggerFactory.CreateLogger("JT808SessionProducer");
            config = producerConfigAccessor.Value;
            TopicName = producerConfigAccessor.Value.TopicName;
            connStr = config.ConnectStr;
            rabbitMQManage = manageMQ;
        }


        
        public async void ProduceAsync(string notice, string terminalNo)
        {
            if (disposed) return;
            var msg = new MsgNotice() { notice = notice, terminalNo = terminalNo };
            await rabbitMQManage.PublishAsync<MsgNotice>(connStr, msg, TopicName);
        }
        

        ~JT808SessionProducer()
        {
            Dispose(false);
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

        public void Dispose()
        {
            //必须为true
            Dispose(true);
            //通知垃圾回收机制不再调用终结器（析构器）
            GC.SuppressFinalize(this);
        }


    }
}
