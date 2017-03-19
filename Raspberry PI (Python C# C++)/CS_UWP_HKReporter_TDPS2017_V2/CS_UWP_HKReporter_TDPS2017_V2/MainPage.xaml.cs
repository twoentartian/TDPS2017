using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CS_UWP_HKReporter_TDPS2017_V2
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		public MainPage()
		{
			this.InitializeComponent();
		}

		private TcpIpManager _tcpIpManager;
		private Console _console;
		private Camera _camera;

		private async void mainPage_Loaded(object sender, RoutedEventArgs e)
		{
			_console = Console.GetInstance();
			_console.ConsoleBox = _textBoxConsole;
			_console.Init();

			SerialRaspberry raspberrySerial = SerialRaspberry.GetInstance();
			await raspberrySerial.Init("UART0");

			_camera = Camera.GetInstance();
			_tcpIpManager = TcpIpManager.GetInstance();
			while (true)
			{
				try
				{
					_captureElement.Source = _camera.CameraCapture;
					break;
				}
				catch (COMException exception)
				{
					await Task.Delay(1);
				}
			}
			
			_camera.StartSendingServiceAsync();
			
		}

		private void mainPage_Unloaded(object sender, RoutedEventArgs e)
		{
			_camera.StopSendingServiceAsync();
			Application.Current.Exit();
		}
	}
}
