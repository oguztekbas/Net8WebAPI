using AuthServer.RabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;


namespace AuthServer.API.BackgroundServices
{
    // StartAsync metodu 1 kere çalışacak.
    // ExecuteAsync metodu ise uygulama stop olmadığı sürece sürekli
    // çalışacak ve loginden sonra RabbitMQ ile kuyruğa ilettiğimiz emailText
    // mesajını almak için kuyruğu sürekli dinleyecek varsa mail gönderecek.
    // Yani RabbitMQ'nun consume kısmı burası. Producer kısmı Service kısmında
    // Login olunca publish ediyoruz.
    public class EmailSendBackgroundService : BackgroundService
    {
        private readonly RabbitMQClientService _rabbitMQClientService;
        private IModel _channel;

        public EmailSendBackgroundService(RabbitMQClientService rabbitMQClientService)
        {
            _rabbitMQClientService = rabbitMQClientService;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _channel = _rabbitMQClientService.Connect();
            _channel.BasicQos(0, 1, false);

            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            var consumer = new AsyncEventingBasicConsumer(_channel);

            _channel.BasicConsume(RabbitMQClientService.QueueName, false, consumer);

            consumer.Received += Consumer_Received;

            await Task.CompletedTask;
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
        {
            string emailText = JsonSerializer.Deserialize<string>(Encoding.UTF8.GetString(@event.Body.ToArray()));
            await EmailService.EmailHelper.SendMail(emailText);

            _channel.BasicAck(@event.DeliveryTag, false); // kuyruktan sil.
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}
