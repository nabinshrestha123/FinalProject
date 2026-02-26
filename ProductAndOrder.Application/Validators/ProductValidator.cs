using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using ProductAndOrder.Application.DTO;

namespace ProductAndOrder.Application.Validators
{
	public class ProductValidator : AbstractValidator<CreateProductDto>

	{
	 public ProductValidator()
		{
			RuleFor(x => x.ProductName)
			.NotEmpty().WithMessage("Product Name is required.")
			.MaximumLength(100).WithMessage("Product Name cannot exceed 100 characters.")
			.MinimumLength(2).WithMessage("Product Name must be at least 2 characters long.");


			RuleFor(x => x.Price)
			.GreaterThan(0).WithMessage("Price of Product is required.");
			

		}
	}
}
