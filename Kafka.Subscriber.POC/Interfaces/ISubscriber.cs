using Kafka.POC.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Kafka.POC.Interfaces
{
	public interface ISubscriber
	{
		Task Start(CancellationToken stoppingToken);

		event AsyncEventHandler<SubscriberEventArgs> OnMessage;
	}
}
