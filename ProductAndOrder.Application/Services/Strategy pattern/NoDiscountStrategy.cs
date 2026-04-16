using System;
using System.Collections.Generic;
using System.Text;
using ProductAndOrder.Application.Interfaces.Strategy_pattern;

namespace ProductAndOrder.Application.Services.Strategy_pattern
{
	public class NoDiscountStrategy : IDiscountStrategy
	{
		public decimal ApplyDiscount(decimal subTotal)
		{
			return subTotal;
		}
	}
}
