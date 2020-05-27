using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DynamicRabbitMqConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting...");
            var argumentList = GetArgumentDictionary(args);
            if(argumentList["host"] != null && argumentList["queue"] != null)
            {
                SetupSubscription(argumentList["host"], argumentList["queue"]);
            }
                   
        }

        static Dictionary<string, string> GetArgumentDictionary(string[] args)
        {
            //Gather arguments provided. NOTE: Argument names are case 
            //sensitive. Providing the same argument more than once does not producde
            //an error. No spaces are allowed, and one '=' sign must be used.  
            
            var arguments = new Dictionary<string, string>();
            foreach (string argument in args)
            {
                string[] splitted = argument.Split('=');

                if (splitted.Length == 2)
                {
                    arguments[splitted[0]] = splitted[1];
                }
            }
            return arguments;
        }

        public static void SetupSubscription(string host, string queue)
        {
            var factory = new ConnectionFactory() { HostName = host };
            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queue,
                                    durable: true,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body.ToArray());
                    Console.WriteLine(" [x] Received {0}", message);
                };
                channel.BasicConsume(queue: queue,
                                    autoAck: true,
                                    consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }   
   }
}

