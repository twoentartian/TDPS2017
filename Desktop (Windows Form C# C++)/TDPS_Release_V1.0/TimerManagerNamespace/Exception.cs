using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimerManagerNamespace
{
	class NoGuidFoundException : ApplicationException
	{
		public NoGuidFoundException(string message)
		{
			Message = message;
		}

		public NoGuidFoundException()
		{
			
		}

		public override string Message { get; }
	}
}
