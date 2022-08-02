namespace SW.VideoUploadService.TacticalSolutionApp
{
    using RabbitMQ.Client;
    using System.Text;
    using System;
    using System.Collections.Generic;
    using RabbitMQ.Client.Events;
    using System.Linq;
    using SW.VideoUploadService.TacticalSolutionApp.Enums;

    public class MessageProduser
    {
        public void Sent(string message, int messageCounter)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "Hello",
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);

                    var body = GetBodyFromString(message);

                    channel.BasicPublish(exchange: "",
                                        routingKey: "Hello",
                                        basicProperties: null,
                                        body: body);
                    Console.WriteLine($" [{messageCounter}] Sent '{message}'");
                }
            }
        }

        public void SentToExchange(string message, int messageCounter, string exchangeName)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout);
                    var body = GetBodyFromString(message);

                    channel.BasicPublish(exchange: exchangeName,
                        routingKey: "",
                        basicProperties: null,
                        body: body);

                    Console.WriteLine($" [{messageCounter}] Sent '{message}'");

                }
            }
        }

        public void SentToDirectExchange(string message, string exchangeName, int messageCounter)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);

                    var body = GetBodyFromString(message);

                    var severity = GetSeverity(message);

                    channel.BasicPublish(exchange: exchangeName,
                        routingKey: severity,
                        basicProperties: null,
                        body: body);

                    Console.WriteLine($" [{messageCounter}] Sent '{severity}':'{message}'");

                }
            }
        }

        public void SentToTopicExchange (string message, string exchangeName, int messageCounter)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchangeName, ExchangeType.Topic);

                    var body = GetBodyFromString(message);

                    var routingKey = GetRoutingKey(message);

                    channel.BasicPublish(exchange: exchangeName,
                        routingKey: routingKey,
                        basicProperties: null,
                        body: body);

                    Console.WriteLine($" [{messageCounter}] Sent '{routingKey}':'{message}'");

                }
            }
        }

        private string GetRoutingKey(string message)
        {
            var routingKey = new string[2];
            string firstPart;

            switch (message.Length)
            {
                case int n when (n <= 1):
                    firstPart = Animals.dog.ToString();
                    break;
                case int n when (n > 1 && n <= 3):
                    firstPart = Animals.fly.ToString();
                    break;
                case int n when (n > 3 && n <= 4):
                    firstPart = Animals.fox.ToString();
                    break;
                case int n when (n > 4 && n <= 5):
                    firstPart = Animals.rabbit .ToString();
                    break;
                case int n when (n > 5 && n <= 6):
                    firstPart = Animals.hedgehog.ToString();
                    break;
                default:
                    firstPart = Animals.elephant.ToString();
                    break;
            }

            routingKey[0] = firstPart;

            string secondPart;

            if (message.Contains('1'))
            {
                secondPart = Color.blue.ToString();
            }
            else if (message.Contains('2'))
            {
                secondPart = Color.orange.ToString();
            }
            else if (message.Contains('3'))
            {
                secondPart = Color.red.ToString();
            }
            else if (message.Contains('4'))
            {
                secondPart = Color.white.ToString();
            }
            else 
            {
                secondPart = Color.yellow.ToString();
            }

            routingKey[1] = secondPart;

            return string.Join('.', routingKey).ToString();
        }

        public byte[] GetBodyFromString(string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        public string GetSeverity(string str)
        {
            switch (str.Length)
            {
                case int n when (n <= 1):
                    return Severity.info.ToString();
                case int n when (n > 1 && n <= 6):
                    return Severity.warning.ToString();
                default:
                    return Severity.error.ToString();

            }
        }

       void OldMain()
        {
            Console.WriteLine("Add your message:");

            int counterOfMessage = 0;

            while (Console.ReadKey().Key != ConsoleKey.Escape)
            {
                counterOfMessage++;
                var message = Console.ReadLine();
                new MessageProduser().Sent(message, counterOfMessage);
                new MessageProduser().SentToExchange(message, counterOfMessage, "logs");
                new MessageProduser().SentToDirectExchange(message, "direct_logs", counterOfMessage);
                new MessageProduser().SentToTopicExchange(message, "topic_logs", counterOfMessage);
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
