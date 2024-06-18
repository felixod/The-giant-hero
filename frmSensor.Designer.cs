namespace SQLBuilder
{
	partial class frmSensor
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
            cmdCancel = new Button();
            cmdSave = new Button();
            txtFind = new TextBox();
            lblSQLFileName = new Label();
            gpbFileName = new GroupBox();
            listView = new ListView();
            columnId = new ColumnHeader();
            columnName = new ColumnHeader();
            columnType = new ColumnHeader();
            columnDescription = new ColumnHeader();
            cmdFind = new Button();
            gpbFileName.SuspendLayout();
            SuspendLayout();
            // 
            // cmdCancel
            // 
            cmdCancel.Location = new Point(592, 403);
            cmdCancel.Name = "cmdCancel";
            cmdCancel.Size = new Size(75, 23);
            cmdCancel.TabIndex = 4;
            cmdCancel.Text = "Отмена";
            cmdCancel.UseVisualStyleBackColor = true;
            cmdCancel.Click += cmdCancel_Click;
            // 
            // cmdSave
            // 
            cmdSave.Location = new Point(511, 403);
            cmdSave.Name = "cmdSave";
            cmdSave.Size = new Size(75, 23);
            cmdSave.TabIndex = 5;
            cmdSave.Text = "Сохранить";
            cmdSave.UseVisualStyleBackColor = true;
            cmdSave.Click += cmdSave_Click;
            // 
            // txtFind
            // 
            txtFind.Location = new Point(120, 24);
            txtFind.Name = "txtFind";
            txtFind.Size = new Size(448, 23);
            txtFind.TabIndex = 6;
            txtFind.Text = "кВ";
            // 
            // lblSQLFileName
            // 
            lblSQLFileName.AutoSize = true;
            lblSQLFileName.Location = new Point(6, 28);
            lblSQLFileName.Name = "lblSQLFileName";
            lblSQLFileName.Size = new Size(104, 15);
            lblSQLFileName.TabIndex = 5;
            lblSQLFileName.Text = "Фильтр датчиков:";
            // 
            // gpbFileName
            // 
            gpbFileName.Controls.Add(listView);
            gpbFileName.Controls.Add(cmdFind);
            gpbFileName.Controls.Add(txtFind);
            gpbFileName.Controls.Add(lblSQLFileName);
            gpbFileName.Location = new Point(12, 12);
            gpbFileName.Name = "gpbFileName";
            gpbFileName.Size = new Size(655, 385);
            gpbFileName.TabIndex = 6;
            gpbFileName.TabStop = false;
            gpbFileName.Text = "Имя файла запроса";
            // 
            // listView
            // 
            listView.Columns.AddRange(new ColumnHeader[] { columnId, columnName, columnType, columnDescription });
            listView.FullRowSelect = true;
            listView.GridLines = true;
            listView.Location = new Point(6, 53);
            listView.Name = "listView";
            listView.Size = new Size(643, 321);
            listView.TabIndex = 8;
            listView.UseCompatibleStateImageBehavior = false;
            listView.View = View.Details;
            // 
            // columnId
            // 
            columnId.Text = "Код";
            columnId.Width = 30;
            // 
            // columnName
            // 
            columnName.Text = "Название";
            columnName.Width = 120;
            // 
            // columnType
            // 
            columnType.Text = "Тип";
            // 
            // columnDescription
            // 
            columnDescription.Text = "Описание";
            columnDescription.Width = 410;
            // 
            // cmdFind
            // 
            cmdFind.Location = new Point(574, 24);
            cmdFind.Name = "cmdFind";
            cmdFind.Size = new Size(75, 23);
            cmdFind.TabIndex = 7;
            cmdFind.Text = "Поиск";
            cmdFind.UseVisualStyleBackColor = true;
            cmdFind.Click += cmdFind_Click;
            // 
            // frmSensor
            // 
            AcceptButton = cmdFind;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = cmdCancel;
            ClientSize = new Size(675, 435);
            Controls.Add(gpbFileName);
            Controls.Add(cmdSave);
            Controls.Add(cmdCancel);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmSensor";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Добавить ссылку датчики";
            Load += frmSensor_Load;
            gpbFileName.ResumeLayout(false);
            gpbFileName.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        internal Button cmdCancel;
		internal Button cmdSave;
		internal TextBox txtFind;
		internal Label lblSQLFileName;
		internal GroupBox gpbFileName;
		private CheckBox chkTrustServerCertificate;
		private CheckBox chkIntegratedSecurity;
		internal TextBox txtUserID;
		internal Label lblUserID;
		internal TextBox txtDataSource;
		internal Label lblDataSource;
		internal TextBox txtSQLDBPass;
		internal Label lblSQLDBPass;
		internal TextBox txtInitialCatalog;
		internal Label lblInitialCatalog;
		internal Button cmdFind;
		private ListView listView;
		private ColumnHeader columnName;
		private ColumnHeader columnType;
		private ColumnHeader columnDescription;
		private ColumnHeader columnId;
	}
}