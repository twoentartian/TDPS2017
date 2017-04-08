using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDPS_Release_V1._0
{
	class LogicErrorException : ApplicationException
	{

	}

	class MultiIpV4AddressException: ApplicationException
	{
		
	}

	class NoClientConnected : ApplicationException
	{
		
	}
}
