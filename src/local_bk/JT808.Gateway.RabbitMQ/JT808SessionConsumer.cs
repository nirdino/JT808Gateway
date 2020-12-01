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
    public sealed class JT808SessionConsumer : IJT808SessionConsumer
    { 
        private readonly ILogger logger;
        private bool disposed = false;
        public CancellationTokenSource Cts { get; private set; } = new CancellationTokenSource();
        private readonly JT808SessionConsumerConfig config;
       

        private readonly string connStr;
        public string TopicName { get; }
        private readonly RabbitMQManage rabbitMQManage;


        public JT808SessionConsumer(ILoggerFactory loggerFactory, IOptions<JT808SessionConsumerConfig> consumerConfigAccessor,RabbitMQManage manageMQ)
        { 
            logger = loggerFactory.CreateLogger("JT808SessionConsumer");
            config = consumerConfigAccessor.Value;
            TopicName = consumerConfigAccessor.Value.TopicName;
            connStr = config.ConnectStr;
            rabbitMQManage = manageMQ;
           
        }

        public async void OnMessage(Action<(string Notice, string TerminalNo)> callback)
        {
            await rabbitMQManage.RegisterProcessor(TopicName, callback);
            /*
            subscriptionResult = bus.SubscribeAsync<MsgNotice>(TopicName, message => Task.Factory.StartNew(() =>
            {
                callback((message.notice, message.terminalNo));
                //这里执行一些操作
                //如果这里有一个异常，那么在这个Task执行完毕后，这个异常会作为结果返回，
                // 然后任务将继续执行下去。
            }).ContinueWith(task =>
            {
                if (task.IsCompleted && !task.IsFaulted)
                {
                    Interlocked.Increment(ref revCount);
                    // 一切都很好
                }
                else
                {
                    // 不要Catch 异常，否则异常会进一步被嵌套，结果会被发送到默认的错误队列
                    throw new EasyNetQException("Message processing exception - look in t  the default error quenue(broker)");
                }
            }));
            */
        }

        public async void Subscribe()
        {
            await rabbitMQManage.Subscribe<MsgNotice>(connStr, TopicName);
        }

        public void Unsubscribe()
        {
            if (disposed) return;
            rabbitMQManage.Unsubscribe(connStr, TopicName);
            //consumer.Unsubscribe();
            Cts.Cancel();
        }
        ~JT808SessionConsumer()
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
