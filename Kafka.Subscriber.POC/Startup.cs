using Kafka.POC.Consumers;
using Kafka.POC.Interfaces;
using Kafka.POC.Producers;
using Kafka.POC.Subscribers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Security.Authentication;
using ConfluentKafka = Confluent.Kafka;
using Kafka.Commom.POC.Models;

namespace Kafka.POC
{
	public class Startup
	{
		private readonly IConfiguration _configuration;

		public Startup(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton<IConnectionFactory>(ctx =>
			{
				var settings = _configuration.GetSection("RabbitMQ");

				return new ConnectionFactory
				{
					UserName = settings.GetSection("UserName").Value,
					Password = settings.GetSection("Password").Value,
					HostName = settings.GetSection("HostName").Value,
					Port = Convert.ToInt32(settings.GetSection("Port").Value),
					VirtualHost = settings.GetSection("VirtualHost").Value,
					Ssl = new SslOption
					{
						Enabled = Convert.ToBoolean(settings.GetSection("Ssl:Enabled").Value),
						ServerName = settings.GetSection("Ssl:ServerName").Value,
						Version = Enum.TryParse(settings.GetSection("Ssl:Version").Value, out SslProtocols version) ? version : SslProtocols.Tls12,
						CertPath = settings.GetSection("Ssl:CertPath").Value
					},
					DispatchConsumersAsync = true // this is mandatory to have Async Subscribers
				};
			});

			services.AddSingleton<ConfluentKafka.ConsumerConfig>(ctx =>
			{
				var settings = _configuration.GetSection("Kafka");

				return new ConfluentKafka.ConsumerConfig
				{
					GroupId = settings.GetSection("GroupId").Value,
					BootstrapServers = settings.GetSection("BootstrapServers").Value,
					EnableAutoCommit = true, // automatically commits offsets in background
					EnableAutoOffsetStore = false, // let us manually tell the offset is correct after processing the message

					AutoOffsetReset = Enum.TryParse(settings.GetSection("AutoOffsetReset").Value, out ConfluentKafka.AutoOffsetReset autoOffsetReset) 
					? autoOffsetReset : 
					ConfluentKafka.AutoOffsetReset.Earliest
				};
			});

			//RABBIT
			//services.AddSingleton<ISubscriber, RabbitSubscriber>();

			//KAFKA
			services.AddSingleton<ISubscriber, KafkaSubscriber>();

			var channel = Channel.CreateBounded<Message>(100);
			services.AddSingleton(channel);

			services.AddSingleton<IProducer>(ctx =>
			{
				var channel = ctx.GetRequiredService<Channel<Message>>();
				var logger = ctx.GetRequiredService<ILogger<Producer>>();
				return new Producer(channel.Writer, logger);
			});

			services.AddSingleton<IEnumerable<IConsumer>>(ctx =>
			{
				var channel = ctx.GetRequiredService<Channel<Message>>();
				var logger = ctx.GetRequiredService<ILogger<Consumer>>();

				var numberOfConsumers = Convert.ToInt32(this._configuration["numberOfConsumers"]);

				var consumers = Enumerable.Range(1, numberOfConsumers)
																	.Select(i => new Consumer(channel.Reader, logger, i))
																	.ToArray();
				return consumers;
			});

			services.AddHostedService<BackgroundWorkerService>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
		}
	}
}