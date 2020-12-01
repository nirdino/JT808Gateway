using EasyNetQ;
using JT808.Gateway.Configs.RabbitMQ;
using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ.Topology;
using EasyNetQ.LightInject;
using Newtonsoft.Json;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using Microsoft.Extensions.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using JT808.Gateway.RabbitMQ.Models;

namespace JT808.Gateway.RabbitMQ
{
    /// <summary>
    /// RabbitMQ管理
    /// </summary>
    public class RabbitMQManage
    {
        private ConcurrentDictionary<string, IBus> dictBus = new ConcurrentDictionary<string, IBus>();
        private readonly ILogger<RabbitMQManage> logger;
        private readonly System.Threading.Timer timer;
        long revCount = 0;
        long sendCount = 0;
        public RabbitMQManage(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<RabbitMQManage>();
            timer = new Timer(new TimerCallback(s =>
            {
                if (logger.IsEnabled(LogLevel.Trace))
                    logger.LogTrace($"RabbitMQManage revCount:{revCount} and sendCount:{sendCount}");
            }), null, 0, 1000);
        }
        public IBus GetEventBus(string connStr)
        {
            return dictBus.GetOrAdd(connStr, (s) =>
            {
                return RabbitHutch.CreateBus(s);
            });
        }
        #region 发送
        public async ValueTask PublishAsync<T>(string connStr, T msg, string TopicName) where T : class
        {
            var bus = GetEventBus(connStr);
            await bus.PubSub.PublishAsync<T>(msg);
            Interlocked.Increment(ref sendCount);
        }
        #endregion

        #region 订阅
        List<(string TopicName, dynamic ProcessCallback)> SubscribeActionList = new List<(string, dynamic)>();
        List<string> sublist = new List<string>();
        
        /// <summary>
        /// 注册回调
        /// </summary>
        /// <param name="TopicName"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public async ValueTask RegisterProcessor(string TopicName, dynamic callback)
        {
            SubscribeActionList.Add((TopicName, callback));
        }
        /// <summary>
        /// 订阅—这里的机制订阅和处理分离，订阅仅指订阅MQ的订阅，需要调用RegisterProcessor进行处理
        /// </summary>
        /// <typeparam name="T">订阅的数据类型</typeparam>
        /// <param name="connStr">连接串</param>
        /// <param name="TopicName">主题(队列小名)</param>
        /// <returns></returns>
        public async ValueTask Subscribe<T>(string connStr, string TopicName) where T : class, IMsgBase
        {
            if (SubscribeActionList.Exists(s => s.TopicName == TopicName)) return;
            var bus = GetEventBus(connStr);
            await bus.PubSub.SubscribeAsync<T>(TopicName, message => Task.Factory.StartNew(() =>
            {
                //查询所有订阅,并根据主题分发到处理实现者
                SubscribeActionList.FindAll(s => s.Item1 == TopicName)
                .ForEach(async callback =>
                {
                    if (callback.Item2 != null)
                    {
                        await callback.ProcessCallback(message.Deconstruct());
                    }
                });
                Interlocked.Increment(ref revCount);
            }));
        }
        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <param name="connStr"></param>
        /// <param name="topicName"></param>
        public void Unsubscribe(string connStr, string topicName)
        {
            SubscribeActionList.Remove(SubscribeActionList.Find(s => s.Item1 == topicName));
            //throw new NotImplementedException();
        }

        #endregion

        public void DisposeBus()
        {
            foreach (var v in dictBus.Values)
            {
                v.Dispose();
            }
            dictBus.Clear();
        }
    }
}