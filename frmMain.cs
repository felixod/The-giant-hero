
using Microsoft.Data.SqlClient;
using OfficeOpenXml;
using SQLBuilder.ini;
using System.Data;
using System.Xml;
using System.Xml.Linq;

namespace SQLBuilder
{
	public partial class frmMain : Form
	{
		public frmMain()
		{
			InitializeComponent();
		}
		private void frmMain_Load(object sender, EventArgs e)
		{
			lblVersion.Text = $"Версия: {Application.ProductVersion}";
			rtbLog.Text = Log.Read();
			Log.Separate();
			Log.Write("Запуск приложения");
			IniFile iniFile = new("config.ini");
			// Секция SCHEDULE
			// Загружаем из INI-файла начальную дату
			if (iniFile.KeyExists("SCHEDULE", "Start_data"))
				dtpStartExecution.Value = Convert.ToDateTime(iniFile.ReadKey("SCHEDULE", "Start_data"));
			else
				dtpStartExecution.Value = DateTime.Now;
			// Загружаем из INI-файла время выполнения
			if (iniFile.KeyExists("SCHEDULE", "Execution_time"))
				dtpExecTime.Value = DateTime.Now.Date.Add(Convert.ToDateTime(iniFile.ReadKey("SCHEDULE", "Execution_time")).TimeOfDay);
			else
				dtpExecTime.Value = DateTime.Now;
			// Загружаем из INI-файла период выполнения
			if (iniFile.KeyExists("SCHEDULE", "Execution_period"))
				nudExecPeriod.Value = int.Parse(iniFile.ReadKey("SCHEDULE", "Execution_period"));
			else
				nudExecPeriod.Value = 1;
			// Секция INTERVAL
			// Загружаем из INI-файла начальную дату запроса
			if (iniFile.KeyExists("INTERVAL", "Start_data"))
				dtpStartData.Value = Convert.ToDateTime(iniFile.ReadKey("INTERVAL", "Start_data"));
			else
				dtpStartData.Value = DateTime.Now;
			// Загружаем из INI-файла конечную дату запроса
			if (iniFile.KeyExists("INTERVAL", "Final_data"))
				dtpFinalData.Value = Convert.ToDateTime(iniFile.ReadKey("INTERVAL", "Final_data"));
			else
				dtpFinalData.Value = DateTime.Now;
			// Секция FILENAME
			// Загружаем из INI-файла имя файлы с SQL-запросом
			if (iniFile.KeyExists("FILENAME", "SQL_File_Name"))
				txtSQLFileName.Text = iniFile.ReadKey("FILENAME", "SQL_File_Name");
			else
				txtSQLFileName.Text = "";
			// Загружаем из INI-файла имя файлы с результатом работы
			if (iniFile.KeyExists("FILENAME", "XLSX_File_Name"))
				txtResultsFileName.Text = iniFile.ReadKey("FILENAME", "XLSX_File_Name");
			else
				txtResultsFileName.Text = "";
			// Загружаем из INI-файла название базы данных
			if (iniFile.KeyExists("FILENAME", "SQL_DB_InitialCatalog"))
				txtInitialCatalog.Text = iniFile.ReadKey("FILENAME", "SQL_DB_InitialCatalog");
			else
				txtInitialCatalog.Text = "";
			// Загружаем из INI-файла пароль пользователя базы данных
			if (iniFile.KeyExists("FILENAME", "SQL_DB_Pass"))
				txtSQLDBPass.Text = Crypt.Decrypt(iniFile.ReadKey("FILENAME", "SQL_DB_Pass"), Enumerable.Range(0, 32).Select(x => (byte)x).ToArray());
			else
				txtSQLDBPass.Text = "";
			// Загружаем из INI-файла имя пользователя базы данных
			if (iniFile.KeyExists("FILENAME", "SQL_DB_UserID"))
				txtUserID.Text = iniFile.ReadKey("FILENAME", "SQL_DB_UserID");
			else
				txtUserID.Text = "";
			// Загружаем из INI-файла имя или ip-адрес сервера
			if (iniFile.KeyExists("FILENAME", "SQL_DB_DataSource"))
				txtDataSource.Text = iniFile.ReadKey("FILENAME", "SQL_DB_DataSource");
			else
				txtDataSource.Text = "";
			// Загружаем из INI-файла тип используемой авторизации
			if (iniFile.KeyExists("FILENAME", "SQL_DB_IntegratedSecurity"))
				chkIntegratedSecurity.Checked = Convert.ToBoolean(iniFile.ReadKey("FILENAME", "SQL_DB_IntegratedSecurity"));
			else
				chkIntegratedSecurity.Checked = false;
			IntegratedSecurity();
			// Загружаем из INI-файла доверие к сертификату сервера
			if (iniFile.KeyExists("FILENAME", "SQL_DB_TrustServerCertificate"))
				chkTrustServerCertificate.Checked = Convert.ToBoolean(iniFile.ReadKey("FILENAME", "SQL_DB_TrustServerCertificate"));
			else
				chkTrustServerCertificate.Checked = true;

			LoadDepartments();
			Log.Write("Загрузка параметров из ini-файла завершена");
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
		/// Открыть форму для добавления записи или изменения сенсоров
		/// </summary>
		private void OpenSensorForm(bool insert)
		{
			if (treeView.SelectedNode != null)
			{
				frmSensor form = new(insert, (int)treeView.SelectedNode.Tag, treeView.SelectedNode.Text);
				form.ShowDialog();
			}
			LoadSensors();
		}


		/// <summary>
		/// Рекурсивная функция. Обновить дерево отделов
		/// </summary>
		private void LoadDepartments()
		{
			int id = 0;
			if (treeView.SelectedNode != null)
			{
				id = (int)treeView.SelectedNode.Tag;
			}

			treeView.Nodes.Clear();
			TreeViewXmlLoader.LoadTreeViewFromXml(treeView, "departments.xml");

			foreach (TreeNode node in treeView.Nodes)
			{
				int idn = (int)node.Tag;
				if (idn == id)
				{
					treeView.SelectedNode = node;
					break;
				}
				// Если узел имеет дочерние узлы, продолжайте поиск рекурсивно
				SelectNodeByTagRecursive(node, id);
			}

		}


		/// <summary>
		/// Загрузка сенсоров
		/// </summary>
		private void LoadSensors()
		{
			if (treeView.SelectedNode != null)
			{
				LoadSensorForIdXML("departments.xml", (int)treeView.SelectedNode.Tag);
			}
		}

		private void LoadSensorForIdXML(string xmlFilePath, int nodeId)
		{

			if (File.Exists(xmlFilePath))
			{
				XmlDocument doc = new();
				doc.Load(xmlFilePath); // Путь к вашему XML файлу
				listView.Items.Clear();

				XmlNode node = doc.SelectSingleNode($"//Node[@Id='{nodeId}']");
				if (node != null)
				{
					foreach (XmlNode sensorNode in node.SelectNodes("Sensor"))
					{
						var param = new ListViewItem(new[] { sensorNode.Attributes["Id"].Value,
															 sensorNode.Attributes["Name"].Value,
															 sensorNode.Attributes["Type"].Value,
															 sensorNode.Attributes["Description"].Value })
						{
							Tag = nodeId
						};
						listView.Items.Add(param);
					}
				}
			}
		}

		// Рекурсивный метод для поиска в дочерних узлах
		private void SelectNodeByTagRecursive(TreeNode parentNode, int id)
		{
			foreach (TreeNode childNode in parentNode.Nodes)
			{
				int idn = (int)childNode.Tag;
				if (idn == id)
				{
					treeView.SelectedNode = childNode;
					break;
				}
				SelectNodeByTagRecursive(childNode, id);
			}
		}

		private void cmdClose_Click(object sender, EventArgs e)
		{
			Log.Write("Завершение работы приложения");
			Log.Separate();
			Application.Exit();
		}

		private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
		{
			SaveIni();
		}

		private void SaveIni()
		{
			IniFile iniFile = new("config.ini");
			// Секция SCHEDULE
			iniFile.WriteKey("SCHEDULE", "Start_data", dtpStartExecution.Value.Date.ToLongDateString());
			iniFile.WriteKey("SCHEDULE", "Execution_time", dtpExecTime.Value.ToShortTimeString());
			iniFile.WriteKey("SCHEDULE", "Execution_period", nudExecPeriod.Value.ToString());
			// Секция INTERVAL
			iniFile.WriteKey("INTERVAL", "Start_data", dtpStartData.Value.Date.ToLongDateString());
			iniFile.WriteKey("INTERVAL", "Final_data", dtpFinalData.Value.Date.ToLongDateString());
			// TODO Тут что-то про объединение данных с прыдыдущими данными, пока непонятно
			// Секция FILENAME
			iniFile.WriteKey("FILENAME", "SQL_File_Name", txtSQLFileName.Text);
			iniFile.WriteKey("FILENAME", "XLSX_File_Name", txtResultsFileName.Text);
			iniFile.WriteKey("FILENAME", "SQL_DB_DataSource", txtDataSource.Text);
			iniFile.WriteKey("FILENAME", "SQL_DB_UserID", txtUserID.Text);
			byte[] key = Enumerable.Range(0, 32).Select(x => (byte)x).ToArray();
			iniFile.WriteKey("FILENAME", "SQL_DB_Pass", Crypt.Encrypt(txtSQLDBPass.Text, key));
			iniFile.WriteKey("FILENAME", "SQL_DB_InitialCatalog", txtInitialCatalog.Text);
			iniFile.WriteKey("FILENAME", "SQL_DB_IntegratedSecurity", chkIntegratedSecurity.Checked.ToString());
			iniFile.WriteKey("FILENAME", "SQL_DB_TrustServerCertificate", chkTrustServerCertificate.Checked.ToString());
			Log.Write("Запись параметров в ini-файл завершена");
		}

		private void cmdSQLFileName_Click(object sender, EventArgs e)
		{
			ofdSQLFileName.Title = "Пожалуйста выберите файл с запросом SQL";
			ofdSQLFileName.InitialDirectory = @"C:\";
			ofdSQLFileName.Filter = "Файлы SQL|*.sql";
			ofdSQLFileName.InitialDirectory = Application.StartupPath;
			if (!string.IsNullOrEmpty(txtSQLFileName.Text))
				ofdSQLFileName.FileName = txtSQLFileName.Text;
			if (ofdSQLFileName.ShowDialog() == DialogResult.OK)
				Log.Write(string.Concat("Выбран файл SQL-запроса: ", ofdSQLFileName.FileName));
			txtSQLFileName.Text = ofdSQLFileName.FileName;
		}

		private void cmdExport_Click(object sender, EventArgs e)
		{
			StreamReader reader = new(txtSQLFileName.Text);
			string strSQL = reader.ReadToEnd();
			Main(txtDataSource.Text, txtUserID.Text, txtSQLDBPass.Text, txtInitialCatalog.Text, chkIntegratedSecurity.Checked, chkTrustServerCertificate.Checked, strSQL);
			rtbLog.Text = Log.Read();

			async Task Main(string sDataSource, string sUserID, string sPassword, string sInitialCatalog, bool bIntegratedSecurity, bool bTrustServerCertificate, string strSQL)
			{
				SqlConnectionStringBuilder builder = new()
				{
					DataSource = sDataSource,
					UserID = sUserID,
					Password = sPassword,
					InitialCatalog = sInitialCatalog,
					IntegratedSecurity = bIntegratedSecurity,
					TrustServerCertificate = bTrustServerCertificate
				};
				Log.Write(string.Concat("Источник данных (DataSource): ", sDataSource));
				Log.Write(string.Concat("Имя пользователя (UserID): ", sUserID));
				Log.Write(string.Concat("Пароль (Password): ", "*********"));
				Log.Write(string.Concat("База данных (InitialCatalog): ", sInitialCatalog));
				Log.Write(string.Concat("SQL-запрос к базе данных: ", strSQL));
				using SqlConnection connection = new(builder.ConnectionString);
				try
				{
					await connection.OpenAsync();

					string xmlFilePath = "departments.xml";
					string selectedIds = "";

					XmlDocument xmlDoc = new();
					xmlDoc.Load(xmlFilePath);

					XmlNodeList sensorNodes = xmlDoc.SelectNodes("//Sensor");
					foreach (XmlNode sensorNode in sensorNodes)
					{
						string sensorId = sensorNode.Attributes["Id"].Value;
						selectedIds += sensorId + ",";
					}

					// Remove the trailing comma
					if (!string.IsNullOrEmpty(selectedIds))
					{
						selectedIds = selectedIds.TrimEnd(',');
					}

					string sqlQuery;
					// Read SQL query from file
					IniFile iniFile = new("config.ini");
					using (StreamReader sr = new(iniFile.ReadKey("FILENAME", "SQL_File_Name")))
					{
						sqlQuery = sr.ReadToEnd();
					}

					SqlCommand command = new(sqlQuery, connection)
					{
						CommandTimeout = Int32.MaxValue // Установите значение в секундах
					};
					command.Parameters.AddWithValue("@selected_ids", selectedIds);
					command.Parameters.AddWithValue("@start_date", Convert.ToDateTime(iniFile.ReadKey("INTERVAL", "Start_data")));
					command.Parameters.AddWithValue("@end_date", Convert.ToDateTime(iniFile.ReadKey("INTERVAL", "Final_data")));

					SqlDataAdapter adapter = new(command);
					DataTable dataTable = new();

					// Подписываемся на событие RowsUpdated
					adapter.RowUpdated += Adapter_RowsUpdated;

					// Заполняем DataTable
					adapter.Fill(dataTable);

					// Отписываемся от события RowsUpdated
					adapter.RowUpdated -= Adapter_RowsUpdated;

					// Set the LicenseContext
					ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

					// Export DataTable to Excel
					using (ExcelPackage excelPackage = new())
					{
						ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet1");
						// Load data from DataTable to Excel worksheet
						worksheet.Cells["A1"].LoadFromDataTable(dataTable, true);
						worksheet.Column(1).Style.Numberformat.Format = "dd.mm.yyyy HH:mm";
						worksheet.Row(1).Height = 75;
						// Установка выравнивания текста по центру и включение переноса текста
						worksheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
						worksheet.Row(1).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
						worksheet.Row(1).Style.WrapText = true;
						// Установка ширины всех колонок
						for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
						{
							worksheet.Column(col).Width = 11; // Установить ширину колонки на 20
						}
						worksheet.Column(1).Width = 20;
						worksheet.Cells["A1"].Value = "Время";
						worksheet.Cells["B1"].Value = "день";
						worksheet.Cells["C1"].Value = "месяц";
						worksheet.Cells["D1"].Value = "год";
						worksheet.Cells["E1"].Value = "час";
						worksheet.Cells["F1"].Value = "мин";

						// Save Excel package to a file
						string filePath = iniFile.ReadKey("FILENAME", "XLSX_File_Name");
						FileInfo excelFile = new(filePath);
						excelPackage.SaveAs(excelFile);

						worksheet.InsertRow(2, 1); // Вставляем одну пустую строку после первой строки
						worksheet.InsertRow(3, 1); // Вставляем одну пустую строку после второй строки

						// Чтение XML файла для получения кодов sensor и их названий

						Dictionary<string, string> sensorNames = [];
						foreach (XmlNode sensorNode in sensorNodes)
						{
							string id = sensorNode.Attributes["Id"].Value;
							string name = sensorNode.Attributes["UserDescription"].Value;
							sensorNames[id] = name;
						}

						Dictionary<string, string> sensorPrefix = [];
						foreach (XmlNode sensorNode in sensorNodes)
						{
							string id = sensorNode.Attributes["Id"].Value;
							string name = sensorNode.Attributes["Prefix"].Value;
							sensorPrefix[id] = name;
						}

						// Переименование столбцов на основе кодов sensor и их названий
						for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
						{
							string header = worksheet.Cells[1, col].Text;
							if (header.StartsWith("S") && sensorNames.ContainsKey(header.Substring(1)))
							{
								worksheet.Cells[1, col].Value = sensorNames[header.Substring(1)];
								int prefix;
								string sprefix = sensorPrefix[header.Substring(1)];
								if (int.TryParse(sprefix, out prefix))
								{
									worksheet.Cells[3, col].Value = prefix;
								}
								else
								{
									worksheet.Cells[3, col].Value = 0;
								}
							}
						}

						// Добавление второй строки с счетчиком столбов
						for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
						{
							worksheet.Cells[2, col].Value = col - 1; // Устанавливаем значение счетчика столбов
						}
						worksheet.Cells["A2"].Style.Numberformat.Format = "@";

						// Вставка текста в ячейку A3 с переносами строк
						worksheet.Cells["A3"].Value = "Время\nПризнак обработки:\n1-среднее,\n2-сложение,\n3-признак времени\n4 - признак температуры";
						worksheet.Cells["A3"].Style.WrapText = true; // Включаем перенос текста
						worksheet.Cells["A3"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center; // Выравниваем текст по центру

						// Установить признак времени у времени
						worksheet.Cells["B3"].Value = 3;
						worksheet.Cells["C3"].Value = 3;
						worksheet.Cells["D3"].Value = 3;
						worksheet.Cells["E3"].Value = 3;
						worksheet.Cells["F3"].Value = 3;

						// Добавление тонкой линии после первого столбца
						worksheet.Column(2).Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
						// Добавление тонкой линии после первой строки
						worksheet.Row(2).Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

						// Сохранение изменений в Excel файл
						excelPackage.Save();
					}

					Log.Write("Выгрузка успешно завершена.");
					MessageBox.Show("Выгрузка успешно завершена");
				}
				catch (SqlException e)
				{
					Log.Write(string.Concat("Возникла ошибка во время выполнения sql-запроса: ", e.ToString()));
					MessageBox.Show(e.ToString());
				}
			}
		}

		private static void Adapter_RowsUpdated(object sender, SqlRowUpdatedEventArgs e)
		{
			// Получаем количество обновленных строк
			int updatedRows = e.RecordsAffected;

			Log.Write($"Обработано {updatedRows} строк.");

			// Выводим информацию о прогрессе
			Console.WriteLine($"Обработано {updatedRows} строк.");
		}


		private void chkIntegratedSecurity_CheckedChanged(object sender, EventArgs e)
		{
			IntegratedSecurity();
		}

		private void IntegratedSecurity()
		{
			// Блокируем поля логина и пароля, если включена доменная авторизация
			txtUserID.Enabled = !chkIntegratedSecurity.Checked;
			lblUserID.Enabled = !chkIntegratedSecurity.Checked;
			txtSQLDBPass.Enabled = !chkIntegratedSecurity.Checked;
			lblSQLDBPass.Enabled = !chkIntegratedSecurity.Checked;
		}

		private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
		{

			if (treeView.SelectedNode.Tag != null)
			{

				if ((int)treeView.SelectedNode.Tag != 0)
				{
					RemoveNodeById("departments.xml", (int)treeView.SelectedNode.Tag);
					treeView.Nodes.Remove(treeView.SelectedNode);
				}
			}
		}

		/// <summary>
		/// Удаляет элемент дерева объектов и элемент из xml по Id
		/// </summary>
		public void RemoveNodeById(string xmlFilePath, int nodeId)
		{

			if (File.Exists(xmlFilePath))
			{
				XmlDocument doc = new();
				doc.Load(xmlFilePath);
				XmlNode nodeToRemove = doc.SelectSingleNode($"//Node[@Id='{nodeId.ToString()}']");
				_ = (nodeToRemove?.ParentNode.RemoveChild(nodeToRemove));
				doc.Save(xmlFilePath);
			}

		}

		private void insertToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenForm(true);
		}

		private void updateToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenForm(false);
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

		private void treeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			OpenForm(false);
		}

		private void toolStripMenuItem1_Click(object sender, EventArgs e)
		{
			OpenSensorForm(false);
		}

		private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
		{
			LoadSensors();
		}

		private void toolStripMenuItem3_Click(object sender, EventArgs e)
		{
			RemoveSensors(listView, "departments.xml");
		}

		public static void RemoveSensors(ListView listView, string xmlFilePath)
		{
			// Список ID для удаления
			List<string> sensorIds = listView.SelectedItems.Cast<ListViewItem>().Select(item => item.Text).ToList();

			// Удаление из ListView
			foreach (ListViewItem item in listView.SelectedItems)
			{
				listView.Items.Remove(item);
			}

			// Удаление из XML
			XDocument doc = XDocument.Load(xmlFilePath);
			var sensors = doc.Descendants("Sensor").Where(x => sensorIds.Contains(x.Attribute("Id").Value)).ToList();
			foreach (var sensor in sensors)
			{
				sensor.Remove();
			}
			doc.Save(xmlFilePath);
		}

		private void cmdApply_Click(object sender, EventArgs e)
		{
			SaveIni();
		}

		private void listView_DoubleClick(object sender, EventArgs e)
		{
			OpenSensorUpdateForm();
		}

		/// <summary>
		/// Открыть форму для добавления записи или изменения сенсоров
		/// </summary>
		private void OpenSensorUpdateForm()
		{
			if (treeView.SelectedNode != null)
			{
				if (listView.SelectedItems.Count == 1)
				{
					int sersorId;
					try
					{
						sersorId = int.Parse(listView.SelectedItems[0].SubItems[0].Text);
						frmSensorUpdate form = new((int)treeView.SelectedNode.Tag, sersorId);
						form.ShowDialog();
					}
					catch (Exception)
					{
					}
				}
			}
			LoadSensors();
		}

		private void cmdResultsFileName_Click(object sender, EventArgs e)
		{
			ofdResultsFileName.Title = "Пожалуйста выберите файл Microsoft Excel";
			ofdResultsFileName.InitialDirectory = @"C:\";
			ofdResultsFileName.Filter = "Файлы Microsoft Excel|*.xlsx";
			ofdResultsFileName.InitialDirectory = Application.StartupPath;
			if (!string.IsNullOrEmpty(txtResultsFileName.Text))
				ofdResultsFileName.FileName = txtResultsFileName.Text;
			if (ofdResultsFileName.ShowDialog() == DialogResult.OK)
				Log.Write(string.Concat("Выбран файл Microsoft Excel для хранения результатов работы: ", ofdResultsFileName.FileName));
			txtResultsFileName.Text = ofdResultsFileName.FileName;
		}
	}

	public class TreeViewXmlLoader
	{
		// Метод для загрузки TreeView из XML
		public static void LoadTreeViewFromXml(System.Windows.Forms.TreeView treeView, string fileName)
		{
			XmlDocument xmlDoc = new();
			if (File.Exists(fileName))
			{
				xmlDoc.Load(fileName);
				TreeNode rootNode = CreateTreeNode(xmlDoc.DocumentElement);
				treeView.Nodes.Add(rootNode);
			}
			else
			{
				CreateEmptyXmlFile(fileName);
				LoadTreeViewFromXml(treeView, fileName);
			}
		}

		private static void CreateEmptyXmlFile(string filePath)
		{
			XmlDocument doc = new();
			// Создание объявления XML
			XmlDeclaration? xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
			XmlElement? root = doc.DocumentElement;
			doc.InsertBefore(xmlDeclaration, root);

			// Создание корневого элемента
			XmlElement rootElement = doc.CreateElement("Node");
			rootElement.SetAttribute("Name", "Святогор");
			rootElement.SetAttribute("Prefix", "");
			rootElement.SetAttribute("Id", "0");
			rootElement.SetAttribute("Code", "");

			doc.AppendChild(rootElement);

			// Сохранение файла
			doc.Save(filePath);
		}

		// Рекурсивный метод для создания TreeNode из XML узла
		private static TreeNode CreateTreeNode(XmlNode xmlNode)
		{
			TreeNode treeNode = new();
			if (xmlNode.Attributes != null && xmlNode.Name == "Node")
			{
				XmlAttribute? nameAttribute = xmlNode.Attributes["Name"];
				if (nameAttribute != null)
				{
					treeNode.Name = nameAttribute.Value;
					treeNode.Text = nameAttribute.Value; // Используем Name для свойства Text
				}

				XmlAttribute? codeAttribute = xmlNode.Attributes["Id"];
				if (codeAttribute != null)
				{
					treeNode.Tag = int.Parse(codeAttribute.Value); // Сохраняем Id в Tag
				}
			}

			foreach (XmlNode childNode in xmlNode.ChildNodes)
			{
				if (childNode.Name == "Node")
				{
					treeNode.Nodes.Add(CreateTreeNode(childNode)); // Рекурсивно добавляем дочерние узлы
				}
			}
			return treeNode;
		}
	}
}