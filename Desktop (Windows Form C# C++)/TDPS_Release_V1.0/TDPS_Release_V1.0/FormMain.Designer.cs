namespace TDPS_Release_V1._0
{
	partial class FormMain
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
			this.pictureBoxRaw = new System.Windows.Forms.PictureBox();
			this.pictureBoxOutput1 = new System.Windows.Forms.PictureBox();
			this.pictureBoxOutput2 = new System.Windows.Forms.PictureBox();
			this.textBoxConsole = new System.Windows.Forms.TextBox();
			this.buttonSample = new System.Windows.Forms.Button();
			this.buttonAutoSample = new System.Windows.Forms.Button();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.applicationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.arduinoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.arduinoControlPanelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.taskToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.groundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.Menu_Ground1_Task1 = new System.Windows.Forms.ToolStripMenuItem();
			this.Menu_Ground1_Task2 = new System.Windows.Forms.ToolStripMenuItem();
			this.ground1Task2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.Menu_Ground2_Task1 = new System.Windows.Forms.ToolStripMenuItem();
			this.Menu_Ground2_Task2 = new System.Windows.Forms.ToolStripMenuItem();
			this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.debug1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxRaw)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxOutput1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxOutput2)).BeginInit();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// pictureBoxRaw
			// 
			this.pictureBoxRaw.BackColor = System.Drawing.SystemColors.Control;
			this.pictureBoxRaw.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBoxRaw.Location = new System.Drawing.Point(12, 54);
			this.pictureBoxRaw.Name = "pictureBoxRaw";
			this.pictureBoxRaw.Size = new System.Drawing.Size(301, 189);
			this.pictureBoxRaw.TabIndex = 0;
			this.pictureBoxRaw.TabStop = false;
			// 
			// pictureBoxOutput1
			// 
			this.pictureBoxOutput1.BackColor = System.Drawing.SystemColors.Control;
			this.pictureBoxOutput1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBoxOutput1.Location = new System.Drawing.Point(319, 54);
			this.pictureBoxOutput1.Name = "pictureBoxOutput1";
			this.pictureBoxOutput1.Size = new System.Drawing.Size(301, 189);
			this.pictureBoxOutput1.TabIndex = 1;
			this.pictureBoxOutput1.TabStop = false;
			// 
			// pictureBoxOutput2
			// 
			this.pictureBoxOutput2.BackColor = System.Drawing.SystemColors.Control;
			this.pictureBoxOutput2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBoxOutput2.Location = new System.Drawing.Point(626, 54);
			this.pictureBoxOutput2.Name = "pictureBoxOutput2";
			this.pictureBoxOutput2.Size = new System.Drawing.Size(301, 189);
			this.pictureBoxOutput2.TabIndex = 2;
			this.pictureBoxOutput2.TabStop = false;
			// 
			// textBoxConsole
			// 
			this.textBoxConsole.Location = new System.Drawing.Point(933, 54);
			this.textBoxConsole.Multiline = true;
			this.textBoxConsole.Name = "textBoxConsole";
			this.textBoxConsole.ReadOnly = true;
			this.textBoxConsole.Size = new System.Drawing.Size(498, 496);
			this.textBoxConsole.TabIndex = 3;
			// 
			// buttonSample
			// 
			this.buttonSample.Location = new System.Drawing.Point(12, 249);
			this.buttonSample.Name = "buttonSample";
			this.buttonSample.Size = new System.Drawing.Size(75, 23);
			this.buttonSample.TabIndex = 4;
			this.buttonSample.Text = "Sample";
			this.buttonSample.UseVisualStyleBackColor = true;
			this.buttonSample.Click += new System.EventHandler(this.buttonSample_Click);
			// 
			// buttonAutoSample
			// 
			this.buttonAutoSample.Location = new System.Drawing.Point(93, 249);
			this.buttonAutoSample.Name = "buttonAutoSample";
			this.buttonAutoSample.Size = new System.Drawing.Size(122, 23);
			this.buttonAutoSample.TabIndex = 5;
			this.buttonAutoSample.Text = "Auto Sample: off";
			this.buttonAutoSample.UseVisualStyleBackColor = true;
			this.buttonAutoSample.Click += new System.EventHandler(this.buttonAutoSample_Click);
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.applicationToolStripMenuItem,
            this.arduinoToolStripMenuItem,
            this.taskToolStripMenuItem,
            this.debugToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(1443, 24);
			this.menuStrip1.TabIndex = 7;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// applicationToolStripMenuItem
			// 
			this.applicationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
			this.applicationToolStripMenuItem.Name = "applicationToolStripMenuItem";
			this.applicationToolStripMenuItem.Size = new System.Drawing.Size(80, 20);
			this.applicationToolStripMenuItem.Text = "Application";
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
			this.exitToolStripMenuItem.Text = "Exit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// arduinoToolStripMenuItem
			// 
			this.arduinoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.arduinoControlPanelToolStripMenuItem});
			this.arduinoToolStripMenuItem.Name = "arduinoToolStripMenuItem";
			this.arduinoToolStripMenuItem.Size = new System.Drawing.Size(62, 20);
			this.arduinoToolStripMenuItem.Text = "Arduino";
			// 
			// arduinoControlPanelToolStripMenuItem
			// 
			this.arduinoControlPanelToolStripMenuItem.Name = "arduinoControlPanelToolStripMenuItem";
			this.arduinoControlPanelToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
			this.arduinoControlPanelToolStripMenuItem.Text = "Arduino Control Panel";
			this.arduinoControlPanelToolStripMenuItem.Click += new System.EventHandler(this.arduinoControlPanelToolStripMenuItem_Click);
			// 
			// taskToolStripMenuItem
			// 
			this.taskToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.groundToolStripMenuItem,
            this.ground1Task2ToolStripMenuItem});
			this.taskToolStripMenuItem.Name = "taskToolStripMenuItem";
			this.taskToolStripMenuItem.Size = new System.Drawing.Size(42, 20);
			this.taskToolStripMenuItem.Text = "Task";
			// 
			// groundToolStripMenuItem
			// 
			this.groundToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Menu_Ground1_Task1,
            this.Menu_Ground1_Task2});
			this.groundToolStripMenuItem.Name = "groundToolStripMenuItem";
			this.groundToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.groundToolStripMenuItem.Text = "Ground 1";
			// 
			// Menu_Ground1_Task1
			// 
			this.Menu_Ground1_Task1.Name = "Menu_Ground1_Task1";
			this.Menu_Ground1_Task1.Size = new System.Drawing.Size(106, 22);
			this.Menu_Ground1_Task1.Text = "Task 1";
			this.Menu_Ground1_Task1.Click += new System.EventHandler(this.Menu_Ground1_Task1_Click);
			// 
			// Menu_Ground1_Task2
			// 
			this.Menu_Ground1_Task2.Name = "Menu_Ground1_Task2";
			this.Menu_Ground1_Task2.Size = new System.Drawing.Size(106, 22);
			this.Menu_Ground1_Task2.Text = "Task 2";
			this.Menu_Ground1_Task2.Click += new System.EventHandler(this.Menu_Ground1_Task2_Click);
			// 
			// ground1Task2ToolStripMenuItem
			// 
			this.ground1Task2ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Menu_Ground2_Task1,
            this.Menu_Ground2_Task2});
			this.ground1Task2ToolStripMenuItem.Name = "ground1Task2ToolStripMenuItem";
			this.ground1Task2ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.ground1Task2ToolStripMenuItem.Text = "Ground 2";
			// 
			// Menu_Ground2_Task1
			// 
			this.Menu_Ground2_Task1.Name = "Menu_Ground2_Task1";
			this.Menu_Ground2_Task1.Size = new System.Drawing.Size(106, 22);
			this.Menu_Ground2_Task1.Text = "Task 1";
			this.Menu_Ground2_Task1.Click += new System.EventHandler(this.Menu_Ground2_Task1_Click);
			// 
			// Menu_Ground2_Task2
			// 
			this.Menu_Ground2_Task2.Name = "Menu_Ground2_Task2";
			this.Menu_Ground2_Task2.Size = new System.Drawing.Size(106, 22);
			this.Menu_Ground2_Task2.Text = "Task 2";
			this.Menu_Ground2_Task2.Click += new System.EventHandler(this.Menu_Ground2_Task2_Click);
			// 
			// debugToolStripMenuItem
			// 
			this.debugToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.debug1ToolStripMenuItem});
			this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
			this.debugToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
			this.debugToolStripMenuItem.Text = "Debug";
			// 
			// debug1ToolStripMenuItem
			// 
			this.debug1ToolStripMenuItem.Name = "debug1ToolStripMenuItem";
			this.debug1ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.debug1ToolStripMenuItem.Text = "Debug1";
			this.debug1ToolStripMenuItem.Click += new System.EventHandler(this.debug1ToolStripMenuItem_Click);
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1443, 587);
			this.Controls.Add(this.buttonAutoSample);
			this.Controls.Add(this.buttonSample);
			this.Controls.Add(this.textBoxConsole);
			this.Controls.Add(this.pictureBoxOutput2);
			this.Controls.Add(this.pictureBoxOutput1);
			this.Controls.Add(this.pictureBoxRaw);
			this.Controls.Add(this.menuStrip1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "FormMain";
			this.Text = "TDPS Release Version 1.0";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
			this.Load += new System.EventHandler(this.FormMain_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxRaw)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxOutput1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxOutput2)).EndInit();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox pictureBoxRaw;
		private System.Windows.Forms.PictureBox pictureBoxOutput1;
		private System.Windows.Forms.PictureBox pictureBoxOutput2;
		private System.Windows.Forms.TextBox textBoxConsole;
		private System.Windows.Forms.Button buttonSample;
		private System.Windows.Forms.Button buttonAutoSample;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem applicationToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem arduinoToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem arduinoControlPanelToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem taskToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem groundToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem Menu_Ground1_Task1;
		private System.Windows.Forms.ToolStripMenuItem Menu_Ground1_Task2;
		private System.Windows.Forms.ToolStripMenuItem ground1Task2ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem Menu_Ground2_Task1;
		private System.Windows.Forms.ToolStripMenuItem Menu_Ground2_Task2;
		private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem debug1ToolStripMenuItem;
	}
}

