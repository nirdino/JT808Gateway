
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

namespace JT808.Gateway.RabbitMQ
{
    public sealed class JT808MsgReplyConsumer : IJT808MsgReplyConsumer
    {
        private bool disposed = false;
        public CancellationTokenSource Cts { get; private set; } = new CancellationTokenSource();
        private readonly string connStr = "host=113.141.66.240:31673;virtualHost=JT808-Gateway;username=dino;password=hbgz.123";
        private readonly EasyNetQ.IBus bus;
     
        public string TopicName { get; }
        private readonly JT808MsgReplyConsumerConfig config;
        private readonly ILogger logger;
        public JT808MsgReplyConsumer(
            IOptions<JT808MsgReplyConsumerConfig> consumerConfigAccessor,
            ILoggerFactory loggerFactory)
        {
            config = consumerConfigAccessor.Value;
            TopicName = consumerConfigAccessor.Value.TopicName;
            connStr = config.ConnectStr;
            bus = RabbitHutch.CreateBus(connStr);
            logger = loggerFactory.CreateLogger("JT808MsgReplyConsumer");
        }

        public void OnMessage(Action<(string TerminalNo, byte[] Data)> callback)
        {
            bus.PubSub.Subscribe<MsgReply>(TopicName, message =>
            {
                callback((message.terminalNo, message.data));
            });
        }
        public void Subscribe()
        {
            //consumer.Subscribe(TopicName);
        }

        public void Unsubscribe()
        {
            if (disposed) return;
            //consumer.Unsubscribe();
            Cts.Cancel();
        }

        private void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                //consumer.Close();
                bus.Dispose();
                Cts.Dispose();
            }
            disposed = true;
        }
        ~JT808MsgReplyConsumer()
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
