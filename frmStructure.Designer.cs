namespace SQLBuilder
{
    partial class frmStructure
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
			components = new System.ComponentModel.Container();
			cmdClose = new Button();
			cmdSave = new Button();
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
			contextMenuListView = new ContextMenuStrip(components);
			removeToolStripMenuItem = new ToolStripMenuItem();
			treeView1 = new TreeView();
			cmdFill = new Button();
			cmdClear = new Button();
			cmdImport = new Button();
			groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
			splitContainer.Panel1.SuspendLayout();
			splitContainer.Panel2.SuspendLayout();
			splitContainer.SuspendLayout();
			contextMenuTreeView.SuspendLayout();
			contextMenuListView.SuspendLayout();
			SuspendLayout();
			// 
			// cmdClose
			// 
			cmdClose.Location = new Point(713, 415);
			cmdClose.Name = "cmdClose";
			cmdClose.Size = new Size(75, 23);
			cmdClose.TabIndex = 0;
			cmdClose.Text = "Закрыть";
			cmdClose.UseVisualStyleBackColor = true;
			cmdClose.Click += cmdClose_Click;
			// 
			// cmdSave
			// 
			cmdSave.Location = new Point(632, 415);
			cmdSave.Name = "cmdSave";
			cmdSave.Size = new Size(75, 23);
			cmdSave.TabIndex = 1;
			cmdSave.Text = "Сохранить";
			cmdSave.UseVisualStyleBackColor = true;
			cmdSave.Click += cmdSave_Click;
			// 
			// groupBox1
			// 
			groupBox1.Controls.Add(splitContainer);
			groupBox1.Location = new Point(12, 12);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new Size(776, 397);
			groupBox1.TabIndex = 2;
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
			splitContainer.Size = new Size(770, 375);
			splitContainer.SplitterDistance = 256;
			splitContainer.TabIndex = 0;
			// 
			// treeView
			// 
			treeView.ContextMenuStrip = contextMenuTreeView;
			treeView.Dock = DockStyle.Fill;
			treeView.Location = new Point(0, 0);
			treeView.Name = "treeView";
			treeView.Size = new Size(256, 375);
			treeView.TabIndex = 0;
			treeView.NodeMouseDoubleClick += treeView_NodeMouseDoubleClick;
			// 
			// contextMenuTreeView
			// 
			contextMenuTreeView.Items.AddRange(new ToolStripItem[] { insertToolStripMenuItem, updateToolStripMenuItem, deleteToolStripMenuItem });
			contextMenuTreeView.Name = "contextMenuTreeView";
			contextMenuTreeView.Size = new Size(129, 70);
			contextMenuTreeView.Opening += contextMenuTreeView_Opening;
			// 
			// insertToolStripMenuItem
			// 
			insertToolStripMenuItem.Name = "insertToolStripMenuItem";
			insertToolStripMenuItem.Size = new Size(128, 22);
			insertToolStripMenuItem.Text = "Добавить";
			insertToolStripMenuItem.Click += insertToolStripMenuItem_Click;
			// 
			// updateToolStripMenuItem
			// 
			updateToolStripMenuItem.Name = "updateToolStripMenuItem";
			updateToolStripMenuItem.Size = new Size(128, 22);
			updateToolStripMenuItem.Text = "Изменить";
			updateToolStripMenuItem.Click += updateToolStripMenuItem_Click;
			// 
			// deleteToolStripMenuItem
			// 
			deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
			deleteToolStripMenuItem.Size = new Size(128, 22);
			deleteToolStripMenuItem.Text = "Удалить";
			deleteToolStripMenuItem.Click += deleteToolStripMenuItem_Click;
			// 
			// listView
			// 
			listView.Columns.AddRange(new ColumnHeader[] { columnName, columnType, columnDescription, columnInstr });
			listView.ContextMenuStrip = contextMenuListView;
			listView.Location = new Point(0, 3);
			listView.Name = "listView";
			listView.Size = new Size(510, 199);
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
			// contextMenuListView
			// 
			contextMenuListView.Items.AddRange(new ToolStripItem[] { removeToolStripMenuItem });
			contextMenuListView.Name = "contextMenuListView";
			contextMenuListView.Size = new Size(119, 26);
			// 
			// removeToolStripMenuItem
			// 
			removeToolStripMenuItem.Name = "removeToolStripMenuItem";
			removeToolStripMenuItem.Size = new Size(118, 22);
			removeToolStripMenuItem.Text = "Удалить";
			removeToolStripMenuItem.Click += removeToolStripMenuItem_Click;
			// 
			// treeView1
			// 
			treeView1.Location = new Point(318, 439);
			treeView1.Name = "treeView1";
			treeView1.Size = new Size(8, 8);
			treeView1.TabIndex = 3;
			// 
			// cmdFill
			// 
			cmdFill.Location = new Point(15, 415);
			cmdFill.Name = "cmdFill";
			cmdFill.Size = new Size(75, 23);
			cmdFill.TabIndex = 4;
			cmdFill.Text = "Заполнить";
			cmdFill.UseVisualStyleBackColor = true;
			cmdFill.Click += cmdFill_Click;
			// 
			// cmdClear
			// 
			cmdClear.Location = new Point(96, 415);
			cmdClear.Name = "cmdClear";
			cmdClear.Size = new Size(75, 23);
			cmdClear.TabIndex = 5;
			cmdClear.Text = "Очистить";
			cmdClear.UseVisualStyleBackColor = true;
			cmdClear.Click += cmdClear_Click;
			// 
			// cmdImport
			// 
			cmdImport.Location = new Point(177, 415);
			cmdImport.Name = "cmdImport";
			cmdImport.Size = new Size(75, 23);
			cmdImport.TabIndex = 6;
			cmdImport.Text = "Импорт";
			cmdImport.UseVisualStyleBackColor = true;
			cmdImport.Click += cmdImport_Click;
			// 
			// frmStructure
			// 
			AcceptButton = cmdSave;
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			CancelButton = cmdClose;
			ClientSize = new Size(800, 450);
			Controls.Add(cmdImport);
			Controls.Add(cmdClear);
			Controls.Add(cmdFill);
			Controls.Add(treeView1);
			Controls.Add(groupBox1);
			Controls.Add(cmdSave);
			Controls.Add(cmdClose);
			FormBorderStyle = FormBorderStyle.FixedSingle;
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "frmStructure";
			Text = "Распределение датчиков по цехам";
			Load += frmStructure_Load;
			groupBox1.ResumeLayout(false);
			splitContainer.Panel1.ResumeLayout(false);
			splitContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
			splitContainer.ResumeLayout(false);
			contextMenuTreeView.ResumeLayout(false);
			contextMenuListView.ResumeLayout(false);
			ResumeLayout(false);
		}

		#endregion

		private Button cmdClose;
        private Button cmdSave;
        private GroupBox groupBox1;
        private SplitContainer splitContainer;
        private TreeView treeView1;
        private ContextMenuStrip contextMenuTreeView;
        private ToolStripMenuItem insertToolStripMenuItem;
        private ToolStripMenuItem updateToolStripMenuItem;
        private ToolStripMenuItem deleteToolStripMenuItem;
        private Button cmdFill;
        private Button cmdClear;
        private ContextMenuStrip contextMenuListView;
        private ToolStripMenuItem removeToolStripMenuItem;
		private Button cmdImport;
		private TreeView treeView;
		private ListView listView;
		private ColumnHeader columnName;
		private ColumnHeader columnType;
		private ColumnHeader columnDescription;
		private ColumnHeader columnInstr;
	}
}