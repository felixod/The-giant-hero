﻿namespace SQLBuilder
{
	partial class frmMain
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
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
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			cmdClose = new Button();
			cmdExport = new Button();
			tabMain = new TabControl();
			tpgSKADA = new TabPage();
			groupBox1 = new GroupBox();
			splitContainer = new SplitContainer();
			treeView = new TreeView();
			contextMenuTreeView = new ContextMenuStrip(components);
			insertToolStripMenuItem = new ToolStripMenuItem();
			updateToolStripMenuItem = new ToolStripMenuItem();
			deleteToolStripMenuItem = new ToolStripMenuItem();
			listView = new ListView();
			columnName = new ColumnHeader();
			columnType = new ColumnHeader();
			columnDescription = new ColumnHeader();
			columnInstr = new ColumnHeader();
			tpgSchedule = new TabPage();
			gbxSchedule = new GroupBox();
			nudExecPeriod = new NumericUpDown();
			lblDay = new Label();
			lblExecPeriod = new Label();
			dtpExecTime = new DateTimePicker();
			lblExecTime = new Label();
			lblStartExecution = new Label();
			dtpStartExecution = new DateTimePicker();
			Label1 = new Label();
			tpgInterval = new TabPage();
			gpbInterval = new GroupBox();
			lblFinalData = new Label();
			dtpFinalData = new DateTimePicker();
			lblStartData = new Label();
			dtpStartData = new DateTimePicker();
			lblInterval = new Label();
			tpgFileName = new TabPage();
			gpbFileName = new GroupBox();
			chkTrustServerCertificate = new CheckBox();
			chkIntegratedSecurity = new CheckBox();
			txtUserID = new TextBox();
			lblUserID = new Label();
			txtDataSource = new TextBox();
			lblDataSource = new Label();
			txtSQLDBPass = new TextBox();
			lblSQLDBPass = new Label();
			txtInitialCatalog = new TextBox();
			lblInitialCatalog = new Label();
			cmdSQLFileName = new Button();
			txtSQLFileName = new TextBox();
			lblSQLFileName = new Label();
			tpgLog = new TabPage();
			gpbLog = new GroupBox();
			rtbLog = new RichTextBox();
			ofdSQLFileName = new OpenFileDialog();
			cmdStructure = new Button();
			tabMain.SuspendLayout();
			tpgSKADA.SuspendLayout();
			groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
			splitContainer.Panel1.SuspendLayout();
			splitContainer.Panel2.SuspendLayout();
			splitContainer.SuspendLayout();
			contextMenuTreeView.SuspendLayout();
			tpgSchedule.SuspendLayout();
			gbxSchedule.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)nudExecPeriod).BeginInit();
			tpgInterval.SuspendLayout();
			gpbInterval.SuspendLayout();
			tpgFileName.SuspendLayout();
			gpbFileName.SuspendLayout();
			tpgLog.SuspendLayout();
			gpbLog.SuspendLayout();
			SuspendLayout();
			// 
			// cmdClose
			// 
			cmdClose.Location = new Point(937, 453);
			cmdClose.Name = "cmdClose";
			cmdClose.Size = new Size(75, 23);
			cmdClose.TabIndex = 1;
			cmdClose.Text = "Закрыть";
			cmdClose.UseVisualStyleBackColor = true;
			cmdClose.Click += cmdClose_Click;
			// 
			// cmdExport
			// 
			cmdExport.Location = new Point(856, 453);
			cmdExport.Name = "cmdExport";
			cmdExport.Size = new Size(75, 23);
			cmdExport.TabIndex = 3;
			cmdExport.Text = "Экспорт";
			cmdExport.UseVisualStyleBackColor = true;
			cmdExport.Click += cmdExport_Click;
			// 
			// tabMain
			// 
			tabMain.Controls.Add(tpgSKADA);
			tabMain.Controls.Add(tpgSchedule);
			tabMain.Controls.Add(tpgInterval);
			tabMain.Controls.Add(tpgFileName);
			tabMain.Controls.Add(tpgLog);
			tabMain.Location = new Point(12, 12);
			tabMain.Name = "tabMain";
			tabMain.SelectedIndex = 0;
			tabMain.Size = new Size(1009, 435);
			tabMain.TabIndex = 4;
			// 
			// tpgSKADA
			// 
			tpgSKADA.Controls.Add(groupBox1);
			tpgSKADA.Location = new Point(4, 24);
			tpgSKADA.Name = "tpgSKADA";
			tpgSKADA.Size = new Size(1001, 407);
			tpgSKADA.TabIndex = 4;
			tpgSKADA.Text = "Структура";
			tpgSKADA.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			groupBox1.Controls.Add(splitContainer);
			groupBox1.Location = new Point(3, 3);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new Size(994, 401);
			groupBox1.TabIndex = 4;
			groupBox1.TabStop = false;
			groupBox1.Text = "Дерево цехов";
			// 
			// splitContainer
			// 
			splitContainer.Dock = DockStyle.Fill;
			splitContainer.Location = new Point(3, 19);
			splitContainer.Name = "splitContainer";
			// 
			// splitContainer.Panel1
			// 
			splitContainer.Panel1.Controls.Add(treeView);
			// 
			// splitContainer.Panel2
			// 
			splitContainer.Panel2.Controls.Add(listView);
			splitContainer.Size = new Size(988, 379);
			splitContainer.SplitterDistance = 329;
			splitContainer.TabIndex = 0;
			// 
			// treeView
			// 
			treeView.ContextMenuStrip = contextMenuTreeView;
			treeView.Dock = DockStyle.Fill;
			treeView.Location = new Point(0, 0);
			treeView.Name = "treeView";
			treeView.Size = new Size(329, 379);
			treeView.TabIndex = 0;
			// 
			// contextMenuTreeView
			// 
			contextMenuTreeView.Items.AddRange(new ToolStripItem[] { insertToolStripMenuItem, updateToolStripMenuItem, deleteToolStripMenuItem });
			contextMenuTreeView.Name = "contextMenuTreeView";
			contextMenuTreeView.Size = new Size(181, 92);
			// 
			// insertToolStripMenuItem
			// 
			insertToolStripMenuItem.Name = "insertToolStripMenuItem";
			insertToolStripMenuItem.Size = new Size(180, 22);
			insertToolStripMenuItem.Text = "Добавить";
			// 
			// updateToolStripMenuItem
			// 
			updateToolStripMenuItem.Name = "updateToolStripMenuItem";
			updateToolStripMenuItem.Size = new Size(180, 22);
			updateToolStripMenuItem.Text = "Изменить";
			// 
			// deleteToolStripMenuItem
			// 
			deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
			deleteToolStripMenuItem.Size = new Size(180, 22);
			deleteToolStripMenuItem.Text = "Удалить";
			deleteToolStripMenuItem.Click += deleteToolStripMenuItem_Click;
			// 
			// listView
			// 
			listView.Columns.AddRange(new ColumnHeader[] { columnName, columnType, columnDescription, columnInstr });
			listView.Dock = DockStyle.Fill;
			listView.Location = new Point(0, 0);
			listView.Name = "listView";
			listView.Size = new Size(655, 379);
			listView.TabIndex = 0;
			listView.UseCompatibleStateImageBehavior = false;
			listView.View = View.Details;
			// 
			// columnName
			// 
			columnName.Text = "Название";
			columnName.Width = 200;
			// 
			// columnType
			// 
			columnType.Text = "Тип";
			// 
			// columnDescription
			// 
			columnDescription.Text = "Описание";
			columnDescription.Width = 300;
			// 
			// columnInstr
			// 
			columnInstr.Text = "Формула";
			columnInstr.Width = 200;
			// 
			// tpgSchedule
			// 
			tpgSchedule.Controls.Add(gbxSchedule);
			tpgSchedule.Location = new Point(4, 24);
			tpgSchedule.Name = "tpgSchedule";
			tpgSchedule.Padding = new Padding(3);
			tpgSchedule.Size = new Size(1001, 407);
			tpgSchedule.TabIndex = 0;
			tpgSchedule.Text = "Расписание";
			tpgSchedule.UseVisualStyleBackColor = true;
			// 
			// gbxSchedule
			// 
			gbxSchedule.Controls.Add(nudExecPeriod);
			gbxSchedule.Controls.Add(lblDay);
			gbxSchedule.Controls.Add(lblExecPeriod);
			gbxSchedule.Controls.Add(dtpExecTime);
			gbxSchedule.Controls.Add(lblExecTime);
			gbxSchedule.Controls.Add(lblStartExecution);
			gbxSchedule.Controls.Add(dtpStartExecution);
			gbxSchedule.Controls.Add(Label1);
			gbxSchedule.Location = new Point(6, 6);
			gbxSchedule.Name = "gbxSchedule";
			gbxSchedule.Size = new Size(992, 395);
			gbxSchedule.TabIndex = 0;
			gbxSchedule.TabStop = false;
			gbxSchedule.Text = "Расписание *";
			// 
			// nudExecPeriod
			// 
			nudExecPeriod.Location = new Point(139, 96);
			nudExecPeriod.Maximum = new decimal(new int[] { 365, 0, 0, 0 });
			nudExecPeriod.Name = "nudExecPeriod";
			nudExecPeriod.Size = new Size(64, 23);
			nudExecPeriod.TabIndex = 8;
			// 
			// lblDay
			// 
			lblDay.AutoSize = true;
			lblDay.Location = new Point(209, 100);
			lblDay.Name = "lblDay";
			lblDay.Size = new Size(36, 15);
			lblDay.TabIndex = 7;
			lblDay.Text = "дней.";
			// 
			// lblExecPeriod
			// 
			lblExecPeriod.AutoSize = true;
			lblExecPeriod.Location = new Point(6, 100);
			lblExecPeriod.Name = "lblExecPeriod";
			lblExecPeriod.Size = new Size(124, 15);
			lblExecPeriod.TabIndex = 5;
			lblExecPeriod.Text = "Период выполнения:";
			// 
			// dtpExecTime
			// 
			dtpExecTime.CustomFormat = "HH:mm";
			dtpExecTime.Format = DateTimePickerFormat.Custom;
			dtpExecTime.Location = new Point(139, 59);
			dtpExecTime.Name = "dtpExecTime";
			dtpExecTime.ShowUpDown = true;
			dtpExecTime.Size = new Size(64, 23);
			dtpExecTime.TabIndex = 2;
			// 
			// lblExecTime
			// 
			lblExecTime.AutoSize = true;
			lblExecTime.Location = new Point(6, 63);
			lblExecTime.Name = "lblExecTime";
			lblExecTime.Size = new Size(117, 15);
			lblExecTime.TabIndex = 3;
			lblExecTime.Text = "Время выполнения:";
			// 
			// lblStartExecution
			// 
			lblStartExecution.AutoSize = true;
			lblStartExecution.Location = new Point(6, 28);
			lblStartExecution.Name = "lblStartExecution";
			lblStartExecution.Size = new Size(96, 15);
			lblStartExecution.TabIndex = 2;
			lblStartExecution.Text = "Начальная дата:";
			// 
			// dtpStartExecution
			// 
			dtpStartExecution.Location = new Point(139, 24);
			dtpStartExecution.Name = "dtpStartExecution";
			dtpStartExecution.Size = new Size(145, 23);
			dtpStartExecution.TabIndex = 1;
			// 
			// Label1
			// 
			Label1.AutoSize = true;
			Label1.Location = new Point(6, 320);
			Label1.Name = "Label1";
			Label1.Size = new Size(546, 15);
			Label1.TabIndex = 0;
			Label1.Text = "* В этом разделе описывается начальная дата, время выполнения и период выполнения заданий.";
			// 
			// tpgInterval
			// 
			tpgInterval.Controls.Add(gpbInterval);
			tpgInterval.Location = new Point(4, 24);
			tpgInterval.Name = "tpgInterval";
			tpgInterval.Padding = new Padding(3);
			tpgInterval.Size = new Size(1001, 407);
			tpgInterval.TabIndex = 1;
			tpgInterval.Text = "Период";
			tpgInterval.UseVisualStyleBackColor = true;
			// 
			// gpbInterval
			// 
			gpbInterval.Controls.Add(lblFinalData);
			gpbInterval.Controls.Add(dtpFinalData);
			gpbInterval.Controls.Add(lblStartData);
			gpbInterval.Controls.Add(dtpStartData);
			gpbInterval.Controls.Add(lblInterval);
			gpbInterval.Location = new Point(6, 6);
			gpbInterval.Name = "gpbInterval";
			gpbInterval.Size = new Size(992, 395);
			gpbInterval.TabIndex = 0;
			gpbInterval.TabStop = false;
			gpbInterval.Text = "Период *";
			// 
			// lblFinalData
			// 
			lblFinalData.AutoSize = true;
			lblFinalData.Location = new Point(6, 63);
			lblFinalData.Name = "lblFinalData";
			lblFinalData.Size = new Size(89, 15);
			lblFinalData.TabIndex = 6;
			lblFinalData.Text = "Конечная дата:";
			// 
			// dtpFinalData
			// 
			dtpFinalData.Location = new Point(139, 59);
			dtpFinalData.Name = "dtpFinalData";
			dtpFinalData.Size = new Size(145, 23);
			dtpFinalData.TabIndex = 5;
			// 
			// lblStartData
			// 
			lblStartData.AutoSize = true;
			lblStartData.Location = new Point(6, 28);
			lblStartData.Name = "lblStartData";
			lblStartData.Size = new Size(96, 15);
			lblStartData.TabIndex = 4;
			lblStartData.Text = "Начальная дата:";
			// 
			// dtpStartData
			// 
			dtpStartData.Location = new Point(139, 24);
			dtpStartData.Name = "dtpStartData";
			dtpStartData.Size = new Size(145, 23);
			dtpStartData.TabIndex = 3;
			// 
			// lblInterval
			// 
			lblInterval.AutoSize = true;
			lblInterval.Location = new Point(6, 321);
			lblInterval.Name = "lblInterval";
			lblInterval.Size = new Size(459, 15);
			lblInterval.TabIndex = 1;
			lblInterval.Text = "* В этом разделе описывается начальная и конечная даты запроса к SQL-серверу.";
			// 
			// tpgFileName
			// 
			tpgFileName.Controls.Add(gpbFileName);
			tpgFileName.Location = new Point(4, 24);
			tpgFileName.Name = "tpgFileName";
			tpgFileName.Size = new Size(1001, 407);
			tpgFileName.TabIndex = 2;
			tpgFileName.Text = "Имя файла";
			tpgFileName.UseVisualStyleBackColor = true;
			// 
			// gpbFileName
			// 
			gpbFileName.Controls.Add(chkTrustServerCertificate);
			gpbFileName.Controls.Add(chkIntegratedSecurity);
			gpbFileName.Controls.Add(txtUserID);
			gpbFileName.Controls.Add(lblUserID);
			gpbFileName.Controls.Add(txtDataSource);
			gpbFileName.Controls.Add(lblDataSource);
			gpbFileName.Controls.Add(txtSQLDBPass);
			gpbFileName.Controls.Add(lblSQLDBPass);
			gpbFileName.Controls.Add(txtInitialCatalog);
			gpbFileName.Controls.Add(lblInitialCatalog);
			gpbFileName.Controls.Add(cmdSQLFileName);
			gpbFileName.Controls.Add(txtSQLFileName);
			gpbFileName.Controls.Add(lblSQLFileName);
			gpbFileName.Location = new Point(6, 6);
			gpbFileName.Name = "gpbFileName";
			gpbFileName.Size = new Size(990, 398);
			gpbFileName.TabIndex = 2;
			gpbFileName.TabStop = false;
			gpbFileName.Text = "Имя файла запроса";
			// 
			// chkTrustServerCertificate
			// 
			chkTrustServerCertificate.AutoSize = true;
			chkTrustServerCertificate.Location = new Point(6, 201);
			chkTrustServerCertificate.Name = "chkTrustServerCertificate";
			chkTrustServerCertificate.Size = new Size(197, 19);
			chkTrustServerCertificate.TabIndex = 17;
			chkTrustServerCertificate.Text = "Доверять сертификату сервера";
			chkTrustServerCertificate.UseVisualStyleBackColor = true;
			// 
			// chkIntegratedSecurity
			// 
			chkIntegratedSecurity.AutoSize = true;
			chkIntegratedSecurity.Location = new Point(6, 176);
			chkIntegratedSecurity.Name = "chkIntegratedSecurity";
			chkIntegratedSecurity.Size = new Size(168, 19);
			chkIntegratedSecurity.TabIndex = 16;
			chkIntegratedSecurity.Text = "Доменная учетная запись";
			chkIntegratedSecurity.UseVisualStyleBackColor = true;
			chkIntegratedSecurity.CheckedChanged += chkIntegratedSecurity_CheckedChanged;
			// 
			// txtUserID
			// 
			txtUserID.Location = new Point(120, 111);
			txtUserID.MaxLength = 128;
			txtUserID.Name = "txtUserID";
			txtUserID.Size = new Size(376, 23);
			txtUserID.TabIndex = 15;
			txtUserID.WordWrap = false;
			// 
			// lblUserID
			// 
			lblUserID.AutoSize = true;
			lblUserID.Location = new Point(6, 115);
			lblUserID.Name = "lblUserID";
			lblUserID.Size = new Size(87, 15);
			lblUserID.TabIndex = 14;
			lblUserID.Text = "Пользователь:";
			// 
			// txtDataSource
			// 
			txtDataSource.Location = new Point(120, 53);
			txtDataSource.MaxLength = 128;
			txtDataSource.Name = "txtDataSource";
			txtDataSource.Size = new Size(376, 23);
			txtDataSource.TabIndex = 13;
			txtDataSource.WordWrap = false;
			// 
			// lblDataSource
			// 
			lblDataSource.AutoSize = true;
			lblDataSource.Location = new Point(6, 57);
			lblDataSource.Name = "lblDataSource";
			lblDataSource.Size = new Size(50, 15);
			lblDataSource.TabIndex = 12;
			lblDataSource.Text = "Сервер:";
			// 
			// txtSQLDBPass
			// 
			txtSQLDBPass.Location = new Point(120, 140);
			txtSQLDBPass.MaxLength = 128;
			txtSQLDBPass.Name = "txtSQLDBPass";
			txtSQLDBPass.PasswordChar = '*';
			txtSQLDBPass.Size = new Size(376, 23);
			txtSQLDBPass.TabIndex = 11;
			txtSQLDBPass.UseSystemPasswordChar = true;
			txtSQLDBPass.WordWrap = false;
			// 
			// lblSQLDBPass
			// 
			lblSQLDBPass.AutoSize = true;
			lblSQLDBPass.Location = new Point(6, 144);
			lblSQLDBPass.Name = "lblSQLDBPass";
			lblSQLDBPass.Size = new Size(52, 15);
			lblSQLDBPass.TabIndex = 10;
			lblSQLDBPass.Text = "Пароль:";
			// 
			// txtInitialCatalog
			// 
			txtInitialCatalog.Location = new Point(120, 82);
			txtInitialCatalog.MaxLength = 255;
			txtInitialCatalog.Name = "txtInitialCatalog";
			txtInitialCatalog.Size = new Size(376, 23);
			txtInitialCatalog.TabIndex = 9;
			// 
			// lblInitialCatalog
			// 
			lblInitialCatalog.AutoSize = true;
			lblInitialCatalog.Location = new Point(6, 86);
			lblInitialCatalog.Name = "lblInitialCatalog";
			lblInitialCatalog.Size = new Size(80, 15);
			lblInitialCatalog.TabIndex = 8;
			lblInitialCatalog.Text = "Название БД:";
			// 
			// cmdSQLFileName
			// 
			cmdSQLFileName.Location = new Point(502, 24);
			cmdSQLFileName.Name = "cmdSQLFileName";
			cmdSQLFileName.Size = new Size(75, 23);
			cmdSQLFileName.TabIndex = 7;
			cmdSQLFileName.Text = "Открыть";
			cmdSQLFileName.UseVisualStyleBackColor = true;
			cmdSQLFileName.Click += cmdSQLFileName_Click;
			// 
			// txtSQLFileName
			// 
			txtSQLFileName.Location = new Point(120, 24);
			txtSQLFileName.Name = "txtSQLFileName";
			txtSQLFileName.Size = new Size(376, 23);
			txtSQLFileName.TabIndex = 6;
			// 
			// lblSQLFileName
			// 
			lblSQLFileName.AutoSize = true;
			lblSQLFileName.Location = new Point(6, 28);
			lblSQLFileName.Name = "lblSQLFileName";
			lblSQLFileName.Size = new Size(72, 15);
			lblSQLFileName.TabIndex = 5;
			lblSQLFileName.Text = "Имя файла:";
			// 
			// tpgLog
			// 
			tpgLog.Controls.Add(gpbLog);
			tpgLog.Location = new Point(4, 24);
			tpgLog.Name = "tpgLog";
			tpgLog.Size = new Size(1001, 407);
			tpgLog.TabIndex = 3;
			tpgLog.Text = "Лог";
			tpgLog.UseVisualStyleBackColor = true;
			// 
			// gpbLog
			// 
			gpbLog.Controls.Add(rtbLog);
			gpbLog.Location = new Point(3, 3);
			gpbLog.Name = "gpbLog";
			gpbLog.Size = new Size(995, 401);
			gpbLog.TabIndex = 0;
			gpbLog.TabStop = false;
			gpbLog.Text = "Лог событий:";
			// 
			// rtbLog
			// 
			rtbLog.BackColor = SystemColors.Window;
			rtbLog.BorderStyle = BorderStyle.None;
			rtbLog.Location = new Point(6, 22);
			rtbLog.Name = "rtbLog";
			rtbLog.ReadOnly = true;
			rtbLog.Size = new Size(983, 373);
			rtbLog.TabIndex = 0;
			rtbLog.Text = "";
			// 
			// ofdSQLFileName
			// 
			ofdSQLFileName.CheckFileExists = false;
			ofdSQLFileName.FileName = "Новый запрос";
			// 
			// cmdStructure
			// 
			cmdStructure.Location = new Point(11, 452);
			cmdStructure.Margin = new Padding(2);
			cmdStructure.Name = "cmdStructure";
			cmdStructure.Size = new Size(75, 23);
			cmdStructure.TabIndex = 5;
			cmdStructure.Text = "Схема";
			cmdStructure.UseVisualStyleBackColor = true;
			cmdStructure.Click += cmdStructure_Click;
			// 
			// frmMain
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			CancelButton = cmdClose;
			ClientSize = new Size(1024, 486);
			Controls.Add(cmdStructure);
			Controls.Add(tabMain);
			Controls.Add(cmdExport);
			Controls.Add(cmdClose);
			FormBorderStyle = FormBorderStyle.FixedSingle;
			MaximizeBox = false;
			Name = "frmMain";
			Text = "SQLBuilder";
			FormClosed += frmMain_FormClosed;
			Load += frmMain_Load;
			tabMain.ResumeLayout(false);
			tpgSKADA.ResumeLayout(false);
			groupBox1.ResumeLayout(false);
			splitContainer.Panel1.ResumeLayout(false);
			splitContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
			splitContainer.ResumeLayout(false);
			contextMenuTreeView.ResumeLayout(false);
			tpgSchedule.ResumeLayout(false);
			gbxSchedule.ResumeLayout(false);
			gbxSchedule.PerformLayout();
			((System.ComponentModel.ISupportInitialize)nudExecPeriod).EndInit();
			tpgInterval.ResumeLayout(false);
			gpbInterval.ResumeLayout(false);
			gpbInterval.PerformLayout();
			tpgFileName.ResumeLayout(false);
			gpbFileName.ResumeLayout(false);
			gpbFileName.PerformLayout();
			tpgLog.ResumeLayout(false);
			gpbLog.ResumeLayout(false);
			ResumeLayout(false);
		}

		#endregion

		internal Button cmdClose;
		internal Button cmdExport;
		internal TabControl tabMain;
		internal TabPage tpgSchedule;
		internal GroupBox gbxSchedule;
		internal NumericUpDown nudExecPeriod;
		internal Label lblDay;
		internal Label lblExecPeriod;
		internal DateTimePicker dtpExecTime;
		internal Label lblExecTime;
		internal Label lblStartExecution;
		internal DateTimePicker dtpStartExecution;
		internal Label Label1;
		internal TabPage tpgInterval;
		internal GroupBox gpbInterval;
		internal Label lblFinalData;
		internal DateTimePicker dtpFinalData;
		internal Label lblStartData;
		internal DateTimePicker dtpStartData;
		internal Label lblInterval;
		internal TabPage tpgFileName;
		internal GroupBox gpbFileName;
		internal TextBox txtSQLDBPass;
		internal Label lblSQLDBPass;
		internal TextBox txtInitialCatalog;
		internal Label lblInitialCatalog;
		internal Button cmdSQLFileName;
		internal TextBox txtSQLFileName;
		internal Label lblSQLFileName;
		internal OpenFileDialog ofdSQLFileName;
		internal TextBox txtDataSource;
		internal Label lblDataSource;
		internal TextBox txtUserID;
		internal Label lblUserID;
		private CheckBox chkIntegratedSecurity;
		private CheckBox chkTrustServerCertificate;
		private TabPage tpgLog;
		private GroupBox gpbLog;
		private RichTextBox rtbLog;
        private Button cmdStructure;
		private TabPage tpgSKADA;
		private GroupBox groupBox1;
		private SplitContainer splitContainer;
		private TreeView treeView;
		private ListView listView;
		private ColumnHeader columnName;
		private ColumnHeader columnType;
		private ColumnHeader columnDescription;
		private ColumnHeader columnInstr;
		private ContextMenuStrip contextMenuTreeView;
		private ToolStripMenuItem insertToolStripMenuItem;
		private ToolStripMenuItem updateToolStripMenuItem;
		private ToolStripMenuItem deleteToolStripMenuItem;
	}
}