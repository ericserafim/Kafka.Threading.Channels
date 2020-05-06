using System;

namespace Kafka.Commom.POC.Models
{
	public class Message
	{
		public Guid Id { get; set; }

		public DateTimeOffset CreatedAt { get; set; }

		public string Text { get; set; }
	}
}
