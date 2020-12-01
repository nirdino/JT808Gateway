﻿using JT808.Gateway.RabbitMQ;
using JT808.Gateway.Configs.RabbitMQ;
using JT808.Gateway.Abstractions;
using JT808.Protocol;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace JT808.Gateway.RabbitMQ
{
    public static class JT808ClientRabbitMQExtensions
    {
        public static IJT808ClientBuilder AddClientRabbitMQ(this IJT808Builder builder)
        {
            return new JT808ClientBuilderDefault(builder);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceDescriptors"></param>
        /// <param name="configuration">GetSection("JT808MsgConsumerConfig")</param>
        /// <returns></returns>
        public static IJT808ClientBuilder AddMsgConsumer(this IJT808ClientBuilder jT808ClientBuilder, IConfiguration configuration)
        {
            jT808ClientBuilder.JT808Builder.Services.Configure<JT808MsgConsumerConfig>(configuration.GetSection("JT808MsgConsumerConfig"));
            jT808ClientBuilder.JT808Builder.Services.TryAddSingleton<IJT808MsgConsumer, JT808MsgConsumer>();
            return jT808ClientBuilder;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceDescriptors"></param>
        /// <param name="configuration">GetSection("JT808MsgReplyProducerConfig")</param>
        /// <returns></returns>
        public static IJT808ClientBuilder AddMsgReplyProducer(this IJT808ClientBuilder jT808ClientBuilder, IConfiguration configuration)
        {
            jT808ClientBuilder.JT808Builder.Services.Configure<JT808MsgReplyProducerConfig>(configuration.GetSection("JT808MsgReplyProducerConfig"));
            jT808ClientBuilder.JT808Builder.Services.TryAddSingleton<IJT808MsgReplyProducer, JT808MsgReplyProducer>();
            return jT808ClientBuilder;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="jT808NettyBuilder"></param>
        /// <param name="configuration">GetSection("JT808MsgReplyConsumerConfig")</param>
        /// <returns></returns>
        public static IJT808ClientBuilder AddMsgReplyConsumer(this IJT808ClientBuilder jT808ClientBuilder, IConfiguration configuration)
        {
            jT808ClientBuilder.JT808Builder.Services.Configure<JT808MsgReplyConsumerConfig>(configuration.GetSection("JT808MsgReplyConsumerConfig"));
            jT808ClientBuilder.JT808Builder.Services.TryAddSingleton<IJT808MsgReplyConsumer, JT808MsgReplyConsumer>();
            return jT808ClientBuilder;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceDescriptors"></param>
        /// <param name="configuration">GetSection("JT808SessionConsumerConfig")</param>
        /// <returns></returns>
        public static IJT808ClientBuilder AddSessionConsumer(this IJT808ClientBuilder jT808ClientBuilder, IConfiguration configuration)
        {
            jT808ClientBuilder.JT808Builder.Services.Configure<JT808SessionConsumerConfig>(configuration.GetSection("JT808SessionConsumerConfig"));
            jT808ClientBuilder.JT808Builder.Services.TryAddSingleton<IJT808SessionConsumer, JT808SessionConsumer>();
            return jT808ClientBuilder;
        }
    }
}