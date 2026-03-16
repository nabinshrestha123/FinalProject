using System;
using System.Collections.Generic;
using System.Text;

namespace ProductAndOrder.Application.Kafka.Producer.ProducerInterface
{
	public interface IKafkaProducer
	{
		Task ProducerAsync<T>(string topic,string key,T message);
	}
}
