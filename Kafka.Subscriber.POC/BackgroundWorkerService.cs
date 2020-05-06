using Kafka.POC.Consumers;
using Kafka.POC.Interfaces;
using Kafka.POC.Models;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Kafka.POC
{
	public class BackgroundWorkerService : BackgroundService
	{
		private readonly ISubscriber _subscriber;
		private readonly IProducer _producer;
		private readonly IEnumerable<IConsumer> _consumers;		

		public BackgroundWorkerService(ISubscriber subscriber, IProducer producer, IEnumerable<IConsumer> consumers)
		{			
			_subscriber = subscriber ?? throw new ArgumentNullException(nameof(subscriber));
			_subscriber.OnMessage += OnMessageAsync;

			_producer = producer ?? throw new ArgumentNullException(nameof(producer));
			_consumers = consumers ?? Enumerable.Empty<Consumer>();	
		}

		private async Task OnMessageAsync(object sender, SubscriberEventArgs args)
		{			
			await _producer.PublishAsync(args.Message);
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			//Start listening MQ Broker
			await _subscriber.Start(stoppingToken);

			//Start up all consumers
			var consumerTasks = _consumers.Select(c => c.StartConsumeAsync(stoppingToken));
			await Task.WhenAll(consumerTasks);
		}
	}
}
