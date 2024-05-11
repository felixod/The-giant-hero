﻿namespace SQLBuilder
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
            treeView1 = new TreeView();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.SuspendLayout();
            contextMenuTreeView.SuspendLayout();
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
            // treeView1
            // 
            treeView1.Location = new Point(318, 439);
            treeView1.Name = "treeView1";
            treeView1.Size = new Size(8, 8);
            treeView1.TabIndex = 3;
            // 
            // frmStructure
            // 
            AcceptButton = cmdSave;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = cmdClose;
            ClientSize = new Size(800, 450);
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
            ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
            splitContainer.ResumeLayout(false);
            contextMenuTreeView.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Button cmdClose;
        private Button cmdSave;
        private GroupBox groupBox1;
        private SplitContainer splitContainer;
        private TreeView treeView;
        private TreeView treeView1;
        private ContextMenuStrip contextMenuTreeView;
        private ToolStripMenuItem insertToolStripMenuItem;
        private ToolStripMenuItem updateToolStripMenuItem;
        private ToolStripMenuItem deleteToolStripMenuItem;
    }
}