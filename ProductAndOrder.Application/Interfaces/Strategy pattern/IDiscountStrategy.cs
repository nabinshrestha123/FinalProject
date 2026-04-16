using System;
using System.Collections.Generic;
using System.Text;

namespace ProductAndOrder.Application.Interfaces.Strategy_pattern
{
	public interface IDiscountStrategy
	{
		decimal ApplyDiscount(decimal subTotal);

	}
}
