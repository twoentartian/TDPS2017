using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpUdpManagerNamespace
{
	class UdpManagerNotInitializeException : ApplicationException
	{
		public UdpManagerNotInitializeException(string message)
		{
			Message = message;
		}

		public override string Message { get; }
	}

	class NoVaildIpV4AddressException : ApplicationException
	{
		public NoVaildIpV4AddressException(string message)
		{
			Message = message;
		}

		public override string Message { get; }
	}

	class MultiIpV4AddressException : ApplicationException
	{
		public MultiIpV4AddressException(string message)
		{
			Message = message;
		}

		public override string Message { get; }
	}

	class TcpManagerNotInitializeException : ApplicationException
	{
		public TcpManagerNotInitializeException(string message)
		{
			Message = message;
		}

		public override string Message { get; }
	}

	class NotEnoughTcpServerCapacityException : ApplicationException
	{
		public NotEnoughTcpServerCapacityException(string message)
		{
			Message = message;
		}

		public override string Message { get; }
	}

	class ConnectionLostException : ApplicationException
	{
		public ConnectionLostException(string message)
		{
			Message = message;
		}

		public override string Message { get; }
	}
}
