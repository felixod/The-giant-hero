namespace SQLBuilder
{
    partial class frmInsertUpdateDeportament
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
            txtCode = new TextBox();
            txtPrefix = new TextBox();
            label3 = new Label();
            label2 = new Label();
            txtName = new TextBox();
            label1 = new Label();
            cmdClose = new Button();
            cmdOK = new Button();
            groupBox.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox
            // 
            groupBox.Controls.Add(txtCode);
            groupBox.Controls.Add(txtPrefix);
            groupBox.Controls.Add(label3);
            groupBox.Controls.Add(label2);
            groupBox.Controls.Add(txtName);
            groupBox.Controls.Add(label1);
            groupBox.Location = new Point(12, 12);
            groupBox.Name = "groupBox";
            groupBox.Size = new Size(505, 100);
            groupBox.TabIndex = 0;
            groupBox.TabStop = false;
            groupBox.Text = "Добавить\\редактировать";
            // 
            // txtCode
            // 
            txtCode.Location = new Point(321, 66);
            txtCode.Name = "txtCode";
            txtCode.Size = new Size(178, 23);
            txtCode.TabIndex = 5;
            txtCode.TextChanged += txtCode_TextChanged;
            // 
            // txtPrefix
            // 
            txtPrefix.Location = new Point(120, 66);
            txtPrefix.Name = "txtPrefix";
            txtPrefix.Size = new Size(159, 23);
            txtPrefix.TabIndex = 4;
            txtPrefix.TextChanged += txtPrefix_TextChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(285, 72);
            label3.Name = "label3";
            label3.Size = new Size(30, 15);
            label3.TabIndex = 3;
            label3.Text = "Код:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(6, 69);
            label2.Name = "label2";
            label2.Size = new Size(60, 15);
            label2.TabIndex = 2;
            label2.Text = "Префикс:";
            // 
            // txtName
            // 
            txtName.Location = new Point(120, 31);
            txtName.Name = "txtName";
            txtName.Size = new Size(379, 23);
            txtName.TabIndex = 1;
            txtName.TextChanged += txtName_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 34);
            label1.Name = "label1";
            label1.Size = new Size(93, 15);
            label1.TabIndex = 0;
            label1.Text = "Наименование:";
            // 
            // cmdClose
            // 
            cmdClose.Location = new Point(442, 118);
            cmdClose.Name = "cmdClose";
            cmdClose.Size = new Size(75, 23);
            cmdClose.TabIndex = 1;
            cmdClose.Text = "Отмена";
            cmdClose.UseVisualStyleBackColor = true;
            cmdClose.Click += cmdClose_Click;
            // 
            // cmdOK
            // 
            cmdOK.Location = new Point(361, 118);
            cmdOK.Name = "cmdOK";
            cmdOK.Size = new Size(75, 23);
            cmdOK.TabIndex = 2;
            cmdOK.Text = "OK";
            cmdOK.UseVisualStyleBackColor = true;
            cmdOK.Click += cmdOK_Click;
            // 
            // frmInsertUpdateDeportament
            // 
            AcceptButton = cmdOK;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = cmdClose;
            ClientSize = new Size(529, 154);
            Controls.Add(cmdOK);
            Controls.Add(cmdClose);
            Controls.Add(groupBox);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmInsertUpdateDeportament";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Добавить\\редактировать отдел";
            Load += frmInsertUpdateDeportament_Load;
            groupBox.ResumeLayout(false);
            groupBox.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox;
        private TextBox txtName;
        private Label label1;
        private Label label2;
        private TextBox txtCode;
        private TextBox txtPrefix;
        private Label label3;
        private Button cmdClose;
        private Button cmdOK;
    }
}