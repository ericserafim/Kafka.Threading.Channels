using Kafka.Commom.POC.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Kafka.POC.Interfaces
{
	public interface IProducer
	{
		Task PublishAsync(Message message, CancellationToken cancellationToken = default);
	}
}
