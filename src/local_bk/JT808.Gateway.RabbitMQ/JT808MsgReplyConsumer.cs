
using JT808.Gateway.Configs.RabbitMQ;
using JT808.Gateway.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using JT808.Gateway.RabbitMQ.Models;
using Microsoft.Extensions.DependencyInjection;

namespace JT808.Gateway.RabbitMQ
{
    public sealed class JT808MsgReplyConsumer : IJT808MsgReplyConsumer
    { 
        private readonly ILogger logger;
        private bool disposed = false;
        public CancellationTokenSource Cts { get; private set; } = new CancellationTokenSource();
        private readonly JT808MsgReplyConsumerConfig config;
       

        private readonly string connStr;
        public string TopicName { get; }
        private readonly RabbitMQManage rabbitMQManage;


        public JT808MsgReplyConsumer(ILoggerFactory loggerFactory, IOptions<JT808MsgReplyConsumerConfig> consumerConfigAccessor,RabbitMQManage manageMQ)
        {  
            logger = loggerFactory.CreateLogger("JT808MsgReplyConsumer");
            config = consumerConfigAccessor.Value;
            TopicName = consumerConfigAccessor.Value.TopicName;
            connStr = config.ConnectStr;
            rabbitMQManage = manageMQ;
          
        }

        public async void OnMessage(Action<(string TerminalNo, byte[] Data)> callback)
        {
            await rabbitMQManage.RegisterProcessor(TopicName, callback);
        }
        public async void Subscribe()
        {
            await rabbitMQManage.Subscribe<MsgReply>(connStr, TopicName);
        }

        public void Unsubscribe()
        {
            if (disposed) return;
            rabbitMQManage.Unsubscribe(connStr, TopicName);
            Cts.Cancel();
        }
        ~JT808MsgReplyConsumer()
        {
            Dispose(false);
        }
        private void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                rabbitMQManage.DisposeBus();
                Cts.Dispose();
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
