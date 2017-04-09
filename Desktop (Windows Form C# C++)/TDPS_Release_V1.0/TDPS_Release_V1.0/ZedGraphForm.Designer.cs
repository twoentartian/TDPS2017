namespace TDPS_Release_V1._0
{
	partial class ZedGraphForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.zedGraphTable = new ZedGraph.ZedGraphControl();
			this.comboBoxDataSelect = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// zedGraphTable
			// 
			this.zedGraphTable.Location = new System.Drawing.Point(12, 41);
			this.zedGraphTable.Name = "zedGraphTable";
			this.zedGraphTable.ScrollGrace = 0D;
			this.zedGraphTable.ScrollMaxX = 0D;
			this.zedGraphTable.ScrollMaxY = 0D;
			this.zedGraphTable.ScrollMaxY2 = 0D;
			this.zedGraphTable.ScrollMinX = 0D;
			this.zedGraphTable.ScrollMinY = 0D;
			this.zedGraphTable.ScrollMinY2 = 0D;
			this.zedGraphTable.Size = new System.Drawing.Size(885, 423);
			this.zedGraphTable.TabIndex = 0;
			// 
			// comboBoxDataSelect
			// 
			this.comboBoxDataSelect.FormattingEnabled = true;
			this.comboBoxDataSelect.Location = new System.Drawing.Point(12, 13);
			this.comboBoxDataSelect.Name = "comboBoxDataSelect";
			this.comboBoxDataSelect.Size = new System.Drawing.Size(885, 21);
			this.comboBoxDataSelect.TabIndex = 1;
			this.comboBoxDataSelect.SelectedIndexChanged += new System.EventHandler(this.comboBoxDataSelect_SelectedIndexChanged);
			// 
			// ZedGraphForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(909, 477);
			this.Controls.Add(this.comboBoxDataSelect);
			this.Controls.Add(this.zedGraphTable);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.Name = "ZedGraphForm";
			this.Text = "ZedGraph";
			this.ResumeLayout(false);

		}

		#endregion

		private ZedGraph.ZedGraphControl zedGraphTable;
		private System.Windows.Forms.ComboBox comboBoxDataSelect;
	}
}