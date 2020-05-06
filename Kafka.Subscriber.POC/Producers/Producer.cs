using Kafka.POC.Interfaces;
using Kafka.Commom.POC.Models;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Kafka.POC.Producers
{
	public class Producer : IProducer
	{
		private readonly ChannelWriter<Message> _writer;
		private readonly ILogger<Producer> _logger;

		public Producer(ChannelWriter<Message> writer, ILogger<Producer> logger)
		{
			_writer = writer;
			_logger = logger;
		}

		public async Task PublishAsync(Message message, CancellationToken cancellationToken = default)
		{
			await _writer.WriteAsync(message, cancellationToken);
			_logger.LogDebug($"Producer => published message {message.Id} '{message.Text}'");
		}
	}
}
