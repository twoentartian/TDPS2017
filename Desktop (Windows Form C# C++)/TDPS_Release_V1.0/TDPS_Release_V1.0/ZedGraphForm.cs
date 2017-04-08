using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

namespace TDPS_Release_V1._0
{
	public partial class ZedGraphForm : Form
	{
		/// <summary>
		/// If the StorageState is changed, remeber to change the FlashData().
		/// </summary>
		private enum StorageState
		{
			Null, Array, List
		};

		private readonly StorageState _nowState;
		private readonly double[,] _dataArray;
		private readonly List<double[]> _dataList;
		private readonly GraphPane _myPane;

		public ZedGraphForm(int[] argData)
		{
			InitializeComponent();
			double[,] newData = new double[1, argData.Length];
			for (int i = 0; i < argData.Length; i++)
			{
				newData[0, i] = argData[i];
			}

			_nowState = StorageState.Array;
			for (int i = 0; i < newData.GetLength(0); i++)
			{
				comboBoxDataSelect.Items.Add(string.Format("Data Set: {0:D}", i));
			}
			_dataArray = newData;
			_myPane = zedGraphTable.GraphPane;
			_myPane.Title.Text = "X - Y";
			_myPane.XAxis.Title.Text = "Point";
			_myPane.YAxis.Title.Text = "Value";
			if (comboBoxDataSelect.Items.Count == 1)
			{
				comboBoxDataSelect.SelectedIndex = 0;
			}
		}

		public ZedGraphForm(float[] argData)
		{
			InitializeComponent();
			double[,] newData = new double[1, argData.Length];
			for (int i = 0; i < argData.Length; i++)
			{
				newData[0, i] = argData[i];
			}

			_nowState = StorageState.Array;
			for (int i = 0; i < newData.GetLength(0); i++)
			{
				comboBoxDataSelect.Items.Add(string.Format("Data Set: {0:D}", i));
			}
			_dataArray = newData;
			_myPane = zedGraphTable.GraphPane;
			_myPane.Title.Text = "X - Y";
			_myPane.XAxis.Title.Text = "Point";
			_myPane.YAxis.Title.Text = "Value";
			if (comboBoxDataSelect.Items.Count == 1)
			{
				comboBoxDataSelect.SelectedIndex = 0;
			}
		}

		public ZedGraphForm(double[] argData)
		{
			InitializeComponent();
			double[,] newData = new double[1,argData.Length];
			for (int i = 0; i < argData.Length; i++)
			{
				newData[0, i] = argData[i];
			}

			_nowState = StorageState.Array;
			for (int i = 0; i < newData.GetLength(0); i++)
			{
				comboBoxDataSelect.Items.Add(string.Format("Data Set: {0:D}", i));
			}
			_dataArray = newData;
			_myPane = zedGraphTable.GraphPane;
			_myPane.Title.Text = "X - Y";
			_myPane.XAxis.Title.Text = "Point";
			_myPane.YAxis.Title.Text = "Value";
			if (comboBoxDataSelect.Items.Count == 1)
			{
				comboBoxDataSelect.SelectedIndex = 0;
			}
		}

		public ZedGraphForm(double[,] argData)
		{
			InitializeComponent();

			_nowState = StorageState.Array;
			for (int i = 0; i < argData.GetLength(0); i++)
			{
				comboBoxDataSelect.Items.Add(string.Format("Data Set: {0:D}", i));
			}
			_dataArray = argData;
			_myPane = zedGraphTable.GraphPane;
			_myPane.Title.Text = "X - Y";
			_myPane.XAxis.Title.Text = "Point";
			_myPane.YAxis.Title.Text = "Value";
			if (comboBoxDataSelect.Items.Count == 1)
			{
				comboBoxDataSelect.SelectedIndex = 0;
			}
		}

		public ZedGraphForm(List<double[]> argData)
		{
			InitializeComponent();

			_nowState = StorageState.List;
			for (int i = 0; i < argData.Count; i++)
			{
				comboBoxDataSelect.Items.Add(string.Format("Data Set: {0:D}", i));
			}
			_dataList = argData;
			_myPane = zedGraphTable.GraphPane;
			_myPane.Title.Text = "X - Y";
			_myPane.XAxis.Title.Text = "Point";
			_myPane.YAxis.Title.Text = "Value";
			if (comboBoxDataSelect.Items.Count == 1)
			{
				comboBoxDataSelect.SelectedIndex = 0;
			}
		}

		private void comboBoxDataSelect_SelectedIndexChanged(object sender, EventArgs e)
		{
			FlashData();
		}

		#region Func

		private void ClearGraph()
		{
			for (int i = 0; i < _myPane.CurveList.Count; i++)
			{
				_myPane.CurveList[i].Clear();
			}
		}

		private void FlashData()
		{
			PointPairList list = new PointPairList();
			if (_nowState == StorageState.Array)
			{
				int selectedValue = comboBoxDataSelect.SelectedIndex;
				for (int i = 0; i < _dataArray.GetLength(1); i++)
				{
					list.Add(i, _dataArray[selectedValue, i]);
				}
			}
			else if (_nowState == StorageState.List)
			{
				int selectedValue = comboBoxDataSelect.SelectedIndex;
				double[] value = _dataList[selectedValue];
				for (int i = 0; i < value.Length; i++)
				{
					list.Add(i, value[i]);
				}
			}
			else
			{
				
			}
			ClearGraph();
			LineItem myCurve = _myPane.AddCurve(String.Empty, list, Color.Blue, SymbolType.Circle);
			_myPane.AxisChange();
			Refresh();
		}
		#endregion
	}
}
