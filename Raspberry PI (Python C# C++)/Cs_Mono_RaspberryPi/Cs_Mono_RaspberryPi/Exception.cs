using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs_Mono_RaspberryPi
{
	class LogicErrorException : ApplicationException
	{
		public LogicErrorException(string message)
		{
			Message = message;
		}

		public override string Message { get; }

	}
}
