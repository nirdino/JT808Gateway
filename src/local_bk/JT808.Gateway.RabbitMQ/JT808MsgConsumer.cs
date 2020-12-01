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
using System.Net;

namespace JT808.Gateway.RabbitMQ
{
    public sealed class JT808MsgConsumer : IJT808MsgConsumer
    { 
        private readonly ILogger logger;
        private bool disposed = false;
        public CancellationTokenSource Cts { get; private set; } = new CancellationTokenSource();
        private readonly JT808MsgConsumerConfig config;
       

        private readonly string connStr;
        public string TopicName { get; }
        private readonly RabbitMQManage rabbitMQManage;

        public JT808MsgConsumer(ILoggerFactory loggerFactory, IOptions<JT808MsgConsumerConfig> consumerConfigAccessor,RabbitMQManage manageMQ)
        {
            logger = loggerFactory.CreateLogger("JT808MsgConsumer");
            config = consumerConfigAccessor.Value;
            TopicName = config.TopicName;
            connStr = config.ConnectStr;
            rabbitMQManage = manageMQ;
        }

        public async void OnMessage(Action<(string TerminalNo, byte[] Data)> callback)
        {
           await rabbitMQManage.RegisterProcessor(TopicName, callback);
        }
        //delegate OnMessageA<MsgData, Task>();
        public async void Subscribe()
        {
            await rabbitMQManage.Subscribe<MsgData>(connStr, TopicName);//.Wait();
        }

        public void Unsubscribe()
        {
            rabbitMQManage.Unsubscribe(connStr, TopicName);
            //throw new NotImplementedException();
        }

        ~JT808MsgConsumer()
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
    }
}
