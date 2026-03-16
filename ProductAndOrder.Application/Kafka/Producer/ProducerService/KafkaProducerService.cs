using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json.Serialization;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NPOI.Util;
using ProductAndOrder.Application.Kafka.Producer.ProducerInterface;

namespace ProductAndOrder.Application.Kafka.Producer.ProducerService
{
public class KafkaProducerService : IKafkaProducer, IDisposable
	{
		private readonly IProducer<string, string> _producer;
		private readonly ILogger<KafkaProducerService> _logger;

		public KafkaProducerService(IConfiguration config, ILogger<KafkaProducerService> logger)
		{
			_logger = logger;

			var producerConfig = new ProducerConfig
			{
				BootstrapServers = config["Kafka:BootstrapServers"],
				ClientId = config["Kafka:ProducerId"],
				Acks = Acks.All,
				EnableIdempotence = true,
				MessageSendMaxRetries = 3,
				RetryBackoffMs = 1000
			};
			_producer = new ProducerBuilder<string, string>(producerConfig).Build();

			
		}
		public async Task ProducerAsync<T>(string topic,string key,T message)
		{
			var serialized=JsonConvert.SerializeObject(message);

			var KafkaMessage = new Message<string, string>
			{
				Key=key,
				Value=serialized

			};

			try
			{
				var result= await _producer.ProduceAsync(topic,KafkaMessage);
				_logger.LogInformation("Message produced to topic {Topic}, partition {Partition}, offset {Offset}",
					result.Topic,result.Partition,result.Offset);
			}
			catch(ProduceException<string,string> ex)
			{
				_logger.LogError(ex, "Failed to produce message To topic {Topic}", topic);
				throw;
			}
			
		}
		public void Dispose() => _producer?.Dispose();
		
	}
}
