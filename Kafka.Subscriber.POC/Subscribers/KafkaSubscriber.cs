using Confluent.Kafka;
using Kafka.Common.POC.Extensions;
using Kafka.POC.Interfaces;
using Kafka.POC.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kafka.POC.Subscribers
{
	public class KafkaSubscriber : ISubscriber
	{
		private readonly ConsumerConfig _consumerConfig;
		private readonly IConfiguration _configuration;
		private readonly ILogger<KafkaSubscriber> _logger;

		public event AsyncEventHandler<SubscriberEventArgs> OnMessage;


		public KafkaSubscriber(ConsumerConfig consumerConfig, IConfiguration configuration, ILogger<KafkaSubscriber> logger)
		{
			_consumerConfig = consumerConfig;
			_configuration = configuration;
			_logger = logger;
		}

		public async Task Start(CancellationToken stoppingToken)
		{
			//Run on a separate thread to prevent blocking framework operations
			await Task.Factory.StartNew(async () => await InitSubscription(stoppingToken));
		}

		private async Task InitSubscription(CancellationToken stoppingToken)
		{
			using (var consumer = new ConsumerBuilder<Ignore, string>(_consumerConfig).Build())
			{
				var topic = _configuration["Kafka:Topic"];
				_logger.LogInformation($"Kafka subscriber started. Listening topic '{topic}'");
				consumer.Subscribe(topic);

				try
				{
					while (!stoppingToken.IsCancellationRequested)
					{
						try
						{							
							await Task.Run(async () =>
							{
								var consumerResult = consumer.Consume(TimeSpan.FromSeconds(1));

								if (consumerResult != null)
								{
									var message = MessageExtensions.CreateFrom(consumerResult.Message.Value);
									await this.OnMessage(this, new SubscriberEventArgs(message));

									consumer.StoreOffset(consumerResult);
								}
							});
						}
						catch (ConsumeException e)
						{
							_logger.LogError($"Kafka => Error occurred: {e.Error.Reason}");
						}
					}
				}
				catch (OperationCanceledException)
				{
					// Ensure the consumer leaves the group cleanly and final offsets are committed.
					consumer.Close();
				}
			}
		}
	}
}
