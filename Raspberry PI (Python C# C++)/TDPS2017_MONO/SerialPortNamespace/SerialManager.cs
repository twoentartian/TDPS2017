using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialPortNamespace
{
	public sealed class SerialManager
	{
		#region Singleton

		private static SerialManager _instance;

		private SerialManager()
		{
			Init();
		}

		public static SerialManager GetInstance()
		{
			return _instance ?? (_instance = new SerialManager());
		}

		#endregion

		public static string[] GetAllVaildPorts()
		{
			return SerialPort.GetPortNames();
		}

		public class SerialPortWithGuid
		{
			public SerialPortWithGuid()
			{
				Guid = Guid.Empty;
				Serial = null;
				Occupied = false;
			}

			public Guid Guid;
			public SerialPort Serial;
			public bool Occupied;

			/// <summary>
			/// Read vaild data
			/// </summary>
			/// <returns></returns>
			public byte[] Read()
			{
				int length = Serial.BytesToRead;
				byte[] data = new byte[length];
				Serial.Read(data, 0, length);
				return data;
			}

			/// <summary>
			/// Send data.
			/// </summary>
			/// <param name="argBytes"></param>
			public void Send(byte[] argBytes)
			{
				Serial.Write(argBytes, 0, argBytes.Length);
			}

			/// <summary>
			/// Send data.
			/// </summary>
			/// <param name="argString"></param>
			public void Send(string argString)
			{
				Serial.Write(argString);
			}
		}

		/// <summary>
		/// Init all the serial instances in the target array.
		/// </summary>
		public void Init()
		{
			for (int i = 0; i < _serialPortArray.Length; i++)
			{
				if (_serialPortArray[i] == null)
				{
					_serialPortArray[i] = new SerialPortWithGuid();
				}
			}
		}

		/// <summary>
		/// Store all the ports in an array.
		/// </summary>
		private readonly SerialPortWithGuid[] _serialPortArray = new SerialPortWithGuid[5];

		/// <summary>
		/// Get the free port. (Not occupied)
		/// </summary>
		/// <returns></returns>
		private SerialPortWithGuid GetFreePort()
		{
			foreach (var port in _serialPortArray)
			{
				if (port.Occupied == false)
				{
					return port;
				}
			}
			throw new NotEnoughPortArrayException("Please add port array length in library");
		}

		/// <summary>
		/// Get port according to port GUID.
		/// </summary>
		/// <param name="argGuid"></param>
		/// <returns></returns>
		public SerialPortWithGuid GetPort(Guid argGuid)
		{
			foreach (var port in _serialPortArray)
			{
				if (port.Guid == argGuid)
				{
					return port;
				}
			}
			throw new ArgumentException("Cannot find vaild port with GUID: " + argGuid);
		}

		/// <summary>
		/// Find port according to port name.
		/// </summary>
		/// <param name="argPortName"></param>
		/// <returns></returns>
		public SerialPortWithGuid GetPort(string argPortName)
		{
			foreach (var port in _serialPortArray)
			{
				if (port.Occupied)
				{
					if (port.Serial.PortName == argPortName)
					{
						return port;
					}
				}
			}
			throw new ArgumentException("Cannot find vaild port with name: " + argPortName);
		}

		/// <summary>
		/// Occupy a port and use it to communicate.
		/// </summary>
		/// <param name="argPortName"></param>
		/// <param name="argBaudRate"></param>
		/// <param name="argParity"></param>
		/// <param name="argHandler"></param>
		/// <returns></returns>
		public SerialPortWithGuid Add(string argPortName, int argBaudRate, Parity argParity, SerialDataReceivedEventHandler argHandler)
		{
			SerialPortWithGuid tempSerialPortWithGuid = GetFreePort();
			tempSerialPortWithGuid.Occupied = true;
			tempSerialPortWithGuid.Guid = Guid.NewGuid();
			tempSerialPortWithGuid.Serial = new SerialPort(argPortName, argBaudRate, argParity);
			tempSerialPortWithGuid.Serial.Open();
			tempSerialPortWithGuid.Serial.DataReceived += argHandler;
			return tempSerialPortWithGuid;
		}

		/// <summary>
		/// Send data according to GUID
		/// </summary>
		/// <param name="argGuid"></param>
		/// <param name="argBytes"></param>
		public void Send(Guid argGuid, byte[] argBytes)
		{
			GetPort(argGuid).Send(argBytes);
		}

		/// <summary>
		/// Send data according to GUID
		/// </summary>
		/// <param name="argGuid"></param>
		/// <param name="argString"></param>
		public void Send(Guid argGuid, string argString)
		{
			GetPort(argGuid).Send(argString);
		}

		/// <summary>
		/// Send data according to port name
		/// </summary>
		/// <param name="argportName"></param>
		/// <param name="argBytes"></param>
		public void Send(string argportName, byte[] argBytes)
		{
			GetPort(argportName).Send(argBytes);
		}

		/// <summary>
		/// Send data according to port name
		/// </summary>
		/// <param name="argportName"></param>
		/// <param name="argString"></param>
		public void Send(string argportName, string argString)
		{
			GetPort(argportName).Send(argString);
		}

		/// <summary>
		/// Close a port by SerialPortWithGuid.
		/// </summary>
		/// <param name="argSerialPortWithGuid"></param>
		public void Close(SerialPortWithGuid argSerialPortWithGuid)
		{
			argSerialPortWithGuid.Guid = Guid.Empty;
			argSerialPortWithGuid.Occupied = false;
			argSerialPortWithGuid.Serial.Close();
			argSerialPortWithGuid.Serial.Dispose();
			argSerialPortWithGuid.Serial = null;
		}

		/// <summary>
		/// Close a port by GUID.
		/// </summary>
		/// <param name="argGuid"></param>
		public void Close(Guid argGuid)
		{
			Close(GetPort(argGuid));
		}

		/// <summary>
		/// Close a port by PortName.
		/// </summary>
		/// <param name="argPortName"></param>
		public void Close(string argPortName)
		{
			Close(GetPort(argPortName));
		}

		/// <summary>
		/// Close all ports.
		/// </summary>
		public void CloseAll()
		{
			foreach (SerialPortWithGuid serialPortWithGuid in _serialPortArray)
			{
				Close(serialPortWithGuid);
			}
		}
	}
}
