using System;
using System.Threading.Tasks;

namespace Kafka.POC.Models
{
  public delegate Task AsyncEventHandler<in TEvent>(object sender, TEvent @event) where TEvent : EventArgs;
}
