using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialPortNamespace
{
	class NotEnoughPortArrayException : ApplicationException
	{
		public NotEnoughPortArrayException(string message)
		{
			Message = message;
		}

		public override string Message { get; }
	}
}
