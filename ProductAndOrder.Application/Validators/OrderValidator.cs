using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using ProductAndOrder.Application.DTO;

namespace ProductAndOrder.Application.Validators
{
	public class OrderValidator : AbstractValidator<CreateOrderDto>
	{
		public OrderValidator()
		{
			RuleFor(x => x.UserId)
				.GreaterThan(0).WithMessage("UserID is required to know its order.");

			RuleFor(t => t.TotalAmount)
				.GreaterThan(0).WithMessage("TotalAmount is required to know total payment.");
		}
	}
}
