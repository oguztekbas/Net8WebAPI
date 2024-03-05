using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthServer.RabbitMQ;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;

namespace AuthServer.BackgroundService
{
    public class EmailSendBackgroundService : IHostedLifecycleService
    {
        private readonly RabbitMQClientService _rabbitMQClientService;
        private IModel _channel;

        public EmailSendBackgroundService(RabbitMQClientService rabbitMQClientService)
        {
            _rabbitMQClientService = rabbitMQClientService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _channel = _rabbitMQClientService.Connect();

            _channel.BasicQos(
        }

        public Task StartingAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

    }
}
