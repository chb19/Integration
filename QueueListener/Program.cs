using DAL;
using DTO;
using MongoDB.Driver;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RedisDataBase;
using System;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueListener
{
    class Program
    {
        private static INewsRepository newsRepository = new NewsRepository(new MongoConfig()
        {
            ConnectionString = "mongodb://localhost",
            Database = "news2"
        });

        private static async Task GetDatabaseNames(MongoClient client)
        {
            using (var cursor = await client.ListDatabasesAsync())
            {
                var databaseDocuments = await cursor.ToListAsync();
                foreach (var databaseDocument in databaseDocuments)
                {
                    Console.WriteLine(databaseDocument["name"]);
                }
            }
        }

        static void Main(string[] args)
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.UserName = ConnectionFactory.DefaultUser;
            factory.Password = ConnectionFactory.DefaultPass;
            factory.VirtualHost = ConnectionFactory.DefaultVHost;
            factory.HostName = "localhost";
            factory.Port = AmqpTcpEndpoint.UseDefaultPort;

            using (IConnection conn = factory.CreateConnection())
            using (var model = conn.CreateModel())
            {
                model.QueueDeclare("news", false, false, false, null);

                var consumer = new EventingBasicConsumer(model);

                consumer.Received += Consumer_Received;
                model.BasicConsume(queue: "news", autoAck: true, consumer: consumer);

                Console.ReadLine();

            }
            Console.WriteLine("\n\n\nStart");
            //ToRedis();

            Show();
            Console.ReadLine();
        }

        private static void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            var body = e.Body;
            NewsDTO n = BinaryConverter.ByteArrayToObject<NewsDTO>(body);

            Console.WriteLine(n);

            newsRepository.AddNews(n);
            //RedisBase.SaveBigData(n.ID, n.Title);
        }

        static void ToRedis()
        {
            foreach (var item in newsRepository.GetAllNewsAsync().GetAwaiter().GetResult())
            {
                RedisBase.SaveBigData(item.ID, item.Title);
            }
        }

        static void Show()
        {
            Console.WriteLine(newsRepository.GetAllNewsAsync().GetAwaiter().GetResult().ToList().Count);
            foreach (var item in newsRepository.GetAllNewsAsync().GetAwaiter().GetResult())
            {
                Console.WriteLine(item);
            }


            //foreach (var item in newsRepository.GetAllNewsAsync().GetAwaiter().GetResult())
            //{
            //    Console.WriteLine(RedisBase.ReadData(item.Title));
            //    Console.WriteLine();
            //    Console.WriteLine();
            //}
        }
    }
}

   
