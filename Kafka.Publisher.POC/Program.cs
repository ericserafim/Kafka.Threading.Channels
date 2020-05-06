using Confluent.Kafka;
using Kafka.Common.POC.Extensions;
using System;
using System.Threading.Tasks;

namespace Kafka.Publisher.POC
{
	class Program
	{
		static void Main(string[] args)
		{
			var conn = new ProducerConfig { BootstrapServers = "localhost:9092" };
			
			var isExit = false;
			while (true)
			{
				PrintMenu();

				var input = Console.ReadKey();
				switch (input.KeyChar)
				{
					case '1':
						SendBulkMessage(conn, 10);
						break;
					case '2':
						SendBulkMessage(conn, 100);
						break;
					case '3':
						SendBulkMessage(conn, 1000);
						break;
					case '4':
						SendBulkMessage(conn, 10000);
						break;
					case '5':
						SendBulkMessage(conn, 1000000);
						break;
					default:
						isExit = true;
						break;
				}

				if (isExit) break;
			}
		}

		private static void PrintMenu()
		{
			Console.Clear();
			Console.WriteLine("1) Send 10 messages simultaneously");
			Console.WriteLine("2) Send 100 messages simultaneously");
			Console.WriteLine("3) Send 1k messages simultaneously");
			Console.WriteLine("4) Send 10k messages simultaneously");
			Console.WriteLine("5) Send 1m messages simultaneously");

			Console.WriteLine("Press [enter] to exit");
		}
		private static void SendBulkMessage(ProducerConfig conn, int numberOfMessages)
		{
			Console.Clear();
			Console.WriteLine($"Sending {numberOfMessages} messages");

			Action<DeliveryReport<Null, string>> handler = r =>
					Console.WriteLine(!r.Error.IsError
							? $"Delivered message to {r.TopicPartitionOffset}"
							: $"Delivery Error: {r.Error.Reason}");

			using (var producer = new ProducerBuilder<Null, string>(conn).Build())
			{
				for (int i = 1; i <= numberOfMessages; ++i)
				{
					var messageJson = CreateMessageJson(i);

					producer.Produce("test", new Message<Null, string> { Value = messageJson }, handler);
				}

				// wait for up to 10 seconds for any inflight messages to be delivered.
				producer.Flush(TimeSpan.FromSeconds(10));
			}
		}

		private static string CreateMessageJson(int messageSequence)
		{
			var message = new Commom.POC.Models.Message
			{
				Id = Guid.NewGuid(),
				CreatedAt = DateTimeOffset.UtcNow,
				Text = $"Message {messageSequence}"
			};

			return message.ToJson();
		}
	}
}