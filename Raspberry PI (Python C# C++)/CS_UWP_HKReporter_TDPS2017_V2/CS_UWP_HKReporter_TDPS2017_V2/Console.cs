using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace CS_UWP_HKRepoter_TDPS2017_Console
{
	class Console
	{
		#region Singleton

		protected Console()
		{

		}

		private static Console _instance;

		public static Console GetInstance()
		{
			if (_instance == null)
			{
				_instance = new Console();
			}
			return _instance;
		}

		#endregion

		private TextBlock _consoleBox;

		public TextBlock ConsoleBox
		{
			get { return _consoleBox; }
			set { _consoleBox = value; }
		}

		public void Init()
		{
			Clear();
			Display();
		}

		public void Clear()
		{
			_consoleBox.Text = String.Empty;
		}

		public void Display()
		{
			_consoleBox.Text = "TDPS 2017 Console:";
		}

		public void Display(string arg)
		{
			_consoleBox.Text = "Console >>> " + arg;
		}
	}
}
