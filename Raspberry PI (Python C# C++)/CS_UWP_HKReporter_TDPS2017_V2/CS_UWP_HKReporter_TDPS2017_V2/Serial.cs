using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;

namespace CS_UWP_HKReporter_TDPS2017_V2
{
	class SerialRaspberry
	{
		#region Singleton

		public static SerialRaspberry GetInstance()
		{
			return Instance ?? (Instance = new SerialRaspberry());
		}

		protected static SerialRaspberry Instance;

		private SerialRaspberry()
		{
			
		}

		#endregion

		private SerialDevice _serialPort;
		private Timer _readTimer;

		public async Task Init(string argPort)
		{
			string aqs = SerialDevice.GetDeviceSelector(argPort);
			var dis = await DeviceInformation.FindAllAsync(aqs);
			_serialPort = await SerialDevice.FromIdAsync(dis[0].Id);

			/* Configure serial settings */
			_serialPort.ReadTimeout = TimeSpan.FromMilliseconds(1000);
			_serialPort.WriteTimeout = TimeSpan.FromMilliseconds(1000);
			_serialPort.BaudRate = 115200;
			_serialPort.Parity = SerialParity.None;
			_serialPort.StopBits = SerialStopBitCount.One;
			_serialPort.DataBits = 8;
		}



		public async void Write(string argData)
		{
			if (_serialPort == null)
			{
				throw new SerialPortNotInitException();
			}
			DataWriter dataWriter = new DataWriter();
			dataWriter.WriteString(argData);
			await _serialPort.OutputStream.WriteAsync(dataWriter.DetachBuffer());
		}

		public async void Write(byte[] argData)
		{
			if (_serialPort == null)
			{
				throw new SerialPortNotInitException();
			}
			DataWriter dataWriter = new DataWriter();
			dataWriter.WriteBytes(argData);
			await _serialPort.OutputStream.WriteAsync(dataWriter.DetachBuffer());
		}

		private async Task<string> Read()
		{
			const uint maxReadLength = 1024;
			DataReader dataReader = new DataReader(_serialPort.InputStream);
			uint bytesToRead = await dataReader.LoadAsync(maxReadLength);
			return dataReader.ReadString(bytesToRead);
		}
	}
}
