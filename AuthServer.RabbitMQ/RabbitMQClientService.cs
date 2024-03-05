using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.RabbitMQ
{
    public class RabbitMQClientService : IDisposable
    {
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;
        public static string ExchangeName = "EmailSendDirechExchange";
        public static string RoutingEmailSend = "emailsend-route";
        public static string QueueName = "emailsend-queue";

        public RabbitMQClientService(ConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IModel Connect()
        {
            _connection = _connectionFactory.CreateConnection();

            if(_channel != null && _channel.IsOpen) 
            {
                return _channel;
            }

            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(ExchangeName, type: "direct", true, false);
            _channel.QueueDeclare(QueueName, true, false, false, null);
            _channel.QueueBind(exchange: ExchangeName, queue: QueueName, routingKey: RoutingEmailSend);

            return _channel;
        }

        public void Dispose()
        {
            if(_channel != null) 
            {
                _channel.Close();
                _channel.Dispose();
            }

            if( _connection != null ) 
            {
                _connection.Close();
                _connection.Dispose();
            }
        }
    }
}
