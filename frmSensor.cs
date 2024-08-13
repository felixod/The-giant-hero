using Microsoft.Data.SqlClient;
using SQLBuilder.ini;
using System.ComponentModel;
using System.Data;
using System.Xml;

namespace SQLBuilder
{
	public partial class frmSensor : Form
	{
		private int _itemId;
		private bool _insert;
		private string _filter;
		private string? _config;


		public frmSensor(bool insert, int itemId, string filter, string? config = null)
		{
			InitializeComponent();
			_itemId = itemId; // Сохраняем переданный ID элемента
			_insert = insert;
			_filter = filter;
			_config = config;
		}

		private void frmSensor_Load(object sender, EventArgs e)
		{
			txtFind.Text = GetFirstWord(_filter);
			Sensor_Load();
		}

		private static string GetFirstWord(string input)
		{
			string[] words = input.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
			return words.Length > 0 ? words[0] : string.Empty;
		}

		private void Sensor_Load()
		{
			BackgroundWorker worker = new();
			worker.DoWork += Worker_DoWork;
			worker.RunWorkerCompleted += Worker_RunWorkerCompleted;

			worker.RunWorkerAsync();
		}

		private void Worker_DoWork(object? sender, DoWorkEventArgs e)
		{
			// Использовать конфигурационный файл с префиксом, если префикс передан
			IniFile iniFile;
			if (!string.IsNullOrEmpty(_config))
			{
				iniFile = new($"config_{_config}.ini");
			}
			else
			{
				iniFile = new("config.ini");
			}
			SqlConnectionStringBuilder builder = new()
			{
				DataSource = iniFile.ReadKey("FILENAME", "SQL_DB_DataSource"),
				UserID = iniFile.ReadKey("FILENAME", "SQL_DB_UserID"),
				Password = Crypt.Decrypt(iniFile.ReadKey("FILENAME", "SQL_DB_Pass"), Enumerable.Range(0, 32).Select(x => (byte)x).ToArray()),
				InitialCatalog = iniFile.ReadKey("FILENAME", "SQL_DB_InitialCatalog"),
				IntegratedSecurity = Convert.ToBoolean(iniFile.ReadKey("FILENAME", "SQL_DB_IntegratedSecurity")),
				TrustServerCertificate = Convert.ToBoolean(iniFile.ReadKey("FILENAME", "SQL_DB_TrustServerCertificate"))
			};
			string connectionString = builder.ToString();
			string query;
			if (string.IsNullOrEmpty(txtFind.Text))
			{
				query = "SELECT [id], [name], type, Description  FROM [MSPDB_Params] JOIN [MSPDB_Params_Desc] ON [MSPDB_Params_Desc].[id_params] = [MSPDB_Params].[id]";
			}
			else
			{
				query = $"SELECT [id], [name], [type], [Description] FROM [MSPDB_Params] JOIN [MSPDB_Params_Desc] ON [MSPDB_Params_Desc].[id_params] = [MSPDB_Params].[id]" +
																	$"WHERE [name] LIKE '%{txtFind.Text}%' OR [Description] LIKE '%{txtFind.Text}%' OR [type] LIKE '%{txtFind.Text}%'";
			}


			using SqlConnection connection = new(connectionString);
			SqlCommand command = new(query, connection);
			SqlDataAdapter adapter = new(command);
			DataTable dataTable = new();

			try
			{
				adapter.Fill(dataTable);
				e.Result = dataTable;
			}
			catch (Exception ex)
			{
				MessageBox.Show("Произошла ошибка: " + ex.Message);

			}

		}

		private void Worker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Error != null)
			{
				MessageBox.Show("Произошла ошибка: " + e.Error.Message);
			}
			else
			{
				DataTable? dataTable = (DataTable?)e.Result;
				if (dataTable == null)
				{
					throw new ArgumentNullException(nameof(dataTable), "DataTable не может быть null.");
				}
				FillListView(dataTable);
			}
		}

		private void FillListView(DataTable dataTable)
		{
			listView.Items.Clear();

			if (dataTable != null)
			{
				foreach (DataRow row in dataTable.Rows)
				{
					if (row.ItemArray != null && row.ItemArray.Length > 0)
					{
						ListViewItem item = new(row.ItemArray.Select(x => x?.ToString() ?? string.Empty).ToArray());
						listView.Items.Add(item);
						// Дальнейшая обработка item
					}
					else
					{
						// Обработка случая, когда ItemArray пуст или null
						Console.WriteLine("ItemArray пуст или не задан.");
					}
					
				}
			}

		}


		static void SaveToXML(int nodeId, string Name, string Type, string Description, string Id)
		{
			// Загружаем XML документ
			XmlDocument doc = new();
			doc.Load(Program._department);

			// Находим узел, в который хотим добавить новый Sensor
			XmlNode? parentNode = doc.SelectSingleNode($"//Node[@Id='{nodeId}']");

			if (parentNode != null)
			{

				XmlNode? sensorNode = doc.SelectSingleNode($"//Sensor[@Id='{Id}']");


				if (sensorNode == null)
				{
					// Если сенсор с таким ID не найден, создаем новый
					XmlElement newSensor = doc.CreateElement("Sensor");
					newSensor.SetAttribute("Id", xmlHelper.RemoveInvalidXmlChars(Id));
					newSensor.SetAttribute("Name", xmlHelper.RemoveInvalidXmlChars(Name));
					newSensor.SetAttribute("Type", xmlHelper.RemoveInvalidXmlChars(Type));
					newSensor.SetAttribute("Description", xmlHelper.RemoveInvalidXmlChars(Description));

					// Генерируем показатели
					string userdescr = $"{Name}[{Type}]{Description}";
					newSensor.SetAttribute("UserDescription", xmlHelper.RemoveInvalidXmlChars(userdescr));
					// TODO: Может быть поймем как понять как ставиться этот признак
					newSensor.SetAttribute("Prefix", "");
					parentNode.AppendChild(newSensor);
				}
				else
				{
					// Если сенсор найден, обновляем его данные
					XmlElement sensorElement = (XmlElement)sensorNode;
					sensorElement.SetAttribute("Name", xmlHelper.RemoveInvalidXmlChars(Name));
					sensorElement.SetAttribute("Type", xmlHelper.RemoveInvalidXmlChars(Type));
					sensorElement.SetAttribute("Description", xmlHelper.RemoveInvalidXmlChars(Description));
					// Генерируем показатели
					string userdescr = $"{Name}[{Type}]{Description}";
					sensorElement.SetAttribute("UserDescription", xmlHelper.RemoveInvalidXmlChars(userdescr));
					// TODO: Может быть поймем как понять как ставиться этот признак
					sensorElement.SetAttribute("Prefix", "");
				}

				// Сохраняем изменения в XML файл
				doc.Save(Program._department);

				Console.WriteLine("Новый Sensor успешно добавлен.");
			}
			else
			{
				Console.WriteLine("Узел для добавления Sensor не найден.");
			}
		}



		private void cmdSave_Click(object sender, EventArgs e)
		{
			Save();
		}

		private void Save()
		{
			if (_insert)
			{
			}
			else
			{
				foreach (ListViewItem selectedItem in listView.SelectedItems)
				{
					string Id = selectedItem.SubItems[1].Text;
					string Name = selectedItem.SubItems[2].Text;
					string Type = selectedItem.SubItems[3].Text;
					string Description = selectedItem.SubItems[0].Text;

					SaveToXML(_itemId, Id, Name, Type, Description);
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

		private void listView_DoubleClick(object sender, EventArgs e)
		{
			Save();
		}
	}
}
