using Confluent.Kafka;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace StreamDataProcessor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddSingleton(new ConsumerConfig
                    {
                        BootstrapServers = "localhost:9092",
                        GroupId = "test2",
                        AutoOffsetReset = AutoOffsetReset.Earliest,
                        EnableAutoCommit = true
                    });
                    services.AddSingleton(typeof(StreamReader));
                    services.AddSingleton(typeof(DbHelper));
                });
    }
}
