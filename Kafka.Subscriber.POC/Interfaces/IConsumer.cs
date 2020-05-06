using System.Threading;
using System.Threading.Tasks;

namespace Kafka.POC.Interfaces
{
	public interface IConsumer
	{
		Task StartConsumeAsync(CancellationToken cancellationToken = default);
	}
}
