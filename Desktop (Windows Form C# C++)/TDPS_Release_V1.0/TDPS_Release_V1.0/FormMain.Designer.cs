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
			this.buttonArduino = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxRaw)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxOutput1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxOutput2)).BeginInit();
			this.SuspendLayout();
			// 
			// pictureBoxRaw
			// 
			this.pictureBoxRaw.BackColor = System.Drawing.SystemColors.Control;
			this.pictureBoxRaw.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBoxRaw.Location = new System.Drawing.Point(12, 12);
			this.pictureBoxRaw.Name = "pictureBoxRaw";
			this.pictureBoxRaw.Size = new System.Drawing.Size(301, 189);
			this.pictureBoxRaw.TabIndex = 0;
			this.pictureBoxRaw.TabStop = false;
			// 
			// pictureBoxOutput1
			// 
			this.pictureBoxOutput1.BackColor = System.Drawing.SystemColors.Control;
			this.pictureBoxOutput1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBoxOutput1.Location = new System.Drawing.Point(319, 12);
			this.pictureBoxOutput1.Name = "pictureBoxOutput1";
			this.pictureBoxOutput1.Size = new System.Drawing.Size(301, 189);
			this.pictureBoxOutput1.TabIndex = 1;
			this.pictureBoxOutput1.TabStop = false;
			// 
			// pictureBoxOutput2
			// 
			this.pictureBoxOutput2.BackColor = System.Drawing.SystemColors.Control;
			this.pictureBoxOutput2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBoxOutput2.Location = new System.Drawing.Point(626, 12);
			this.pictureBoxOutput2.Name = "pictureBoxOutput2";
			this.pictureBoxOutput2.Size = new System.Drawing.Size(301, 189);
			this.pictureBoxOutput2.TabIndex = 2;
			this.pictureBoxOutput2.TabStop = false;
			// 
			// textBoxConsole
			// 
			this.textBoxConsole.Location = new System.Drawing.Point(933, 12);
			this.textBoxConsole.Multiline = true;
			this.textBoxConsole.Name = "textBoxConsole";
			this.textBoxConsole.ReadOnly = true;
			this.textBoxConsole.Size = new System.Drawing.Size(498, 496);
			this.textBoxConsole.TabIndex = 3;
			// 
			// buttonSample
			// 
			this.buttonSample.Location = new System.Drawing.Point(12, 207);
			this.buttonSample.Name = "buttonSample";
			this.buttonSample.Size = new System.Drawing.Size(75, 23);
			this.buttonSample.TabIndex = 4;
			this.buttonSample.Text = "Sample";
			this.buttonSample.UseVisualStyleBackColor = true;
			this.buttonSample.Click += new System.EventHandler(this.buttonSample_Click);
			// 
			// buttonAutoSample
			// 
			this.buttonAutoSample.Location = new System.Drawing.Point(93, 207);
			this.buttonAutoSample.Name = "buttonAutoSample";
			this.buttonAutoSample.Size = new System.Drawing.Size(122, 23);
			this.buttonAutoSample.TabIndex = 5;
			this.buttonAutoSample.Text = "Auto Sample: off";
			this.buttonAutoSample.UseVisualStyleBackColor = true;
			this.buttonAutoSample.Click += new System.EventHandler(this.buttonAutoSample_Click);
			// 
			// buttonArduino
			// 
			this.buttonArduino.Location = new System.Drawing.Point(12, 485);
			this.buttonArduino.Name = "buttonArduino";
			this.buttonArduino.Size = new System.Drawing.Size(122, 23);
			this.buttonArduino.TabIndex = 6;
			this.buttonArduino.Text = "Arduino Panel";
			this.buttonArduino.UseVisualStyleBackColor = true;
			this.buttonArduino.Click += new System.EventHandler(this.buttonArduino_Click);
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1443, 520);
			this.Controls.Add(this.buttonArduino);
			this.Controls.Add(this.buttonAutoSample);
			this.Controls.Add(this.buttonSample);
			this.Controls.Add(this.textBoxConsole);
			this.Controls.Add(this.pictureBoxOutput2);
			this.Controls.Add(this.pictureBoxOutput1);
			this.Controls.Add(this.pictureBoxRaw);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.Name = "FormMain";
			this.Text = "TDPS Release Version 1.0";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
			this.Load += new System.EventHandler(this.FormMain_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxRaw)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxOutput1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxOutput2)).EndInit();
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
		private System.Windows.Forms.Button buttonArduino;
	}
}

