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
    public sealed class JT808MsgConsumer : IJT808MsgConsumer
    {
        private bool disposed = false;
        public CancellationTokenSource Cts { get; private set; } = new CancellationTokenSource();
        private readonly string connStr = "host=113.141.66.240:31673;virtualHost=JT808-Gateway;username=dino;password=hbgz.123";
        private readonly EasyNetQ.IBus bus;
        public string TopicName { get; }
        private readonly JT808MsgConsumerConfig config;
        private readonly ILogger logger;

        public JT808MsgConsumer(
            IOptions<JT808MsgConsumerConfig> consumerConfigAccessor,
            ILoggerFactory loggerFactory)
        {
            config = consumerConfigAccessor.Value;
            TopicName = consumerConfigAccessor.Value.TopicName;
            connStr = config.ConnectStr;
            bus = RabbitHutch.CreateBus(connStr);
            logger = loggerFactory.CreateLogger("JT808MsgConsumer");
        }

        public void OnMessage(Action<(string TerminalNo, byte[] Data)> callback)
        {
             bus.PubSub.SubscribeAsync<MsgData>(TopicName,async message => {
                 Task reslut= Task.Run(() =>
                 {
                     callback((message.terminalNo, message.data));
                 });
                 await reslut;
                 //callback(message.terminalNo, message.data);
            });
            //bus.SubscribeAsync<MsgData>(TopicName, callback);
            //bus.Subscribe<MsgData>(TopicName, message =>
            //{
            //    callback((message.terminalNo,message.data));
            //});
        }




    public void Subscribe()
        {
            //throw new NotImplementedException();
        }

        public void Unsubscribe()
        {
            //throw new NotImplementedException();
        }

        public void Dispose()
        {           
            //必须为true
            Dispose(true);
            //通知垃圾回收机制不再调用终结器（析构器）
            GC.SuppressFinalize(this);
            //throw new NotImplementedException();
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
    }
}
