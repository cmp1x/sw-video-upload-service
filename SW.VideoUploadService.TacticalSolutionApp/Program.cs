namespace SW.VideoUploadService.TacticalSolutionApp
{
    using System;
    using System.Threading.Tasks;

    class Program
    {
        public static void Main(string[] args)
        {
            var randomGenerator = new Random();

            var videoMessage = new VideoProducer();

            var queueName = "queue_name";
            var exchangeForQueue = "queue_video_exchange";
            var routinKeyForQueue = "queue_routingKey";

            for (int i = 0; i < 10; i++)
            {
                Console.Write($"[{i}] ");

                videoMessage.Produce(
                    queueName: queueName,
                    exchangeName: exchangeForQueue,
                    routingKey: routinKeyForQueue);

                videoMessage
                    .MakePauseAsync(randomGenerator.Next(100, 1000))
                    .Wait();
            }
        }
    }
}
