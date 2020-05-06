using Kafka.Commom.POC.Models;
using System;

namespace Kafka.POC.Models
{
	public class SubscriberEventArgs : EventArgs
	{
		public SubscriberEventArgs(Message message)
		{
			this.Message = message;
		}

		public Message Message { get; }
	}
}
