using NPOI.OpenXmlFormats.Wordprocessing;
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
	public class OrderService : IOrderDto
	{
		private readonly IOrder _Order;
		private readonly IUserServiceClient _UserServiceClient;
		private readonly IKafkaProducer _KafkaProducr;
		private readonly IProduct _product;
		


		public OrderService(IOrder order, IUserServiceClient UserServiceClient, IKafkaProducer kafkaProducer, IProduct product)
		{
			_Order = order;
			_UserServiceClient = UserServiceClient;
			_KafkaProducr = kafkaProducer;
			_product = product;
		}
		public async Task<IEnumerable<OrderDto>> GetAllOrderAsync()
		{
			var orders = await _Order.GetAllOrderAsync();
			var products = await _product.GetAllProductsAsync();
			var productOrders = await _Order.GetAllProductOrder();
			var productOrder =
			(from o in orders
			 join po in productOrders on o.Id equals po.OrderId
			 join p in products on po.ProductId equals p.Id
			 select new OrderDto
			 {
				 Id = o.Id,
				 ProductName=p.ProductName,
				 OrderDate = o.OrderDate,
				 OrderStatus = (OrderStatus)o.OrderStatus,
				 TotalAmount = o.TotalAmount,
				 Quantity = po.Quantity,
				 OrderStatusInfo = ((OrderStatus)o.OrderStatus).ToString()


			 }).ToList();
			return productOrder;

		}
		public async Task<ExecutionResult<OrderDto>> GetOrderByIdAsync(int orderId)
		{
			var orders = await _Order.GetOrderByIdAsync(orderId);
			if (orders == null)
				return new ExecutionResult<OrderDto>()
				{
					Data = null,
					Message = "OrderId not found",
					Status = ResponseStatus.BadRequest
				};
			var productOrder = await _Order.GetProductOrderByOrderIdAsync(orderId);
			if (productOrder == null)
				return new ExecutionResult<OrderDto>()
				{
					Data = null,
					Message = "Product id not found",
					Status = ResponseStatus.BadRequest
				};

			var productDetail = await _product.GetProductByIdAsync(productOrder.ProductId);
			if (productDetail == null)
				return new ExecutionResult<OrderDto>()
				{
					Data = null,
					Message = "Product not found",
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
				ProductName= productDetail.ProductName,
				OrderStatusInfo = ((OrderStatus)orders.OrderStatus).ToString()
			};
			return new ExecutionResult<OrderDto>()
			{
				Data = result,
				Message = "OrderId found",
				Status = ResponseStatus.Ok
			};
		}

		public async Task<ExecutionResult<int>> AddOrderAsync(CreateOrderDto createorder,int actionBy)
		{
			var isValidProduct = await _product.GetProductByIdAsync(createorder.ProductId);
			if (isValidProduct == null)
			{
				return new ExecutionResult<int>()
				{
					Data = 0,
					Message = "ProductId not found",
					Status = ResponseStatus.BadRequest
				};
			}

			var order = new Order
			{
				UserId = actionBy,
				OrderDate = DateTime.UtcNow,
				OrderStatus = (int)OrderStatus.Pending,
				TotalAmount = createorder.Quantity * isValidProduct.Price ?? 0

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
				Message = $"Order with ID {createdOrder.Id} has been created with status {createdOrder.OrderStatus}.",
				UserId = actionBy

			};

			await _KafkaProducr.ProducerAsync(
				topic: KafkaTopics.OrderCreated,
				key: order.UserId.ToString(),
				message: orderCreatedEvent
				);

			return new ExecutionResult<int>()
			{
				Data = createdOrder.Id,
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
		public async Task<ExecutionResult<bool>> UpdateOrderAsync(UpdateOrderDto request)
		{
			Order order = await _Order.GetOrderByIdAsync(request.Id);
			if (order == null)
			{
				return new ExecutionResult<bool>()
				{
					Data = false,
					Message = "OrderId not found",
					Status = ResponseStatus.BadRequest
				};
			}

			ProductOrder productOrder = await _Order.GetProductOrderByOrderIdAsync(order.Id);
			//error handling for product order

			if (productOrder == null)
			{
				return new ExecutionResult<bool>()
				{
					Data = false,
					Message = "ProductOrder not found for the given OrderId",
					Status = ResponseStatus.BadRequest
				};
			}
			productOrder.Quantity = request.Quantity;

			product productDetail = await _product.GetProductByIdAsync(productOrder.ProductId);
			//error handling for product
			if (productDetail == null)
			{
				return new ExecutionResult<bool>()
				{
					Data = false,
					Message = "Product not found for the given ProductId",
					Status = ResponseStatus.BadRequest
				};
			}
			order.OrderDate = DateTime.UtcNow;
			if (request.OrderStatus == OrderStatus.Canceled)
			{
				order.OrderStatus = (int)(OrderStatus)request.OrderStatus;
			}
			order.TotalAmount = productDetail?.Price ?? 0 * productOrder.Quantity;

			await _Order.UpdateOrderAsync(order);

			await _Order.UpdateProductOrderAsync(productOrder);
			return new ExecutionResult<bool>()
			{
				Data = true,
				Message = "UpdateSuccessful",
				Status = ResponseStatus.Ok
			};



		}

	}
}
