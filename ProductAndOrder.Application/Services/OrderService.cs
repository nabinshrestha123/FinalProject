// ❌ REMOVED this (not needed)
// using NPOI.OpenXmlFormats.Wordprocessing;

using ProductAndOrder.Application.DTO;
using ProductAndOrder.Application.Interfaces;
using ProductAndOrder.Application.Kafka.KafkaEntities;
using ProductAndOrder.Application.Kafka.Producer.ProducerInterface;
using ProductAndOrder.Application.Kafka.Topic;
using ProductAndOrder.Application.Services.Strategy_pattern;
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
		private readonly DiscountStrategyFactory _discountStrategyFactory;

		public OrderService(
			IOrder order,
			IUserServiceClient UserServiceClient,
			IKafkaProducer kafkaProducer,
			IProduct product,
			DiscountStrategyFactory discountStrategyFactory)
		{
			_Order = order;
			_UserServiceClient = UserServiceClient;
			_KafkaProducr = kafkaProducer;
			_product = product;
			_discountStrategyFactory = discountStrategyFactory;
		}

		// ================= GET ALL =================
		public async Task<IEnumerable<OrderDto>> GetAllOrderAsync()
		{
			var orders = await _Order.GetAllOrderAsync();
			var products = await _product.GetAllProductsAsync();
			var productOrders = await _Order.GetAllProductOrder();

			var result =
			(from o in orders
			 join po in productOrders on o.Id equals po.OrderId
			 join p in products on po.ProductId equals p.Id
			 select new OrderDto
			 {
				 Id = o.Id,
				 ProductName = p.ProductName,
				 OrderDate = o.OrderDate,
				 OrderStatus = (OrderStatus)o.OrderStatus,
				 TotalAmount = o.TotalAmount,
				 Quantity = po.Quantity,
				 OrderStatusInfo = ((OrderStatus)o.OrderStatus).ToString(),
				 SubTotal = o.SubTotal,              // ✅ ADDED
				 DiscountAmount = o.DiscountAmount   // ✅ ADDED
			 }).ToList();

			return result;
		}

		// ================= GET BY ID =================
		public async Task<ExecutionResult<OrderDto>> GetOrderByIdAsync(int orderId)
		{
			var order = await _Order.GetOrderByIdAsync(orderId);

			if (order == null)
			{
				return new ExecutionResult<OrderDto>
				{
					Data = null,
					Message = "OrderId not found",
					Status = ResponseStatus.BadRequest
				};
			}

			var productOrder = await _Order.GetProductOrderByOrderIdAsync(orderId);
			var product = await _product.GetProductByIdAsync(productOrder.ProductId);
			var user = await _UserServiceClient.GetUserAsync(order.UserId);

			var result = new OrderDto
			{
				Id = order.Id,
				OrderDate = order.OrderDate,
				OrderStatus = (OrderStatus)order.OrderStatus,
				TotalAmount = order.TotalAmount,
				CustomerName = user?.Name ?? "-",
				ProductName = product?.ProductName,
				OrderStatusInfo = ((OrderStatus)order.OrderStatus).ToString(),
				SubTotal = order.SubTotal,            // ✅ ADDED
				DiscountAmount = order.DiscountAmount // ✅ ADDED
			};

			return new ExecutionResult<OrderDto>
			{
				Data = result,
				Message = "Order found",
				Status = ResponseStatus.Ok
			};
		}

		// ================= ADD ORDER =================
		public async Task<ExecutionResult<int>> AddOrderAsync(CreateOrderDto createorder, int actionBy)
		{
			var product = await _product.GetProductByIdAsync(createorder.ProductId);

			if (product == null)
			{
				return new ExecutionResult<int>
				{
					Data = 0,
					Message = "Product not found",
					Status = ResponseStatus.BadRequest
				};
			}

			// ✅ FIXED NULL ISSUE
			decimal subTotal = (product.Price ?? 0) * createorder.Quantity;

			// ✅ STRATEGY PATTERN USED
			var strategy = _discountStrategyFactory.GetStrategy(
				createorder.DiscountType,
				createorder.DiscountValue
			);

			

			decimal finalTotal = strategy.ApplyDiscount(subTotal);

			if (finalTotal < 0)
				finalTotal = 0;
			decimal discountAmount = subTotal - finalTotal;

			var order = new Order
			{
				UserId = actionBy,
				DiscountType = createorder.DiscountType.ToString(),
				SubTotal = subTotal,
				DiscountAmount = discountAmount,
				TotalAmount = finalTotal,
				OrderStatus = (int)OrderStatus.Pending,
				OrderDate = DateTime.UtcNow
			};

			var createdOrder = await _Order.AddOrderAsync(order);

			var productOrder = new ProductOrder
			{
				OrderId = createdOrder.Id,
				ProductId = createorder.ProductId,
				Quantity = createorder.Quantity,
				UnitPrice = product.Price ?? 0,
				TotalPrice = subTotal
			};

			await _Order.AddProductOrderAsync(productOrder);

			// ✅ OPTIONAL Kafka
			var orderCreatedEvent = new OrderCreatedEvent
			{
				Message = $"Order {createdOrder.Id} created",
				UserId = actionBy
			};

			await _KafkaProducr.ProducerAsync(
				KafkaTopics.OrderCreated,
				createdOrder.UserId.ToString(),
				orderCreatedEvent
			);

			return new ExecutionResult<int>
			{
				Data = createdOrder.Id,
				Message = "Order created successfully",
				Status = ResponseStatus.Ok
			};
		}

		// ================= DELETE =================
		public async Task<bool> DeleteOrderAsync(int id)
		{
			var order = await _Order.GetOrderByIdAsync(id);

			if (order == null)
				return false;

			await _Order.DeleteOrderAsync(order);
			return true;
		}

		// ================= UPDATE =================
		public async Task<ExecutionResult<bool>> UpdateOrderAsync(UpdateOrderDto request)
		{
			var order = await _Order.GetOrderByIdAsync(request.Id);

			if (order == null)
			{
				return new ExecutionResult<bool>
				{
					Data = false,
					Message = "Order not found",
					Status = ResponseStatus.BadRequest
				};
			}

			var productOrder = await _Order.GetProductOrderByOrderIdAsync(order.Id);

			if (productOrder == null)
			{
				return new ExecutionResult<bool>
				{
					Data = false,
					Message = "ProductOrder not found",
					Status = ResponseStatus.BadRequest
				};
			}

			var product = await _product.GetProductByIdAsync(productOrder.ProductId);

			if (product == null)
			{
				return new ExecutionResult<bool>
				{
					Data = false,
					Message = "Product not found",
					Status = ResponseStatus.BadRequest
				};
			}

			productOrder.Quantity = request.Quantity;

			// ✅ FIXED CALCULATION BUG
			order.TotalAmount = (product.Price ?? 0) * productOrder.Quantity;

			order.OrderDate = DateTime.UtcNow;

			if (request.OrderStatus == OrderStatus.Canceled)
				order.OrderStatus = (int)request.OrderStatus;

			await _Order.UpdateOrderAsync(order);
			await _Order.UpdateProductOrderAsync(productOrder);

			return new ExecutionResult<bool>
			{
				Data = true,
				Message = "Updated successfully",
				Status = ResponseStatus.Ok
			};
		}
	}
}