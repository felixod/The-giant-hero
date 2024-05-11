using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLBuilder
{
    public partial class frmInsertUpdateDeportament : Form
    {
        private int _itemId;
        private bool _insert;

        public frmInsertUpdateDeportament(bool insert, int itemId)
        {
            InitializeComponent();
            _itemId = itemId; // Сохраняем переданный ID элемента
            _insert = insert;
        }

        private void frmInsertUpdateDeportament_Load(object sender, EventArgs e)
        {
            UpdateButton();
            if (!_insert)
            {
                using ApplicationContext context = new();
                var entityToUpdate = context.Departments.FirstOrDefault(p => p.Id == _itemId);
                if (entityToUpdate != null)
                {
                    txtName.Text = entityToUpdate.Name;
                    txtPrefix.Text = entityToUpdate.Prefix;
                    txtCode.Text = entityToUpdate.Code;
                    context.SaveChanges();
                }
            }
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            if (_insert)
            {
                using ApplicationContext context = new();
                if (_itemId == 0)
                {
                    var d = new Department { Name = txtName.Text, Prefix = txtPrefix.Text, Code = txtCode.Text };
                    context.Departments.Add(d);
                    context.SaveChanges();
                }
                else
                {
                    var d = new Department { Name = txtName.Text, Prefix = txtPrefix.Text, Code = txtCode.Text, ParentId = _itemId };
                    context.Departments.Add(d);
                    context.SaveChanges();
                }
            }
            else
            {
                using ApplicationContext context = new();
                var entityToUpdate = context.Departments.FirstOrDefault(p => p.Id == _itemId);
                if (entityToUpdate != null)
                {
                    entityToUpdate.Name = txtName.Text;
                    entityToUpdate.Prefix = txtPrefix.Text;
                    entityToUpdate.Code = txtCode.Text;
                    context.SaveChanges();
                }
            }

            this.Close();
        }

        private void UpdateButton()
        {
            // Проверяем, заполнены ли все три поля
            bool isNameFilled = !string.IsNullOrWhiteSpace(txtName.Text);
            bool isPrefixFilled = !string.IsNullOrWhiteSpace(txtPrefix.Text);
            bool isCodeFilled = !string.IsNullOrWhiteSpace(txtCode.Text);

            // Если все три поля заполнены, включаем кнопку, иначе отключаем
            cmdOK.Enabled = isNameFilled && isPrefixFilled && isCodeFilled;
        }

        private void cmdClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            UpdateButton();
        }

        private void txtPrefix_TextChanged(object sender, EventArgs e)
        {
            UpdateButton();
        }

        private void txtCode_TextChanged(object sender, EventArgs e)
        {
            UpdateButton();
        }
    }
}
