using System;
using System.Collections.Generic;
using System.Text;
using ProductAndOrder.Application.Interfaces.Strategy_pattern;

namespace ProductAndOrder.Application.Services.Strategy_pattern
{
	public class PercentageDiscountStrategy : IDiscountStrategy

	{

		private readonly decimal _discountPercentage;
		public PercentageDiscountStrategy(decimal discountPercentage)
		{
			_discountPercentage = discountPercentage;
		}
		public decimal ApplyDiscount(decimal subTotal)
		{
			return subTotal - (subTotal * _discountPercentage / 100);
		}
	}	
	
}
