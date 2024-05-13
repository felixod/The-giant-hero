namespace SQLBuilder
{
	partial class frmImport
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
			cmdClose = new Button();
			cmdOK = new Button();
			groupBox = new GroupBox();
			txtLog = new RichTextBox();
			label1 = new Label();
			txtConnectionString = new TextBox();
			groupBox.SuspendLayout();
			SuspendLayout();
			// 
			// cmdClose
			// 
			cmdClose.Location = new Point(713, 415);
			cmdClose.Name = "cmdClose";
			cmdClose.Size = new Size(75, 23);
			cmdClose.TabIndex = 0;
			cmdClose.Text = "Отмена";
			cmdClose.UseVisualStyleBackColor = true;
			// 
			// cmdOK
			// 
			cmdOK.Location = new Point(632, 415);
			cmdOK.Name = "cmdOK";
			cmdOK.Size = new Size(75, 23);
			cmdOK.TabIndex = 1;
			cmdOK.Text = "OK";
			cmdOK.UseVisualStyleBackColor = true;
			// 
			// groupBox
			// 
			groupBox.Controls.Add(txtLog);
			groupBox.Controls.Add(label1);
			groupBox.Controls.Add(txtConnectionString);
			groupBox.Location = new Point(12, 12);
			groupBox.Name = "groupBox";
			groupBox.Size = new Size(776, 397);
			groupBox.TabIndex = 2;
			groupBox.TabStop = false;
			groupBox.Text = "Импорт данных из внешней базы данных";
			// 
			// txtLog
			// 
			txtLog.BackColor = SystemColors.Control;
			txtLog.BorderStyle = BorderStyle.None;
			txtLog.Location = new Point(6, 67);
			txtLog.Name = "txtLog";
			txtLog.ReadOnly = true;
			txtLog.Size = new Size(764, 324);
			txtLog.TabIndex = 2;
			txtLog.Text = "";
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new Point(6, 35);
			label1.Name = "label1";
			label1.Size = new Size(128, 15);
			label1.TabIndex = 1;
			label1.Text = "Строка подключения:";
			// 
			// txtConnectionString
			// 
			txtConnectionString.Location = new Point(140, 32);
			txtConnectionString.Name = "txtConnectionString";
			txtConnectionString.Size = new Size(630, 23);
			txtConnectionString.TabIndex = 0;
			// 
			// frmImport
			// 
			AcceptButton = cmdOK;
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			CancelButton = cmdClose;
			ClientSize = new Size(800, 450);
			Controls.Add(groupBox);
			Controls.Add(cmdOK);
			Controls.Add(cmdClose);
			FormBorderStyle = FormBorderStyle.FixedToolWindow;
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "frmImport";
			StartPosition = FormStartPosition.CenterParent;
			Text = "Импорт из базы данных";
			groupBox.ResumeLayout(false);
			groupBox.PerformLayout();
			ResumeLayout(false);
		}

		#endregion

		private Button cmdClose;
		private Button cmdOK;
		private GroupBox groupBox;
		private RichTextBox txtLog;
		private Label label1;
		private TextBox txtConnectionString;
	}
}