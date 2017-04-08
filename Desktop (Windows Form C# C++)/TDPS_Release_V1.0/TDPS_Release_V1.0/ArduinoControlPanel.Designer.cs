namespace TDPS_Release_V1._0
{
	partial class FormArduinoControlPanel
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
			this.buttonSend = new System.Windows.Forms.Button();
			this.textBoxSpeedMotorA = new System.Windows.Forms.TextBox();
			this.textBoxMotorTime = new System.Windows.Forms.TextBox();
			this.textBoxSpeedMotorB = new System.Windows.Forms.TextBox();
			this.labelMotorA = new System.Windows.Forms.Label();
			this.labelMotorB = new System.Windows.Forms.Label();
			this.comboBoxMotorA = new System.Windows.Forms.ComboBox();
			this.comboBoxMotorB = new System.Windows.Forms.ComboBox();
			this.labelSpeed = new System.Windows.Forms.Label();
			this.labelTime = new System.Windows.Forms.Label();
			this.labelArduinoBusy = new System.Windows.Forms.Label();
			this.textBoxArduinoConsole = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// buttonSend
			// 
			this.buttonSend.Location = new System.Drawing.Point(229, 82);
			this.buttonSend.Name = "buttonSend";
			this.buttonSend.Size = new System.Drawing.Size(100, 23);
			this.buttonSend.TabIndex = 0;
			this.buttonSend.Text = "Send";
			this.buttonSend.UseVisualStyleBackColor = true;
			this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
			// 
			// textBoxSpeedMotorA
			// 
			this.textBoxSpeedMotorA.Location = new System.Drawing.Point(123, 56);
			this.textBoxSpeedMotorA.Name = "textBoxSpeedMotorA";
			this.textBoxSpeedMotorA.Size = new System.Drawing.Size(100, 20);
			this.textBoxSpeedMotorA.TabIndex = 1;
			// 
			// textBoxMotorTime
			// 
			this.textBoxMotorTime.Location = new System.Drawing.Point(123, 82);
			this.textBoxMotorTime.Name = "textBoxMotorTime";
			this.textBoxMotorTime.Size = new System.Drawing.Size(100, 20);
			this.textBoxMotorTime.TabIndex = 2;
			// 
			// textBoxSpeedMotorB
			// 
			this.textBoxSpeedMotorB.Location = new System.Drawing.Point(229, 56);
			this.textBoxSpeedMotorB.Name = "textBoxSpeedMotorB";
			this.textBoxSpeedMotorB.Size = new System.Drawing.Size(100, 20);
			this.textBoxSpeedMotorB.TabIndex = 3;
			// 
			// labelMotorA
			// 
			this.labelMotorA.AutoSize = true;
			this.labelMotorA.Location = new System.Drawing.Point(123, 13);
			this.labelMotorA.Name = "labelMotorA";
			this.labelMotorA.Size = new System.Drawing.Size(44, 13);
			this.labelMotorA.TabIndex = 4;
			this.labelMotorA.Text = "Motor A";
			// 
			// labelMotorB
			// 
			this.labelMotorB.AutoSize = true;
			this.labelMotorB.Location = new System.Drawing.Point(226, 13);
			this.labelMotorB.Name = "labelMotorB";
			this.labelMotorB.Size = new System.Drawing.Size(44, 13);
			this.labelMotorB.TabIndex = 5;
			this.labelMotorB.Text = "Motor B";
			// 
			// comboBoxMotorA
			// 
			this.comboBoxMotorA.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxMotorA.FormattingEnabled = true;
			this.comboBoxMotorA.Location = new System.Drawing.Point(123, 29);
			this.comboBoxMotorA.Name = "comboBoxMotorA";
			this.comboBoxMotorA.Size = new System.Drawing.Size(100, 21);
			this.comboBoxMotorA.TabIndex = 6;
			// 
			// comboBoxMotorB
			// 
			this.comboBoxMotorB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxMotorB.FormattingEnabled = true;
			this.comboBoxMotorB.Location = new System.Drawing.Point(229, 29);
			this.comboBoxMotorB.Name = "comboBoxMotorB";
			this.comboBoxMotorB.Size = new System.Drawing.Size(100, 21);
			this.comboBoxMotorB.TabIndex = 7;
			// 
			// labelSpeed
			// 
			this.labelSpeed.AutoSize = true;
			this.labelSpeed.Location = new System.Drawing.Point(20, 59);
			this.labelSpeed.Name = "labelSpeed";
			this.labelSpeed.Size = new System.Drawing.Size(97, 13);
			this.labelSpeed.TabIndex = 8;
			this.labelSpeed.Text = "Speed (MIN~MAX)";
			// 
			// labelTime
			// 
			this.labelTime.AutoSize = true;
			this.labelTime.Location = new System.Drawing.Point(65, 87);
			this.labelTime.Name = "labelTime";
			this.labelTime.Size = new System.Drawing.Size(52, 13);
			this.labelTime.TabIndex = 9;
			this.labelTime.Text = "Time (ms)";
			// 
			// labelArduinoBusy
			// 
			this.labelArduinoBusy.AutoSize = true;
			this.labelArduinoBusy.Location = new System.Drawing.Point(20, 13);
			this.labelArduinoBusy.Name = "labelArduinoBusy";
			this.labelArduinoBusy.Size = new System.Drawing.Size(70, 13);
			this.labelArduinoBusy.TabIndex = 10;
			this.labelArduinoBusy.Text = "Arduino: Free";
			// 
			// textBoxArduinoConsole
			// 
			this.textBoxArduinoConsole.Location = new System.Drawing.Point(335, 29);
			this.textBoxArduinoConsole.Multiline = true;
			this.textBoxArduinoConsole.Name = "textBoxArduinoConsole";
			this.textBoxArduinoConsole.ReadOnly = true;
			this.textBoxArduinoConsole.Size = new System.Drawing.Size(244, 236);
			this.textBoxArduinoConsole.TabIndex = 11;
			// 
			// FormArduinoControlPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(591, 277);
			this.Controls.Add(this.textBoxArduinoConsole);
			this.Controls.Add(this.labelArduinoBusy);
			this.Controls.Add(this.labelTime);
			this.Controls.Add(this.labelSpeed);
			this.Controls.Add(this.comboBoxMotorB);
			this.Controls.Add(this.comboBoxMotorA);
			this.Controls.Add(this.labelMotorB);
			this.Controls.Add(this.labelMotorA);
			this.Controls.Add(this.textBoxSpeedMotorB);
			this.Controls.Add(this.textBoxMotorTime);
			this.Controls.Add(this.textBoxSpeedMotorA);
			this.Controls.Add(this.buttonSend);
			this.Name = "FormArduinoControlPanel";
			this.Text = "Arduino Control Panel";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormArduinoControlPanel_FormClosing);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonSend;
		private System.Windows.Forms.TextBox textBoxSpeedMotorA;
		private System.Windows.Forms.TextBox textBoxMotorTime;
		private System.Windows.Forms.TextBox textBoxSpeedMotorB;
		private System.Windows.Forms.Label labelMotorA;
		private System.Windows.Forms.Label labelMotorB;
		private System.Windows.Forms.ComboBox comboBoxMotorA;
		private System.Windows.Forms.ComboBox comboBoxMotorB;
		private System.Windows.Forms.Label labelSpeed;
		private System.Windows.Forms.Label labelTime;
		private System.Windows.Forms.Label labelArduinoBusy;
		private System.Windows.Forms.TextBox textBoxArduinoConsole;
	}
}