using System;
using System.Collections.Generic;
using System.Text;

namespace ProductAndOrder.Application.Kafka.Topic
{
	public class KafkaTopics
	{
		public const string OrderCreated = "order-created";
		public const string OrderStatusChanged = "order-status-changed";
		public const string ProductStockUpdated = "product-stock-updated";
	}
}
