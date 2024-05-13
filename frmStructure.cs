using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;

namespace SQLBuilder
{
	public partial class frmStructure : Form
	{

		/// <summary>
		/// Инициализация формы
		/// </summary>
		public frmStructure()
		{
			InitializeComponent();
			using (ApplicationContext context = new())
			{
				//context.Database.EnsureDeleted();
				//context.Database.EnsureCreated();
				if (context.Database.EnsureCreated()) context.Database.Migrate(); // Если базы данных не существует - создает миграцию
			}
			LoadDepartments();
		}

		/// <summary>
		/// Обработка события. Нажатие на кнопку сохранить
		/// </summary>
		private void cmdSave_Click(object sender, EventArgs e)
		{

		}

		/// <summary>
		/// Рекурсивная функция. Обновить дерево отделов
		/// </summary>
		private void LoadDepartments()
		{
			TreeNode _selectedNode = treeView.SelectedNode;
			treeView.Nodes.Clear();
			using (ApplicationContext context = new())
			{
				var departments = context.Departments.ToList(); // Загрузка всех подразделений
				foreach (var department in departments)
				{
					if (department.ParentId == null)
					{
						// Добавьте корневые элементы в TreeView
						var rootNode = new TreeNode(department.Name)
						{
							Tag = department.Id
						};
						AddChildDepartments(rootNode, department, departments);
						treeView.Nodes.Add(rootNode);
					}
				}
			}
			if (_selectedNode != null)
			{
				treeView.SelectedNode = _selectedNode;
			}
		}

		/// <summary>
		/// Рекурсивая функция. Добавление детей в дерево
		/// </summary>
		private void AddChildDepartments(TreeNode parentNode, Department parentDepartment, List<Department> allDepartments)
		{
			foreach (var childDepartment in allDepartments.Where(d => d.ParentId == parentDepartment.Id))
			{
				var childNode = new TreeNode(childDepartment.Name)
				{
					Tag = childDepartment.Id
				};
				parentNode.Nodes.Add(childNode);
				AddChildDepartments(childNode, childDepartment, allDepartments);
			}
		}

		private static void DbTest()
		{
			using (ApplicationContext db = new())
			{
				// пересоздаем базу данных
				//db.Database.EnsureDeleted();
				//db.Database.EnsureCreated();
				if (db.Database.EnsureCreated()) db.Database.Migrate(); // Если базы данных не существует - создает миграцию



				//MSPDB_Params p = new() { name = "1" , type = "1"};
				//db.MSPDB_Params.AddRange(p);
				//db.SaveChanges();
				//Company google = new() { Name = "Google" };

				//_ = InsertCompanieToDataBase("Microsoft");
				//_ = InsertCompanieToDataBase("Google");

				//db.Companies.AddRange(microsoft, google);

				//Company? c = GetCompanieToDataBase("microsoft");
				//if (c != null)
				//{
				//	User tom = new() { Name = "Tom", Age = 36, Company = c };

				//}

				//User bob = new() { Name = "Bob", Age = 39, Company = google };
				//User alice = new() { Name = "Alice", Age = 28, Company = microsoft };
				//User kate = new() { Name = "Kate", Age = 25, Company = google };

				//Company? c = GetCompanieToDataBase("microsoft");
				//if (c != null)
				//{
				//	_ = InsertUserToDataBase("Tom", 26, c);
				//}

				//db.Users.AddRange(tom, bob, alice, kate);

			}

			//using (ApplicationContext db = new())
			//{
			//	var users = await db.Users
			//						.Include(p => p.Company)
			//						.Where(p => p.CompanyId == 4)
			//						.ToListAsync();     // асинхронное получение данных

			//	foreach (var user in users)
			//		Console.WriteLine($"{user.Name} ({user.Age}) - {user.Company?.Name}");
			//}
		}

		/// <summary>
		/// Обработчик события. Если при открытии всплывающего меню существует выбранный элемент дерева.
		/// </summary>
		private void contextMenuTreeView_Opening(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (treeView.SelectedNode != null)
			{
				insertToolStripMenuItem.Enabled = true;
				updateToolStripMenuItem.Enabled = true;
				deleteToolStripMenuItem.Enabled = true;
			}
			else
			{
				insertToolStripMenuItem.Enabled = true;
				updateToolStripMenuItem.Enabled = false;
				deleteToolStripMenuItem.Enabled = false;
			}
		}

		/// <summary>
		/// Добавление нового элемента в таблицу организаций
		/// </summary>
		private void insertToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenForm(true);
		}

		/// <summary>
		/// Открыть форму редактирования элемента списка
		/// </summary>
		private void updateToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenForm(false);
		}

		/// <summary>
		/// Открыть форму для добавления записи или изменения
		/// </summary>
		private void OpenForm(bool insert)
		{
			if (treeView.SelectedNode != null)
			{
				frmInsertUpdateDeportament form = new(insert, (int)treeView.SelectedNode.Tag);
				form.ShowDialog();
			}
			else
			{
				frmInsertUpdateDeportament form = new(insert, 0);
				form.ShowDialog();
			}
			LoadDepartments();
		}


		/// <summary>
		/// Закрыть форму
		/// </summary>
		private void cmdClose_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		/// <summary>
		/// При событии (форма открывается)
		/// </summary>
		private void frmStructure_Load(object sender, EventArgs e)
		{
			LoadParams();
		}

		/// <summary>
		/// Рекурсивое удаление всех детей в списке
		/// </summary>
		private void RecursiveDelete(Department entity)
		{
			using ApplicationContext context = new();
			var recordsWithCode = context.Departments
				.Where(r => r.ParentId == entity.Id)
				.ToList();
			foreach (var record in recordsWithCode.ToList())
			{
				RecursiveDelete(record);
				context.Departments.Remove(record);
				context.SaveChanges();
			}

		}

		/// <summary>
		/// Контекстное меню дерева. Удалить запись
		/// </summary>
		private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using ApplicationContext context = new();
			var entity = context.Departments.Find((int)treeView.SelectedNode.Tag);
			if (entity != null)
			{
				RecursiveDelete(entity);
				context.Departments.Remove(entity);
				context.SaveChanges();
				LoadDepartments();
			}
		}

		/// <summary>
		/// Контекстное меню списка. Удалить запись
		/// </summary>
		private void removeToolStripMenuItem_Click(object sender, EventArgs e)
		{
		}

		/// <summary>
		/// Обработка события. Двойной клик на элементе дерева. Открыть форму редактирования элемента
		/// </summary>
		private void treeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			OpenForm(false);
		}

		private void cmdFill_Click(object sender, EventArgs e)
		{
			Random random = new();
			using ApplicationContext context = new();

			var param = new MSPDB_Params
			{
				name = random.Next(1, 1001).ToString(),
				type = random.Next(1, 1001).ToString()
			};
			context.MSPDB_Params.Add(param);
			context.SaveChanges();

			var desc = new MSPDB_Params_Desc
			{
				dtb = DateTime.Now,
				id_param = param.id,
				Description = random.Next(1, 1001).ToString()
			};
			context.MSPDB_Params_Desc.Add(desc);
			context.SaveChanges();

			var instr = new MSPDB_Params_Instr
			{
				dtb = DateTime.Now,
				id_param = param.id,
				Name = random.Next(1, 1001).ToString()
			};
			context.MSPDB_Params_Instr.Add(instr);
			context.SaveChanges();
			LoadParams();
		}

		/// <summary>
		/// Загрузка параметров в список
		/// </summary>
		private void LoadParams()
		{
			listView.Items.Clear();
			using ApplicationContext context = new();
			var ps = context.MSPDB_Params
				.Include(t1 => t1.Desc)
				.Include(t1 => t1.Instr)
				.ToList();

			foreach (var p in ps)
			{
				var param = new ListViewItem(new[] { p.name, p.type, p.Desc.Description, p.Instr.Name });
				param.Tag = p.id;
				listView.Items.Add(param);
			}
		}

		private void cmdClear_Click(object sender, EventArgs e)
		{
			using ApplicationContext context = new();
			var ps = context.MSPDB_Params
				.Include(t1 => t1.Desc)
				.Include(t1 => t1.Instr)
				.ToList();
			foreach (var p in ps)
			{
				context.MSPDB_Params_Instr.Remove(p.Instr);
				context.MSPDB_Params_Desc.Remove(p.Desc);
				context.MSPDB_Params.Remove(p);
				context.SaveChanges();
			}
			LoadParams();
		}

		private void cmdImport_Click(object sender, EventArgs e)
		{
			frmImport form = new();
			form.ShowDialog();
		}
	}
}
