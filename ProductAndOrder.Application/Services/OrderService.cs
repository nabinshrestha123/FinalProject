using ProductAndOrder.Application.DTO;
using ProductAndOrder.Application.Interfaces;
using ProductAndOrder.Application.Kafka.KafkaEntities;
using ProductAndOrder.Application.Kafka.Producer.ProducerInterface;
using ProductAndOrder.Application.Kafka.Topic;
using ProductAndOrder.Domain.Entities;
using ProductAndOrder.Domain.Enum;
using ProductAndOrder.Domain.Interfaces;

namespace ProductAndOrder.Application.Services
{
	public class OrderService: IOrderDto
	{   private readonly IOrder _Order;
		private readonly IUserServiceClient _UserServiceClient;
		private readonly IKafkaProducer _KafkaProducr;
		private readonly IProduct _product;
		

		public OrderService(IOrder order, IUserServiceClient UserServiceClient,IKafkaProducer kafkaProducer, IProduct product) { 
			_Order = order;
			_UserServiceClient = UserServiceClient;
			_KafkaProducr = kafkaProducer;
			_product = product;
		}
		public async Task<IEnumerable<OrderDto>> GetAllOrderAsync()
		{
			var orders= await _Order.GetAllOrderAsync();
			return orders.Select(o => new OrderDto
			{
              Id = o.Id,	
			  OrderDate = o.OrderDate,
			  OrderStatus= (Domain.Enum.OrderStatus)o.OrderStatus,
			  TotalAmount = o.TotalAmount,

			}).ToList();

		}
		public async Task<ExecutionResult<OrderDto>> GetOrderByIdAsync(int id)
		{
			var orders = await _Order.GetOrderByIdAsync(id);
			if (orders == null)
				return new ExecutionResult<OrderDto>()
				{
					Data = null,
					Message = "OrderId not found",
					Status = ResponseStatus.BadRequest
				};
			var customerDetail = await _UserServiceClient.GetUserAsync(orders.UserId);

			var result = new OrderDto
			{
				Id = orders.Id,
				OrderDate = orders.OrderDate,
				OrderStatus = (Domain.Enum.OrderStatus)orders.OrderStatus,
				TotalAmount = orders.TotalAmount,
				CustomerName = customerDetail == null ? "-" : customerDetail.Name,
			};
			return new ExecutionResult<OrderDto>()
			{
				Data = result,
				Message = "OrderId found",
				Status = ResponseStatus.Ok
			};
		}

		public async Task<ExecutionResult<OrderDto>> AddOrderAsync(CreateOrderDto createorder)
		{
			var isValidProduct = await _product.GetProductByIdAsync(createorder.ProductId);
			if (isValidProduct == null)
			{
				return new ExecutionResult<OrderDto>()
				{
					Data = null,
					Message = "ProductId not found",
					Status = ResponseStatus.BadRequest
				};
			}

			var order = new Order
			{   
				UserId = createorder.UserId,
				OrderDate = createorder.OrderDate,
				OrderStatus = (int)(OrderStatus)createorder.OrderStatus,
				TotalAmount = createorder.TotalAmount,
			};
			var createdOrder = await _Order.AddOrderAsync(order);

			ProductOrder productAndOrder = new ProductOrder
			{
				OrderId = createdOrder.Id,
				ProductId = createorder.ProductId,
				Quantity = createorder.Quantity
			};
			var productOrder = await _Order.AddProductOrderAsync(productAndOrder);

			var orderCreatedEvent = new OrderCreatedEvent
			{
				Message= $"Order with ID {createdOrder.Id} has been created with status {createdOrder.OrderStatus}.",
				UserId = createorder.UserId

			};

			await _KafkaProducr.ProducerAsync(
				topic: KafkaTopics.OrderCreated,
				key: order.UserId.ToString(),
				message: orderCreatedEvent
				);
			
			return new ExecutionResult<OrderDto>()
			{
				Data = new OrderDto
				{
					Id = createdOrder.Id,
					OrderDate = createdOrder.OrderDate,
					OrderStatus = (Domain.Enum.OrderStatus)createdOrder.OrderStatus,
					TotalAmount = createdOrder.TotalAmount,
				},
				Message = "Order created successfully",
				Status = ResponseStatus.Ok
			};
		}
		public async Task<bool> DeleteOrderAsync(int id)
		{
			var dltorder = await _Order.GetOrderByIdAsync(id);	
			if (dltorder == null)
				return false;
			await _Order.DeleteOrderAsync(dltorder);
			return true;

		}
		public async Task<bool> UpdateOrderAsync(UpdateOrderDto updateorder)
		{
			var order = await _Order.GetOrderByIdAsync(updateorder.Id);
			if (order == null)
				return false;
			order.OrderDate = updateorder.OrderDate;
		    order.OrderStatus = (int)(OrderStatus)updateorder.OrderStatus;
			order.TotalAmount = updateorder.TotalAmount;

			await _Order.UpdateOrderAsync(order);

			return true;


		}

	}
}
