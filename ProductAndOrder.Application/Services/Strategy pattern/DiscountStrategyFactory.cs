using System;
using System.Collections.Generic;
using System.Text;
using ProductAndOrder.Application.DTO;
using ProductAndOrder.Application.Interfaces.Strategy_pattern;


namespace ProductAndOrder.Application.Services.Strategy_pattern
{
	public class DiscountStrategyFactory
	{
		public IDiscountStrategy GetStrategy(DiscountType type, decimal value)
		{
			return type switch
			{
				DiscountType.Percentage => new PercentageDiscountStrategy(value),
				DiscountType.Flat => new FlatDiscountStrategy(value),
				_ => new NoDiscountStrategy()
			};
		}
	}
}
