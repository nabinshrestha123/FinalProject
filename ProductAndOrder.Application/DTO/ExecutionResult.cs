using System;
using System.Collections.Generic;
using System.Text;
using NPOI.SS.Formula.Functions;

namespace ProductAndOrder.Application.DTO
{
	public class ExecutionResult<T>
	{

		public ResponseStatus Status { get; set; }

		public string Message { get; set; }

		public T Data { get; set; }

	}
		public enum ResponseStatus
		{
			Ok = 200,
			BadRequest = 400,
			UnHandleExpection = 500
		}
	
}