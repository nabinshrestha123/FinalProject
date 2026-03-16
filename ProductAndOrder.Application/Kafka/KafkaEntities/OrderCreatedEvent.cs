using System;
using System.Collections.Generic;
using System.Text;
using ProductAndOrder.Domain.Enum;

namespace ProductAndOrder.Application.Kafka.KafkaEntities
{
	public class OrderCreatedEvent
	{
		public int OrderId { get; set; }
		public OrderStatus status { get; set; }

		public string Message { get; set; }
	}
}
