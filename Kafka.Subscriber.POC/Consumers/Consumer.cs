using Kafka.Commom.POC.Models;
using Kafka.POC.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Kafka.POC.Consumers
{
	public class Consumer : IConsumer
	{
		private readonly ChannelReader<Message> _reader;
		private readonly ILogger<Consumer> _logger;
		private readonly int _instanceId;

		public Consumer(ChannelReader<Message> reader, ILogger<Consumer> logger, int instanceId)
		{
			_reader = reader;
			_logger = logger;
			_instanceId = instanceId;
		}

		public async Task StartConsumeAsync(CancellationToken cancellationToken = default)
		{
			_logger.LogInformation($"Consumer ({_instanceId}) => started");

			try
			{
				await foreach (var message in _reader.ReadAllAsync(cancellationToken))
				{
					_logger.LogInformation($"{DateTime.Now.ToString("G")}: Consumer ({_instanceId})=> Received message {message.Id} : '{message.Text}'");
					
					//Process simulation
					await Task.Delay(500, cancellationToken);
				}
			}
			catch (OperationCanceledException ex)
			{
				_logger.LogWarning($"Consumer ({_instanceId}) => forced stop. Error: {ex.Message}");
			}

			_logger.LogInformation($"Consumer ({_instanceId}) => shutting down");
		}
	}
}
