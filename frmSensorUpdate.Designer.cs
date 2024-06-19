namespace SQLBuilder
{
	partial class frmSensorUpdate
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
			groupBox = new GroupBox();
			nudPrefix = new NumericUpDown();
			label2 = new Label();
			txtName = new TextBox();
			label1 = new Label();
			cmdOK = new Button();
			cmdClose = new Button();
			groupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)nudPrefix).BeginInit();
			SuspendLayout();
			// 
			// groupBox
			// 
			groupBox.Controls.Add(nudPrefix);
			groupBox.Controls.Add(label2);
			groupBox.Controls.Add(txtName);
			groupBox.Controls.Add(label1);
			groupBox.Location = new Point(12, 12);
			groupBox.Name = "groupBox";
			groupBox.Size = new Size(505, 100);
			groupBox.TabIndex = 3;
			groupBox.TabStop = false;
			groupBox.Text = "Редактировать дополнительную информацию о сенсоре";
			// 
			// nudPrefix
			// 
			nudPrefix.Location = new Point(133, 67);
			nudPrefix.Maximum = new decimal(new int[] { 4, 0, 0, 0 });
			nudPrefix.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
			nudPrefix.Name = "nudPrefix";
			nudPrefix.Size = new Size(84, 23);
			nudPrefix.TabIndex = 9;
			nudPrefix.Value = new decimal(new int[] { 1, 0, 0, 0 });
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new Point(6, 69);
			label2.Name = "label2";
			label2.Size = new Size(119, 15);
			label2.TabIndex = 2;
			label2.Text = "Признак обработки:";
			// 
			// txtName
			// 
			txtName.Location = new Point(133, 31);
			txtName.Name = "txtName";
			txtName.Size = new Size(366, 23);
			txtName.TabIndex = 1;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new Point(6, 34);
			label1.Name = "label1";
			label1.Size = new Size(121, 15);
			label1.TabIndex = 0;
			label1.Text = "Свое наименование:";
			// 
			// cmdOK
			// 
			cmdOK.Location = new Point(357, 118);
			cmdOK.Name = "cmdOK";
			cmdOK.Size = new Size(75, 23);
			cmdOK.TabIndex = 5;
			cmdOK.Text = "OK";
			cmdOK.UseVisualStyleBackColor = true;
			cmdOK.Click += cmdOK_Click;
			// 
			// cmdClose
			// 
			cmdClose.Location = new Point(438, 118);
			cmdClose.Name = "cmdClose";
			cmdClose.Size = new Size(75, 23);
			cmdClose.TabIndex = 4;
			cmdClose.Text = "Отмена";
			cmdClose.UseVisualStyleBackColor = true;
			// 
			// frmSensorUpdate
			// 
			AcceptButton = cmdOK;
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			CancelButton = cmdClose;
			ClientSize = new Size(525, 147);
			Controls.Add(groupBox);
			Controls.Add(cmdOK);
			Controls.Add(cmdClose);
			FormBorderStyle = FormBorderStyle.FixedToolWindow;
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "frmSensorUpdate";
			StartPosition = FormStartPosition.CenterParent;
			Text = "Персонализация сенсора";
			Load += frmSensorUpdate_Load;
			groupBox.ResumeLayout(false);
			groupBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)nudPrefix).EndInit();
			ResumeLayout(false);
		}

		#endregion

		private GroupBox groupBox;
		private Label label2;
		private TextBox txtName;
		private Label label1;
		private Button cmdOK;
		private Button cmdClose;
		internal NumericUpDown nudPrefix;
	}
}