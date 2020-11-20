using JT808.Gateway.Client;
using JT808.Gateway.RabbitMQ.QueueHosting.Impl;
using JT808.Gateway.RabbitMQ.QueueHosting.Jobs;
using JT808.Gateway.ReplyMessage;
using JT808.Gateway.WebApiClientTool;
using JT808.Protocol;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace JT808.Gateway.RabbitMQ.QueueHosting
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serverHostBuilder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{ hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
                    var conf = config.Build();
                    //配置文件存Nacos则取消注释
                    //config.AddNacosConfiguration(conf.GetSection("NacosConfig"));
                    config.Build();

                })
                .ConfigureLogging((context, logging) =>
                {
                    Console.WriteLine($"Environment.OSVersion.Platform:{Environment.OSVersion.Platform.ToString()}");
                    logging.AddConsole();
                    logging.SetMinimumLevel(LogLevel.Trace);
                })
                .ConfigureServices((hostContext, services) =>
                {

                    services.AddSingleton<ILoggerFactory, LoggerFactory>();
                    services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
                    services.AddJT808Configure()
                            //添加客户端工具
                            .AddClient()
                            .AddClientReport()
                            .Builder()
                            //添加客户端服务
                            .AddClientRabbitMQ()
                            .AddMsgConsumer(hostContext.Configuration)
                            //添加消息应答生产者
                            .AddMsgReplyProducer(hostContext.Configuration)
                            //添加消息应答服务并实现消息应答处理
                            .AddReplyMessage<JT808ReplyMessageHandlerImpl>()
                            .Builder()
                            //添加消息应答处理
                            .AddGateway(hostContext.Configuration)
                            .AddMessageHandler<JT808CustomMessageHandlerImpl>()
                            .AddServerRabbitMQMsgProducer(hostContext.Configuration)
                            .AddServerRabbitMQSessionProducer(hostContext.Configuration)
                            .AddServerRabbitMQMsgReplyConsumer(hostContext.Configuration)
                            .AddTcp()
                            .AddUdp()
                            .AddHttp()
                            .Register();//必须注册的
                    services.AddJT808WebApiClientTool(hostContext.Configuration);
                    //httpclient客户端调用
                    services.AddHostedService<CallHttpClientJob>();
                    //客户端测试
                    services.AddHostedService<UpJob>();


                    //services.AddNacosConfig

                    /*
                    services.AddSingleton<ILoggerFactory, LoggerFactory>();
                    services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));

                    services.AddJT808Configure()
                    .AddGateway(hostContext.Configuration)
                    .AddServerRabbitMQMsgProducer(hostContext.Configuration)
                    .AddServerRabbitMQMsgReplyConsumer(hostContext.Configuration)
                    .AddServerRabbitMQSessionProducer(hostContext.Configuration)
                    .AddTcp()
                    .Register();
                    */
                    //.AddUdp();
                    // services.AddJT808Configure().AddGateway(hostContext.Configuration);
                    //.AddServerRabbitMQMsgProducer(hostContext.Configuration)
                    //.AddServerRabbitMQMsgReplyConsumer(hostContext.Configuration)
                    //.AddServerRabbitMQSessionProducer(hostContext.Configuration)
                    //.AddTcp()
                    ////.AddUdp()
                    ////.AddGrpc()
                    //.Builder();
                });

            await serverHostBuilder.RunConsoleAsync();
        }
    }
}
