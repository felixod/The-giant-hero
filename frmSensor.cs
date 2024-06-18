using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using System.Xml.Linq;
using Microsoft.Data.SqlClient;
using System.Xml;

namespace SQLBuilder
{
	public partial class frmSensor : Form
	{
		private int _itemId;
		private bool _insert;


		public frmSensor(bool insert, int itemId)
		{
			InitializeComponent();
			_itemId = itemId; // Сохраняем переданный ID элемента
			_insert = insert;
		}

		private void frmSensor_Load(object sender, EventArgs e)
		{
			Sensor_Load();
		}

		private void Sensor_Load()
		{
			BackgroundWorker worker = new();
			worker.DoWork += Worker_DoWork;
			worker.RunWorkerCompleted += Worker_RunWorkerCompleted;

			worker.RunWorkerAsync();
		}

		private void Worker_DoWork(object sender, DoWorkEventArgs e)
		{
			string connectionString = "Server=MSSQL02\\DB02;Database=MSCADA;Trusted_Connection=True;Integrated Security=true;TrustServerCertificate=True";

			string query;
			if (string.IsNullOrEmpty(txtFind.Text))
			{
				query = "SELECT [name], type, Description, [id]  FROM [MSPDB_Params] JOIN [MSPDB_Params_Desc] ON [MSPDB_Params_Desc].[id_params] = [MSPDB_Params].[id]";
			}
			else
			{
				query = $"SELECT [name], [type], [Description], [id] FROM [MSPDB_Params] JOIN [MSPDB_Params_Desc] ON [MSPDB_Params_Desc].[id_params] = [MSPDB_Params].[id]" +
																	$"WHERE [name] LIKE '%{txtFind.Text}%' OR [Description] LIKE '%{txtFind.Text}%' OR [type] LIKE '%{txtFind.Text}%'";
			}


			using SqlConnection connection = new(connectionString);
			SqlCommand command = new(query, connection);
			SqlDataAdapter adapter = new(command);
			DataTable dataTable = new();

			adapter.Fill(dataTable);

			e.Result = dataTable;
		}

		private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Error != null)
			{
				MessageBox.Show("Произошла ошибка: " + e.Error.Message);
			}
			else
			{
				DataTable dataTable = (DataTable)e.Result;
				FillListView(dataTable);
			}
		}

		private void FillListView(DataTable dataTable)
		{
			listView.Items.Clear();
			
				foreach (DataRow row in dataTable.Rows)
				{
					ListViewItem item = new(row.ItemArray.Select(x => x.ToString()).ToArray());
					listView.Items.Add(item);
				}
		}


		static void SaveToXML(string Name, string Type, string Description, string Formula)
		{
			// Загружаем XML документ
			XmlDocument doc = new();
			doc.Load("departments.xml");

			// Находим узел, в который хотим добавить новый Sensor
			XmlNode parentNode = doc.SelectSingleNode("//Node[@Id='2']");

			if (parentNode != null)
			{
				// Создаем новый элемент Sensor
				XmlElement newSensor = doc.CreateElement("Sensor");
				newSensor.SetAttribute("Name", xmlHelper.RemoveInvalidXmlChars(Name));
				newSensor.SetAttribute("Type", xmlHelper.RemoveInvalidXmlChars(Type));
				newSensor.SetAttribute("Description", xmlHelper.RemoveInvalidXmlChars(Description));
				newSensor.SetAttribute("Formula", xmlHelper.RemoveInvalidXmlChars(Formula));

				// Добавляем новый Sensor в узел
				parentNode.AppendChild(newSensor);

				// Сохраняем изменения в XML файл
				doc.Save("departments.xml");

				Console.WriteLine("Новый Sensor успешно добавлен.");
			}
			else
			{
				Console.WriteLine("Узел для добавления Sensor не найден.");
			}
		}



		private void cmdSave_Click(object sender, EventArgs e)
		{
			if (_insert)
			{	
			}
			else
			{
				foreach (ListViewItem selectedItem in listView.SelectedItems)
				{
					SaveToXML(selectedItem.SubItems[0].Text, selectedItem.SubItems[1].Text, selectedItem.SubItems[2].Text, selectedItem.SubItems[3].Text);
				}
			}

			this.Close();
		}

		private void cmdCancel_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void cmdFind_Click(object sender, EventArgs e)
		{
			Sensor_Load();
		}
	}
}
