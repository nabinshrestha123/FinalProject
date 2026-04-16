using System;
using System.Collections.Generic;
using System.Text;
using ProductAndOrder.Application.Interfaces.Strategy_pattern;

namespace ProductAndOrder.Application.Services.Strategy_pattern
{
	public class FlatDiscountStrategy: IDiscountStrategy
	{
		private readonly decimal _amount;

		public FlatDiscountStrategy(decimal amount)
		{
			_amount = amount;
		}

		public decimal ApplyDiscount(decimal subTotal)
		{
			return subTotal - _amount;
		}
	}
}
