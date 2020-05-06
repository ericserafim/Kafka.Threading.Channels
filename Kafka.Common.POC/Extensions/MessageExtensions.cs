using Kafka.Commom.POC.Models;
using System.Text;
using System.Text.Json;

namespace Kafka.Common.POC.Extensions
{
	public static class MessageExtensions
	{
		public static Message CreateFrom(string json)
		{
			 return JsonSerializer.Deserialize<Message>(json);
		}

		public static Message CreateFrom(byte[] stream)
		{
			var json = Encoding.UTF8.GetString(stream);
			return CreateFrom(json);
		}

		public static string ToJson(this Message message)
		{
			return JsonSerializer.Serialize(message);
		}
	}
}
