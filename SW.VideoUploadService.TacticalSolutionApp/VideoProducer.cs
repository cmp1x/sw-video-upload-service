using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SW.VideoUploadService.TacticalSolutionApp
{
    internal class VideoProducer
    {
        public VideoProducer()
        {
        }

        internal void Produce(string queueName, string exchangeName, string routingKey)
        {
            var video = this.MakeVideo();
            
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(
                        queue: queueName,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);

                    var body = GetBodyFromString(video);

                    channel.BasicPublish(
                        exchange: exchangeName,
                        routingKey: routingKey,
                        basicProperties: null,
                        body: body);

                    MakeLog(video);
                }
            }
        }

        public async Task MakePauseAsync(int duration)
        {
           await Task.Delay(duration);
        }

        private void MakeLog(string video)
        {
            Console.WriteLine($"Video \"{video}\" was sended.");
        }

        private byte[] GetBodyFromString(string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        private string MakeVideo()
        {
            int durationLimit = 20;
            int messageLengthLimit = 101;
            int firstCharInAscii = 20;
            int lastCharInAscii = 128;

            var messageLength = new Random();
            var charOfMessage = new Random();

            var duration = charOfMessage.Next(durationLimit).ToString();
            var message = new StringBuilder(duration);

            for (int i = 0; i <= messageLength.Next(messageLengthLimit); i++)
            {
                message = message.Append((char)charOfMessage.Next(firstCharInAscii, lastCharInAscii));
            }

            return message.ToString();
        }
    }
}