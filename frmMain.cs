
using Microsoft.Data.SqlClient;
using OfficeOpenXml;
using SQLBuilder.ini;
using System.Data;
using System.Xml;
using System.Xml.Linq;
using System.Diagnostics;
using System.Security.Principal;

namespace SQLBuilder
{
	public partial class frmMain : Form
	{
		private bool _silent;
		private string? _config;
		private Stopwatch stopwatch = new();
		private System.Windows.Forms.Timer? timer;

		/// <summary>
		/// Конструктор класса основной формы приложения
		/// </summary>
		/// <param name="silent">Если true, приложение выполняется в тихом режиме без отображения графического интерфейса</param>
		/// <param name="config">Если не null, приложение запускается с требуемой конфигурацией параметров</param>
		public frmMain(bool silent = false, string? config = null)
		{
			_silent = silent;
			_config = config;

			// Изменяем значение файл логов по-умолчанию
			if (!string.IsNullOrEmpty(_config))
			{
				Log.ConfigPrefix(_config);
			}
			InitializeComponent();

		}


		/// <summary>
		/// Сохранение текущих настроек программы в конфигурационный файл
		/// </summary>
		private void SaveIniFile()
		{
			// Использовать конфигурационный файл с префиксом, если префикс передан
			IniFile iniFile = new(string.IsNullOrEmpty(_config) ? "config.ini" : $"config_{_config}.ini");

			// Секция SCHEDULE
			iniFile.WriteKey("SCHEDULE", "Start_data", dtpStartExecution.Value.Date.ToLongDateString());
			iniFile.WriteKey("SCHEDULE", "Execution_time", dtpExecTime.Value.ToShortTimeString());
			iniFile.WriteKey("SCHEDULE", "Execution_period", nudExecPeriod.Value.ToString());
			// Секция INTERVAL
			iniFile.WriteKey("INTERVAL", "Start_data", dtpStartData.Value.Date.ToLongDateString());
			iniFile.WriteKey("INTERVAL", "Final_data", dtpFinalData.Value.Date.ToLongDateString());
			// TODO Тут что-то про объединение данных с прыдыдущими данными, пока непонятно
			// Секция FILENAME
			iniFile.WriteKey("FILENAME", "Config_Name", txtConfigName.Text);
			iniFile.WriteKey("FILENAME", "Config_Name_Id", txtConfigNameId.Text);
			iniFile.WriteKey("FILENAME", "SQL_File_Name", txtSQLFileName.Text);
			iniFile.WriteKey("FILENAME", "PathToResultFolder", txtResultsFileName.Text);
			iniFile.WriteKey("FILENAME", "SQL_DB_DataSource", txtDataSource.Text);
			iniFile.WriteKey("FILENAME", "SQL_DB_UserID", txtUserID.Text);
			byte[] key = Enumerable.Range(0, 32).Select(x => (byte)x).ToArray();
			iniFile.WriteKey("FILENAME", "SQL_DB_Pass", Crypt.Encrypt(txtSQLDBPass.Text, key));
			iniFile.WriteKey("FILENAME", "SQL_DB_InitialCatalog", txtInitialCatalog.Text);
			iniFile.WriteKey("FILENAME", "SQL_DB_IntegratedSecurity", chkIntegratedSecurity.Checked.ToString());
			iniFile.WriteKey("FILENAME", "SQL_DB_TrustServerCertificate", chkTrustServerCertificate.Checked.ToString());
			iniFile.WriteKey("FILENAME", "AddDateToFileResult", chkResultsFileNameAddDate.Checked.ToString());
			iniFile.WriteKey("FILENAME", "Format_Export", cbxFormat.SelectedIndex.ToString());
			Log.Write("Запись параметров в ini-файл завершена");
		}

		/// <summary>
		/// Читает параметры из ini-файла и заполняет значениями элементы формы
		/// </summary>
		private void ReadIniFile()
		{
			// Использовать конфигурационный файл с префиксом, если префикс передан
			IniFile iniFile = new(string.IsNullOrEmpty(_config) ? "config.ini" : $"config_{_config}.ini");

			// Секция SCHEDULE
			// Загружаем из INI-файла начальную дату
			try
			{
				if (iniFile.KeyExists("SCHEDULE", "Start_data"))
				{
					string startData = iniFile.ReadKey("SCHEDULE", "Start_data");
					// Проверяем, является ли строка корректной датой
					if (DateTime.TryParse(startData, out DateTime parsedDate))
					{
						dtpStartExecution.Value = parsedDate;
					}
					else
					{
						// Если строка не является корректной датой, устанавливаем текущее время
						dtpStartExecution.Value = DateTime.Now;
					}
				}
				else
				{
					dtpStartExecution.Value = DateTime.Now;
				}
			}
			catch (Exception ex)
			{
				// Обработка исключений, например, логирование ошибки
				Console.WriteLine($"Ошибка: {ex.Message}");
				Log.Write($"Ошибка: {ex.Message}");
				dtpStartExecution.Value = DateTime.Now; // Устанавливаем текущее время в случае ошибки
			}
			try
			{
				// Проверяем, существует ли ключ "Execution_time" в секции "SCHEDULE"
				if (iniFile.KeyExists("SCHEDULE", "Execution_time"))
				{
					string executionTimeString = iniFile.ReadKey("SCHEDULE", "Execution_time");

					// Проверяем, является ли строка корректной датой/временем
					if (DateTime.TryParse(executionTimeString, out DateTime executionTime))
					{
						// Устанавливаем время выполнения, добавляя время к текущей дате
						dtpExecTime.Value = DateTime.Now.Date.Add(executionTime.TimeOfDay);
					}
					else
					{
						// Если строка не является корректной датой/временем, устанавливаем текущее время
						Console.WriteLine($"Ошибка: '{executionTimeString}' не является корректным временем. Устанавливаем текущее время.");
						dtpExecTime.Value = DateTime.Now;
					}
				}
				else
				{
					// Если ключ не существует, устанавливаем текущее время
					dtpExecTime.Value = DateTime.Now;
				}
			}
			catch (Exception ex)
			{
				// Обработка исключений, например, логирование ошибки
				Console.WriteLine($"Ошибка: {ex.Message}");
				Log.Write($"Ошибка: {ex.Message}");
				dtpExecTime.Value = DateTime.Now; // Устанавливаем текущее время в случае ошибки
			}
			try
			{
				// Проверяем, существует ли ключ "Execution_period" в секции "SCHEDULE"
				if (iniFile.KeyExists("SCHEDULE", "Execution_period"))
				{
					string executionPeriodString = iniFile.ReadKey("SCHEDULE", "Execution_period");

					// Проверяем, является ли строка корректным целым числом
					if (int.TryParse(executionPeriodString, out int executionPeriod))
					{
						// Устанавливаем период выполнения
						nudExecPeriod.Value = executionPeriod;
					}
					else
					{
						// Если строка не является корректным целым числом, устанавливаем значение по умолчанию
						Console.WriteLine($"Ошибка: '{executionPeriodString}' не является корректным целым числом. Устанавливаем значение по умолчанию 1.");
						Log.Write($"Ошибка: '{executionPeriodString}' не является корректным целым числом. Устанавливаем значение по умолчанию 1.");
						nudExecPeriod.Value = 1;
					}
				}
				else
				{
					// Если ключ не существует, устанавливаем значение по умолчанию
					nudExecPeriod.Value = 1;
				}
			}
			catch (Exception ex)
			{
				// Обработка исключений, например, логирование ошибки
				Console.WriteLine($"Ошибка при загрузке периода выполнения: {ex.Message}");
				Log.Write($"Ошибка при загрузке периода выполнения: {ex.Message}");
				nudExecPeriod.Value = 1; // Устанавливаем значение по умолчанию в случае ошибки
			}

			// Секция INTERVAL
			try
			{
				// Проверяем, существует ли ключ "Start_data" в секции "INTERVAL"
				if (iniFile.KeyExists("INTERVAL", "Start_data"))
				{
					string startDataString = iniFile.ReadKey("INTERVAL", "Start_data");

					// Проверяем, является ли строка корректной датой
					if (DateTime.TryParse(startDataString, out DateTime startData))
					{
						// Устанавливаем начальную дату запроса
						dtpStartData.Value = startData;
					}
					else
					{
						// Если строка не является корректной датой, устанавливаем текущее время
						Console.WriteLine($"Ошибка: '{startDataString}' не является корректной датой. Устанавливаем текущее время.");
						Log.Write($"Ошибка: '{startDataString}' не является корректной датой. Устанавливаем текущее время.");
						dtpStartData.Value = DateTime.Now;
					}
				}
				else
				{
					// Если ключ не существует, устанавливаем текущее время
					dtpStartData.Value = DateTime.Now;
				}
			}
			catch (Exception ex)
			{
				// Обработка исключений, например, логирование ошибки
				Console.WriteLine($"Ошибка при загрузке начальной даты запроса: {ex.Message}");
				Log.Write($"Ошибка при загрузке начальной даты запроса: {ex.Message}");
				dtpStartData.Value = DateTime.Now; // Устанавливаем текущее время в случае ошибки
			}

			try
			{
				// Проверяем, существует ли ключ "Final_data" в секции "INTERVAL"
				if (iniFile.KeyExists("INTERVAL", "Final_data"))
				{
					string finalDataString = iniFile.ReadKey("INTERVAL", "Final_data");

					// Проверяем, является ли строка корректной датой
					if (DateTime.TryParse(finalDataString, out DateTime finalData))
					{
						// Устанавливаем конечную дату запроса
						dtpFinalData.Value = finalData;
					}
					else
					{
						// Если строка не является корректной датой, устанавливаем текущее время
						Console.WriteLine($"Ошибка: '{finalDataString}' не является корректной датой. Устанавливаем текущее время.");
						Log.Write($"Ошибка: '{finalDataString}' не является корректной датой. Устанавливаем текущее время.");
						dtpFinalData.Value = DateTime.Now;
					}
				}
				else
				{
					// Если ключ не существует, устанавливаем текущее время
					dtpFinalData.Value = DateTime.Now;
				}
			}
			catch (Exception ex)
			{
				// Обработка исключений, например, логирование ошибки
				Console.WriteLine($"Ошибка при загрузке конечной даты запроса: {ex.Message}");
				Log.Write($"Ошибка при загрузке конечной даты запроса: {ex.Message}");
				dtpFinalData.Value = DateTime.Now; // Устанавливаем текущее время в случае ошибки
			}

			// Секция FILENAME
			// Загружаем из INI-файла наименование и код конфигурации
			if (iniFile.KeyExists("FILENAME", "Config_Name"))
			{
				txtConfigName.Text = iniFile.ReadKey("FILENAME", "Config_Name");
			}
			else
				txtConfigName.Text = "";

			if (iniFile.KeyExists("FILENAME", "Config_Name_Id"))
				txtConfigNameId.Text = iniFile.ReadKey("FILENAME", "Config_Name_Id");
			else
				txtConfigNameId.Text = "";

			// Загружаем из INI-файла имя файлы с SQL-запросом
			if (iniFile.KeyExists("FILENAME", "SQL_File_Name"))
				txtSQLFileName.Text = iniFile.ReadKey("FILENAME", "SQL_File_Name");
			else
				txtSQLFileName.Text = "";

			// Загружаем из INI-файла путь до папки с результатами
			if (iniFile.KeyExists("FILENAME", "PathToResultFolder"))
				txtResultsFileName.Text = iniFile.ReadKey("FILENAME", "PathToResultFolder");
			else
				txtResultsFileName.Text = "";

			try
			{
				// Проверяем, существует ли ключ "AddDateToFileResult" в секции "FILENAME"
				if (iniFile.KeyExists("FILENAME", "AddDateToFileResult"))
				{
					string addDateToFileResultString = iniFile.ReadKey("FILENAME", "AddDateToFileResult");

					// Проверяем, является ли строка корректным булевым значением
					if (bool.TryParse(addDateToFileResultString, out bool addDateToFileResult))
					{
						// Устанавливаем значение чекбокса
						chkResultsFileNameAddDate.Checked = addDateToFileResult;
					}
					else
					{
						// Если строка не является корректным булевым значением, устанавливаем значение по умолчанию
						Console.WriteLine($"Ошибка: '{addDateToFileResultString}' не является корректным булевым значением. Устанавливаем значение по умолчанию (false).");
						Log.Write($"Ошибка: '{addDateToFileResultString}' не является корректным булевым значением. Устанавливаем значение по умолчанию (false).");
						chkResultsFileNameAddDate.Checked = false;
					}
				}
				else
				{
					// Если ключ не существует, устанавливаем значение по умолчанию
					chkResultsFileNameAddDate.Checked = false;
				}
			}
			catch (Exception ex)
			{
				// Обработка исключений, например, логирование ошибки
				Console.WriteLine($"Ошибка при загрузке значения добавления даты к имени файла результата: {ex.Message}");
				Log.Write($"Ошибка при загрузке значения добавления даты к имени файла результата: {ex.Message}");
				chkResultsFileNameAddDate.Checked = false; // Устанавливаем значение по умолчанию в случае ошибки
			}

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
			try
			{
				// Проверяем, существует ли ключ "SQL_DB_IntegratedSecurity" в секции "FILENAME"
				if (iniFile.KeyExists("FILENAME", "SQL_DB_IntegratedSecurity"))
				{
					string integratedSecurityString = iniFile.ReadKey("FILENAME", "SQL_DB_IntegratedSecurity");

					// Проверяем, является ли строка корректным булевым значением
					if (bool.TryParse(integratedSecurityString, out bool integratedSecurity))
					{
						// Устанавливаем значение чекбокса
						chkIntegratedSecurity.Checked = integratedSecurity;
					}
					else
					{
						// Если строка не является корректным булевым значением, устанавливаем значение по умолчанию
						Console.WriteLine($"Ошибка: '{integratedSecurityString}' не является корректным булевым значением. Устанавливаем значение по умолчанию (false).");
						Log.Write($"Ошибка: '{integratedSecurityString}' не является корректным булевым значением. Устанавливаем значение по умолчанию (false).");
						chkIntegratedSecurity.Checked = false;
					}
				}
				else
				{
					// Если ключ не существует, устанавливаем значение по умолчанию
					chkIntegratedSecurity.Checked = false;
				}
			}
			catch (Exception ex)
			{
				// Обработка исключений, например, логирование ошибки
				Console.WriteLine($"Ошибка при загрузке типа используемой авторизации: {ex.Message}");
				Log.Write($"Ошибка при загрузке типа используемой авторизации: {ex.Message}");
				chkIntegratedSecurity.Checked = false; // Устанавливаем значение по умолчанию в случае ошибки
				
			}
			IntegratedSecurity();

			// Загружаем из INI-файла доверие к сертификату сервера
			try
			{
				// Проверяем, существует ли ключ "SQL_DB_TrustServerCertificate" в секции "FILENAME"
				if (iniFile.KeyExists("FILENAME", "SQL_DB_TrustServerCertificate"))
				{
					string trustServerCertificateString = iniFile.ReadKey("FILENAME", "SQL_DB_TrustServerCertificate");

					// Проверяем, является ли строка корректным булевым значением
					if (bool.TryParse(trustServerCertificateString, out bool trustServerCertificate))
					{
						// Устанавливаем значение чекбокса
						chkTrustServerCertificate.Checked = trustServerCertificate;
					}
					else
					{
						// Если строка не является корректным булевым значением, устанавливаем значение по умолчанию
						Console.WriteLine($"Ошибка: '{trustServerCertificateString}' не является корректным булевым значением. Устанавливаем значение по умолчанию (true).");
						Log.Write($"Ошибка: '{trustServerCertificateString}' не является корректным булевым значением. Устанавливаем значение по умолчанию (true).");
						chkTrustServerCertificate.Checked = true;
					}
				}
				else
				{
					// Если ключ не существует, устанавливаем значение по умолчанию
					chkTrustServerCertificate.Checked = true;
				}
			}
			catch (Exception ex)
			{
				// Обработка исключений, например, логирование ошибки
				Console.WriteLine($"Ошибка при загрузке доверия к сертификату сервера: {ex.Message}");
				Log.Write($"Ошибка при загрузке доверия к сертификату сервера: {ex.Message}");
				chkTrustServerCertificate.Checked = true; // Устанавливаем значение по умолчанию в случае ошибки
			}

			// Загружаем из INI-файла формат, в который будет осуществляться экспорт
			if (iniFile.KeyExists("FILENAME", "Format_Export"))
			{
				string formatExportValue = iniFile.ReadKey("FILENAME", "Format_Export");
				if (int.TryParse(formatExportValue, out int selectedIndex))
				{
					cbxFormat.SelectedIndex = selectedIndex;
				}
				else
				{
					cbxFormat.SelectedIndex = 0; // Устанавливаем значение по умолчанию, если парсинг не удался
				}
			}
			else
			{
				cbxFormat.SelectedIndex = 0; // Устанавливаем значение по умолчанию, если ключ не существует
			}
			Log.Write("Загрузка параметров из ini-файла завершена");
		}


		/// <summary>
		/// Обработка события загрузки формы в память
		/// </summary>
		private void frmMain_Load(object sender, EventArgs e)
		{
			rtbLog.Text = Log.Read();
			Log.Separate();
			Log.Write("Запуск приложения");

			ReadIniFile();
			LoadDepartments();

			// Выбираем узел по-умолчанию, если он сохранен в конфигурации
			string inputText = txtConfigNameId.Text;
			if (string.IsNullOrWhiteSpace(inputText))
			{
				Console.WriteLine("Поле не должно быть пустым.");
			}
			else
			{
				if (int.TryParse(inputText, out int configNameId))
				{
					// Если преобразование прошло успешно, вызываем метод
					SelectNodeByTag(configNameId);
				}
				else
				{
					Console.WriteLine("Введите корректное целое число.");
				}
			}

			// Выполняем экспорт в тихом режиме
			if (_silent)
			{
				this.Visible = false;
				this.ShowInTaskbar = false;
				this.WindowState = FormWindowState.Minimized;
				cmdExport_Click(sender, e);
			}
			else {
				// Если форма будет отображена, меняем заголовок и читаем лог в специальное поле
				this.Text = $"{txtConfigName.Text} (Конфигурация: {Program._department})";
				lblVersion.Text = $"Версия: {Application.ProductVersion}";
				TextBoxRead();
			}

		}

		/// <summary>
		/// Открыть форму для добавления элемента дерева или его изменения
		/// </summary>
		/// <param name="insert">Если true, форма открывается в режиме добавления элемента, иначе в режиме редактирования</param>
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
		/// Открыть форму для добавления записи о сенсорах или их изменении
		/// </summary>
		/// <param name="insert">Если true, форма открывается в режиме добавления элемента, иначе в режиме редактирования</param>
		private void OpenSensorForm(bool insert)
		{
			if (treeView.SelectedNode != null)
			{
				frmSensor form = new(insert, (int)treeView.SelectedNode.Tag, treeView.SelectedNode.Text, _config);
				form.ShowDialog();
			}
			LoadSensors();
		}


		/// <summary>
		/// Обновить дерево подразделений
		/// </summary>
		private void LoadDepartments()
		{
			int id = 0;
			if (treeView.SelectedNode != null)
			{
				id = (int)treeView.SelectedNode.Tag;
			}

			treeView.Nodes.Clear();
			TreeViewXmlLoader.LoadTreeViewFromXml(treeView, Program._department);
			SelectNodeByTag(id);
		}

		/// <summary>
		/// Функция обертка для загрузки сенсоров. Проверяет наличие выделенного элемента дерева
		/// </summary>
		private void LoadSensors()
		{
			if (treeView.SelectedNode != null)
			{
				LoadSensorForIdXML(Program._department, (int)treeView.SelectedNode.Tag);
			}
		}

		/// <summary>
		/// Функция обертка для загрузки сенсоров. Проверяет наличие выделенного элемента дерева
		/// </summary>
		/// <param name="xmlFilePath">Ссылка на конфигурационный файл XML</param>
		/// <param name="nodeId">Идентификатор подразделения</param>
		private void LoadSensorForIdXML(string xmlFilePath, int nodeId)
		{

			if (File.Exists(xmlFilePath))
			{
				XmlDocument doc = new();
				doc.Load(xmlFilePath); // Путь к вашему XML файлу
				listView.Items.Clear();

				XmlNode? node = doc.SelectSingleNode($"//Node[@Id='{nodeId}']");
				if (node != null)
				{
					XmlNodeList? sensorNodes = node.SelectNodes("Sensor");
					if (sensorNodes != null)
					{
						foreach (XmlNode sensorNode in sensorNodes)
						{
							var param = new ListViewItem(new[]
							{
								sensorNode.Attributes["Id"]?.Value ?? string.Empty,
								sensorNode.Attributes["Name"]?.Value ?? string.Empty,
								sensorNode.Attributes["Type"]?.Value ?? string.Empty,
								sensorNode.Attributes["Description"]?.Value ?? string.Empty
							})
							{
								Tag = nodeId
							};
							listView.Items.Add(param);
						}
					}
					else
					{
						// Обработка случая, когда узлы не найдены (если необходимо)
						Console.WriteLine("Нет узлов Sensor для обработки.");
						Log.Write("Нет узлов Sensor для обработки.");
					}
				}
			}
		}

		/// <summary>
		/// Поиск элемента дерева по идентификатору
		/// </summary>
		/// <param name="id">Код идентификатора</param>
		private void SelectNodeByTag(int id)
		{
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
		/// Рекурсивный метод для поиска в дочерних узлах
		/// </summary>
		/// <param name="parentNode">Ссылка на родителя в элементах дерева</param>
		/// <param name="id">Код идентификатора</param>
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

		/// <summary>
		/// Обработка события нажатия на кнопку "Закрыть"
		/// </summary>
		private void cmdClose_Click(object sender, EventArgs e)
		{
			Log.Write("Завершение работы приложения");
			Log.Separate();
			Application.Exit();
		}

		/// <summary>
		/// Обработка события закрытия формы
		/// </summary>
		private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
		{
			SaveIniFile();
		}

		/// <summary>
		/// Обработка события нажатия на кнопку "cmdSQLFileName"
		/// </summary>
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

		/// <summary>
		/// Обработка события нажатия на кнопку "cmdExport"
		/// </summary>
		private async void cmdExport_Click(object sender, EventArgs e)
		{
			try
			{
				if (cbxFormat.SelectedIndex == 0)
				{
					Log.Write("Выгружаем в файл формата Microsoft Excel");
					//TODO: Работает неоптимизированная функция. Нужно адаптировать
					await ExportToExcelAsync1();
				}
				else
				{
					//TODO: Работает неоптимизированная функция. Нужно адаптировать
					Log.Write("Выгружаем в файл формата CSV");
					await ExportToCSVAsync1();
				}
			}
			catch (Exception ex)
			{
				Log.Write($"Ошибка при экспорте: {ex.Message}");
				// Дополнительная обработка ошибок, если необходимо
			}
		}

		/// <summary>
		/// Записывает данные из DataTable в CSV-файл.
		/// </summary>
		/// <param name="dataTable">Ссылка на набор данных.</param>
		/// <param name="filePath">Путь до файла с разделителями.</param>
		public static void WriteDataTableToCsv(DataTable dataTable, string filePath)
		{
			try
			{
				using (StreamWriter writer = new(filePath))
				{
					// Запись заголовков столбцов
					for (int i = 0; i < dataTable.Columns.Count; i++)
					{
						writer.Write(dataTable.Columns[i]);
						if (i < dataTable.Columns.Count - 1)
						{
							writer.Write(","); // Разделитель
						}
					}
					writer.WriteLine(); // Переход на новую строку

					// Запись данных строк
					foreach (DataRow row in dataTable.Rows)
					{
						for (int i = 0; i < dataTable.Columns.Count; i++)
						{
							writer.Write(row[i].ToString());
							if (i < dataTable.Columns.Count - 1)
							{
								writer.Write(","); // Разделитель
							}
						}
						writer.WriteLine(); // Переход на новую строку
					}
				}

				Console.WriteLine($"Данные успешно записаны в файл: {filePath}");
				Log.Write($"Данные успешно записаны в файл: {filePath}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Произошла ошибка при записи в CSV: {ex.Message}");
				Log.Write($"Произошла ошибка при записи в CSV: {ex.Message}");
			}
		}

		/// <summary>
		/// Асинхронная функция для экспорта данных в CSV-файл.
		/// </summary>
		private async Task ExportToCSVAsync()
		{
			try
			{
				string strSQL = await ReadSqlFromFileAsync(txtSQLFileName.Text);
				await ExportDataAsync(txtDataSource.Text, txtUserID.Text, txtSQLDBPass.Text, txtInitialCatalog.Text, chkIntegratedSecurity.Checked, chkTrustServerCertificate.Checked, strSQL, cbxFormat.SelectedIndex);
				rtbLog.Text = Log.Read();
			}
			catch (FileNotFoundException fnfEx)
			{
				MessageBox.Show("Файл не найден: " + fnfEx.Message);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Произошла ошибка: " + ex.Message);
			}
		}

		/// <summary>
		/// Асинхронная функция для экспорта данных в Excel-файл.
		/// </summary>
		private async Task ExportToExcelAsync()
		{
			try
			{
				string strSQL = await ReadSqlFromFileAsync(txtSQLFileName.Text);
				await ExportDataAsync(txtDataSource.Text, txtUserID.Text, txtSQLDBPass.Text, txtInitialCatalog.Text, chkIntegratedSecurity.Checked, chkTrustServerCertificate.Checked, strSQL, cbxFormat.SelectedIndex);
				rtbLog.Text = Log.Read();
			}
			catch (FileNotFoundException fnfEx)
			{
				MessageBox.Show("Файл не найден: " + fnfEx.Message);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Произошла ошибка: " + ex.Message);
			}
		}

		/// <summary>
		/// Читает SQL-запрос из указанного файла.
		/// </summary>
		/// <param name="filePath">Путь к файлу, содержащему SQL-запрос.</param>
		/// <returns>Строка, содержащая SQL-запрос.</returns>
		private async Task<string> ReadSqlFromFileAsync(string filePath)
		{
			using StreamReader reader = new(filePath);
			return await reader.ReadToEndAsync();
		}

		/// <summary>
		/// Выполняет экспорт данных из базы данных в указанный формат (CSV или Excel).
		/// </summary>
		/// <param name="sDataSource">Источник данных для подключения к базе данных.</param>
		/// <param name="sUserID">Имя пользователя для подключения к базе данных.</param>
		/// <param name="sPassword">Пароль для подключения к базе данных.</param>
		/// <param name="sInitialCatalog">Имя базы данных.</param>
		/// <param name="bIntegratedSecurity">Флаг, указывающий, использовать ли интегрированную безопасность.</param>
		/// <param name="bTrustServerCertificate">Флаг, указывающий, доверять ли сертификату сервера.</param>
		/// <param name="strSQL">SQL-запрос для выполнения.</param>
		/// <param name="exportType">Тип экспорта (CSV или Excel).</param>
		/// <returns>Задача, представляющая асинхронную операцию.</returns>
		private async Task ExportDataAsync(string sDataSource, string sUserID, string sPassword, string sInitialCatalog, bool bIntegratedSecurity, bool bTrustServerCertificate, string strSQL, int exportType)
		{
			cmdExport.Enabled = false;

			SqlConnectionStringBuilder builder = new()
			{
				DataSource = sDataSource,
				UserID = sUserID,
				Password = sPassword,
				InitialCatalog = sInitialCatalog,
				IntegratedSecurity = bIntegratedSecurity,
				TrustServerCertificate = bTrustServerCertificate
			};

			LogConnectionDetails(sDataSource, sUserID, sInitialCatalog, strSQL);

			using SqlConnection connection = new(builder.ConnectionString);
			try
			{
				await connection.OpenAsync();
				string selectedIds = ReadSensorIds();
				DataTable dataTable = await ExecuteSqlQueryAsync(connection, strSQL, selectedIds, dtpStartData.Value.Date, dtpFinalData.Value.Date);

				if (exportType == 1)
				{
					string filePath = GenerateFilePath("output", ".csv");
					WriteDataTableToCsv(dataTable, filePath);
				}
				else if (exportType == 0)
				{
					string filePath = GenerateFilePath("output", ".xlsx");
					await WriteDataTableToExcelAsync(dataTable, filePath);
				}

				Log.Write("Выгрузка успешно завершена.");
				if (!_silent)
				{
					MessageBox.Show("Выгрузка успешно завершена");
				}
			}
			catch (SqlException e)
			{
				Log.Write($"Возникла ошибка во время выполнения sql-запроса: {e}");
				MessageBox.Show(e.ToString());
			}
			finally
			{
				cmdExport.Enabled = true;
			}
		}

		/// <summary>
		/// Логирует детали подключения к базе данных.
		/// </summary>
		/// <param name="sDataSource">Источник данных для подключения к базе данных.</param>
		/// <param name="sUserID">Имя пользователя для подключения к базе данных.</param>
		/// <param name="sInitialCatalog">Имя базы данных.</param>
		/// <param name="strSQL">SQL-запрос для выполнения.</param>
		private void LogConnectionDetails(string sDataSource, string sUserID, string sInitialCatalog, string strSQL)
		{
			Log.Write($"Источник данных (DataSource): {sDataSource}");
			Log.Write($"Имя пользователя (UserID): {sUserID}");
			Log.Write($"Пароль (Password): *********");
			Log.Write($"База данных (InitialCatalog): {sInitialCatalog}");
			Log.Write($"SQL-запрос к базе данных: {strSQL}");
		}

		/// <summary>
		/// Читает идентификаторы сенсоров из XML-файла.
		/// </summary>
		/// <returns>Строка, содержащая идентификаторы сенсоров, разделенные запятыми.</returns>
		private static string ReadSensorIds()
		{
			string xmlFilePath = Program._department;
			XmlDocument xmlDoc = new();
			xmlDoc.Load(xmlFilePath);
			XmlNodeList? sensorNodes = xmlDoc.SelectNodes("//Sensor");

			// Проверка на null и возврат пустой строки, если нет сенсоров
			if (sensorNodes == null || sensorNodes.Count == 0)
			{
				return string.Empty; // Или можно выбросить исключение, если это критично
			}

			return string.Join(",", sensorNodes.Cast<XmlNode>().Select(node => node.Attributes["Id"].Value));
		}

		/// <summary>
		/// Выполняет SQL-запрос и возвращает результаты в виде DataTable.
		/// </summary>
		/// <param name="connection">Соединение с базой данных.</param>
		/// <param name="strSQL">SQL-запрос для выполнения.</param>
		/// <param name="selectedIds">Идентификаторы сенсоров для фильтрации данных.</param>
		/// <param name="startDate">Дата начала для фильтрации данных.</param>
		/// <param name="endDate">Дата окончания для фильтрации данных.</param>
		/// <returns>DataTable, содержащий результаты выполнения SQL-запроса.</returns>
		private async Task<DataTable> ExecuteSqlQueryAsync(SqlConnection connection, string strSQL, string selectedIds, DateTime startDate, DateTime endDate)
		{
			DataTable dataTable = new();
			using SqlCommand command = new(strSQL, connection)
			{
				CommandTimeout = Int32.MaxValue // Установите значение в секундах
			};

			command.Parameters.AddWithValue("@selected_ids", selectedIds);
			command.Parameters.AddWithValue("@start_date", startDate);
			command.Parameters.AddWithValue("@end_date", endDate);

			using SqlDataAdapter adapter = new(command);
			await Task.Run(() => adapter.Fill(dataTable));
			return dataTable;
		}

		/// <summary>
		/// Генерирует путь к файлу для экспорта данных.
		/// </summary>
		/// <param name="fileNameWithoutExtension">Имя файла без расширения.</param>
		/// <param name="fileExtension">Расширение файла.</param>
		/// <returns>Строка, представляющая полный путь к файлу.</returns>
		private string GenerateFilePath(string fileNameWithoutExtension, string fileExtension)
		{
			IniFile iniFile = new(string.IsNullOrEmpty(_config) ? "config.ini" : $"config_{_config}.ini");
			string filePath = iniFile.ReadKey("FILENAME", "PathToResultFolder");
			string currentDateTime = DateTime.Now.ToString("yyyyMMdd_HH_mm_ss");

			if (chkResultsFileNameAddDate.Checked)
			{
				return $"{filePath}\\{fileNameWithoutExtension}_{currentDateTime}{fileExtension}";
			}
			else
			{
				return $"{filePath}\\{fileNameWithoutExtension}{fileExtension}";
			}
		}

		/// <summary>
		/// Записывает данные из DataTable в Excel-файл.
		/// </summary>
		/// <param name="dataTable">Ссылка на набор данных.</param>
		/// <param name="filePath">Путь до Excel-файла.</param>
		/// <returns>Задача, представляющая асинхронную операцию.</returns>
		private async Task WriteDataTableToExcelAsync(DataTable dataTable, string filePath)
		{
			ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

			using ExcelPackage excelPackage = new();
			ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet1");
			worksheet.Cells["A1"].LoadFromDataTable(dataTable, true);
			worksheet.Column(1).Style.Numberformat.Format = "dd.mm.yyyy HH:mm";
			worksheet.Row(1).Height = 75;
			worksheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
			worksheet.Row(1).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
			worksheet.Row(1).Style.WrapText = true;

			// Установка ширины всех колонок
			for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
			{
				worksheet.Column(col).Width = 11; // Установить ширину колонки на 11
			}
			worksheet.Column(1).Width = 20;
			worksheet.Cells["A1"].Value = "Время";
			worksheet.Cells["B1"].Value = "день";
			worksheet.Cells["C1"].Value = "месяц";
			worksheet.Cells["D1"].Value = "год";
			worksheet.Cells["E1"].Value = "час";
			worksheet.Cells["F1"].Value = "мин";

			// Сохранение Excel файла
			FileInfo excelFile = new(filePath);
			await excelPackage.SaveAsAsync(excelFile);

			worksheet.InsertRow(2, 1); // Вставляем одну пустую строку после первой строки
			worksheet.InsertRow(3, 1); // Вставляем одну пустую строку после второй строки

			// Чтение XML файла для получения кодов sensor и их названий

			string xmlFilePath = Program._department;
			XmlDocument xmlDoc = new();
			xmlDoc.Load(xmlFilePath);
			XmlNodeList? sensorNodes = xmlDoc.SelectNodes("//Sensor");

			// Проверка на null и возврат пустой строки, если нет сенсоров
			if (sensorNodes == null || sensorNodes.Count == 0)
			{
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
						string sprefix = sensorPrefix[header.Substring(1)];
						if (int.TryParse(sprefix, out int prefix))
						{
							worksheet.Cells[3, col].Value = prefix;
						}
						else
						{
							worksheet.Cells[3, col].Value = 0;
						}
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
			await excelPackage.SaveAsAsync(excelFile);
		}

		/// <summary>
		/// Асинхронная функция сохранения данных из базы данных в CSV-файл
		/// </summary>
		private async Task ExportToCSVAsync1()
		{
			try
			{
				using StreamReader reader = new(txtSQLFileName.Text);
				// Чтение данных из файла
				string strSQL = reader.ReadToEnd();

				await Main(txtDataSource.Text, txtUserID.Text, txtSQLDBPass.Text, txtInitialCatalog.Text, chkIntegratedSecurity.Checked, chkTrustServerCertificate.Checked, strSQL);
				rtbLog.Text = Log.Read();
			}
			catch (FileNotFoundException fnfEx)
			{
				// Обработка случая, когда файл не найден
				MessageBox.Show("Файл не найден: " + fnfEx.Message);
				// Дополнительные действия, например, уведомление пользователя
			}
			catch (Exception ex)
			{
				// Обработка других исключений
				MessageBox.Show("Произошла ошибка: " + ex.Message);
				// Дополнительные действия, например, логирование ошибки
			}

			async Task Main(string sDataSource, string sUserID, string sPassword, string sInitialCatalog, bool bIntegratedSecurity, bool bTrustServerCertificate, string strSQL)
			{
				cmdExport.Enabled = false;

				SqlConnectionStringBuilder builder = new()
				{
					DataSource = sDataSource,
					UserID = sUserID,
					Password = sPassword,
					InitialCatalog = sInitialCatalog,
					IntegratedSecurity = bIntegratedSecurity,
					TrustServerCertificate = bTrustServerCertificate
				};
				Log.Separate();
				Log.Write(string.Concat("Источник данных (DataSource): ", sDataSource));
				Log.Write(string.Concat("Имя пользователя (UserID): ", sUserID));
				Log.Write(string.Concat("Пароль (Password): ", "*********"));
				Log.Write(string.Concat("База данных (InitialCatalog): ", sInitialCatalog));
				Log.Write(string.Concat("SQL-запрос к базе данных: ", strSQL));

				using SqlConnection connection = new(builder.ConnectionString);
				try
				{
					await connection.OpenAsync();

					string xmlFilePath = Program._department;
					string selectedIds = "";

					XmlDocument xmlDoc = new();
					xmlDoc.Load(xmlFilePath);

					XmlNodeList? sensorNodes = xmlDoc.SelectNodes("//Sensor");
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

					Log.Write("Начинаем выполнение запроса. Ожидайте, запрос может выполняться ОЧЕНЬ продолжительное время.");
					TextBoxRead();
					int i = tabMain.SelectedIndex;
					tabMain.SelectedIndex = 4;
					// Подписываемся на событие RowsUpdated
					adapter.RowUpdated += Adapter_RowUpdated;

					//Stopwatch stopwatch = new();
					stopwatch.Reset();
					stopwatch.Start();

					timer = new System.Windows.Forms.Timer
					{
						Interval = 5000 // 5 секунд
					};
					timer.Tick += Timer_Tick;
					timer.Start();

					// Заполняем DataTable
					try
					{
						await Task.Run(() => adapter.Fill(dataTable));
					}
					catch (SqlException sqlEx)
					{
						// Обработка ошибок SQL
						Console.WriteLine("Произошла ошибка SQL: " + sqlEx.Message);
						// Дополнительные действия, например, логирование ошибки
						cmdExport.Enabled = true;
					}
					catch (Exception ex)
					{
						// Обработка других исключений
						Console.WriteLine("Произошла ошибка: " + ex.Message);
						// Дополнительные действия, например, логирование ошибки
						cmdExport.Enabled = true;
					}

					stopwatch.Stop();

					timer.Stop();
					TimeSpan elapsedTime = stopwatch.Elapsed;

					Log.Write($"Время выполнения запроса: {elapsedTime.TotalSeconds} секунд");
					TextBoxRead();

					// Отписываемся от события RowsUpdated
					adapter.RowUpdated -= Adapter_RowUpdated;

					tabMain.SelectedIndex = i;

					// Save Excel package to a file
					string filePath = iniFile.ReadKey("FILENAME", "PathToResultFolder");
					// Получаем текущую дату и время
					string currentDateTime = DateTime.Now.ToString("yyyyMMdd_HH_mm_ss");

					// Разделяем имя файла и расширение
					string fileNameWithoutExtension = "output";
					string fileExtension = ".csv";

					if (chkResultsFileNameAddDate.Checked)
					{
						// Формируем новое имя файла
						if (!string.IsNullOrEmpty(_config))
						{
							filePath = $"{filePath}\\{fileNameWithoutExtension}_{_config}_{currentDateTime}{fileExtension}";
						}
						else
						{
							filePath = $"{filePath}\\{fileNameWithoutExtension}_{currentDateTime}{fileExtension}";
						}
					}
					else
					{
						// Формируем новое имя файла
						if (!string.IsNullOrEmpty(_config))
						{
							filePath = $"{filePath}\\{fileNameWithoutExtension}_{_config}{fileExtension}";
						}
						else
						{
							filePath = $"{filePath}\\{fileNameWithoutExtension}{fileExtension}";
						}
					}

					WriteDataTableToCsv(dataTable, filePath);

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

					// Чтение данных из CSV
					List<string[]> csvData = [];
					using (StreamReader reader = new(filePath))
					{
						while (!reader.EndOfStream)
						{
							string? line = reader.ReadLine();
							string[] values = line.Split(',');
							csvData.Add(values);
						}
					}

					// Изменение заголовков столбцов
					if (csvData.Count > 0)
					{
						for (int col = 0; col < csvData[0].Length; col++)
						{
							string header = csvData[0][col];
							if (header.StartsWith("S") && sensorNames.ContainsKey(header.Substring(1)))
							{
								csvData[0][col] = sensorNames[header.Substring(1)];
								string sprefix = sensorPrefix[header.Substring(1)];
								if (int.TryParse(sprefix, out int prefix))
								{
									// Здесь можно добавить логику для записи префикса в нужную строку
									// Например, в строку 3 (индекс 2)
									if (csvData.Count > 2)
									{
										csvData[2][col] = prefix.ToString();
									}
								}
								else
								{
									if (csvData.Count > 2)
									{
										csvData[2][col] = "0";
									}
								}
							}
						}
					}

					// Запись измененных данных обратно в CSV
					using (StreamWriter writer = new(filePath))
					{
						foreach (var row in csvData)
						{
							writer.WriteLine(string.Join(",", row));
						}
					}

					Log.Write("Выгрузка успешно завершена.");
					TextBoxRead();
					if (!_silent)
					{
						MessageBox.Show("Выгрузка успешно завершена");
					}
					cmdExport.Enabled = true;
					if (_silent)
					{
						Application.Exit();
					}
				}
				catch (SqlException e)
				{
					Log.Write(string.Concat("Возникла ошибка во время выполнения sql-запроса: ", e.ToString()));
					MessageBox.Show(e.ToString());
					cmdExport.Enabled = true;
				}
			}
		}

		private async Task ExportToExcelAsync1() 
		{
			try
			{
				using StreamReader reader = new(txtSQLFileName.Text);
				// Чтение данных из файла
				string strSQL = reader.ReadToEnd();

				await Main(txtDataSource.Text, txtUserID.Text, txtSQLDBPass.Text, txtInitialCatalog.Text, chkIntegratedSecurity.Checked, chkTrustServerCertificate.Checked, strSQL);
				rtbLog.Text = Log.Read();
			}
			catch (FileNotFoundException fnfEx)
			{
				// Обработка случая, когда файл не найден
				MessageBox.Show("Файл не найден: " + fnfEx.Message);
				// Дополнительные действия, например, уведомление пользователя
			}
			catch (Exception ex)
			{
				// Обработка других исключений
				MessageBox.Show("Произошла ошибка: " + ex.Message);
				// Дополнительные действия, например, логирование ошибки
			}

			async Task Main(string sDataSource, string sUserID, string sPassword, string sInitialCatalog, bool bIntegratedSecurity, bool bTrustServerCertificate, string strSQL)
			{
				cmdExport.Enabled = false;

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

					string xmlFilePath = Program._department;
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

					Log.Write("Начинаем выполнение запроса. Ожидайте, запрос может выполняться ОЧЕНЬ продолжительное время.");
					TextBoxRead();
					int i = tabMain.SelectedIndex;
					tabMain.SelectedIndex = 4;
					// Подписываемся на событие RowsUpdated
					adapter.RowUpdated += Adapter_RowUpdated;

					//Stopwatch stopwatch = new();
					stopwatch.Reset();
					stopwatch.Start();

					timer = new System.Windows.Forms.Timer
					{
						Interval = 5000 // 5 секунд
					};
					timer.Tick += Timer_Tick;
					timer.Start();

					// Заполняем DataTable
					try
					{
						await Task.Run(() => adapter.Fill(dataTable));
					}
					catch (SqlException sqlEx)
					{
						// Обработка ошибок SQL
						Console.WriteLine("Произошла ошибка SQL: " + sqlEx.Message);
						// Дополнительные действия, например, логирование ошибки
						cmdExport.Enabled = true;
					}
					catch (Exception ex)
					{
						// Обработка других исключений
						Console.WriteLine("Произошла ошибка: " + ex.Message);
						// Дополнительные действия, например, логирование ошибки
						cmdExport.Enabled = true;
					}

					stopwatch.Stop();

					timer.Stop();
					TimeSpan elapsedTime = stopwatch.Elapsed;

					Log.Write($"Время выполнения запроса: {elapsedTime.TotalSeconds} секунд");
					TextBoxRead();

					// Отписываемся от события RowsUpdated
					adapter.RowUpdated -= Adapter_RowUpdated;

					tabMain.SelectedIndex = i;

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
						string filePath = iniFile.ReadKey("FILENAME", "PathToResultFolder");
						// Получаем текущую дату и время
						string currentDateTime = DateTime.Now.ToString("yyyyMMdd_HH_mm_ss");

						// Разделяем имя файла и расширение
						string fileNameWithoutExtension = "output";
						string fileExtension = ".xlsx";

						if (chkResultsFileNameAddDate.Checked)
						{
							// Формируем новое имя файла
							if (!string.IsNullOrEmpty(_config))
							{
								filePath = $"{filePath}\\{fileNameWithoutExtension}_{_config}_{currentDateTime}{fileExtension}";
							}
							else
							{
								filePath = $"{filePath}\\{fileNameWithoutExtension}_{currentDateTime}{fileExtension}";
							}
						}
						else
						{
							// Формируем новое имя файла
							if (!string.IsNullOrEmpty(_config))
							{
								filePath = $"{filePath}\\{fileNameWithoutExtension}_{_config}{fileExtension}";
							}
							else
							{
								filePath = $"{filePath}\\{fileNameWithoutExtension}{fileExtension}";
							}
						}

						Log.Write($"Файл выгружается в : {filePath}");
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
								string sprefix = sensorPrefix[header.Substring(1)];
								if (int.TryParse(sprefix, out int prefix))
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
					TextBoxRead();
					if (!_silent)
					{
						MessageBox.Show("Выгрузка успешно завершена");
					}
					cmdExport.Enabled = true;
					if (_silent)
					{
						Application.Exit();
					}
				}
				catch (SqlException e)
				{
					Log.Write(string.Concat("Возникла ошибка во время выполнения sql-запроса: ", e.ToString()));
					MessageBox.Show(e.ToString());
					cmdExport.Enabled = true;
				}
			}
		}

		private void Timer_Tick(object? sender, EventArgs e)
		{
			TimeSpan elapsedTime = stopwatch.Elapsed;
			UpdateTimeLabel(elapsedTime.TotalSeconds.ToString("F2")); // Обновляем метку на форме
		}

		private void UpdateTimeLabel(string time)
		{
			// Обновляем метку на форме
			Log.Write($"Время выполнения: {time} секунд");
			TextBoxRead();
		}

		private void TextBoxRead()
		{
			rtbLog.Text = Log.Read();
			rtbLog.SelectionStart = rtbLog.Text.Length;
			rtbLog.ScrollToCaret();
		}

		private void Adapter_RowUpdated(object sender, SqlRowUpdatedEventArgs e)
		{
			// Получаем количество обновленных строк
			int updatedRows = e.RecordsAffected;

			Log.Write($"Обработано {updatedRows} строк.");
			rtbLog.Text = Log.Read();
			rtbLog.SelectionStart = rtbLog.Text.Length;
			rtbLog.ScrollToCaret();

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
					RemoveNodeById(Program._department, (int)treeView.SelectedNode.Tag);
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
				configToolStripmenuItem.Enabled = true;
			}
			else
			{
				insertToolStripMenuItem.Enabled = true;
				updateToolStripMenuItem.Enabled = false;
				deleteToolStripMenuItem.Enabled = false;
				configToolStripmenuItem.Enabled = false;
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
			RemoveSensors(listView, Program._department);
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
			SaveIniFile();
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

			fbdResult.Description = "Выберите папку";
			fbdResult.ShowNewFolderButton = true; // Позволяет создавать новые папки

			// Показать диалог и проверить результат
			if (fbdResult.ShowDialog() == DialogResult.OK)
			{
				string selectedPath = fbdResult.SelectedPath;
				txtResultsFileName.Text = selectedPath;
				Console.WriteLine($"Выбрана папка: {selectedPath}");
			}
			else
			{
				Console.WriteLine("Выбор папки отменен.");
			}
		}

		private void cmdTasks_Click(object sender, EventArgs e)
		{

			WindowsIdentity identity = WindowsIdentity.GetCurrent();
			WindowsPrincipal principal = new(identity);
			if (principal.IsInRole(WindowsBuiltInRole.Administrator))
			{
				// Выполните код, требующий прав администратора
				// Путь к исполняемому файлу, который нужно запускать

				string exePath;

				if (!string.IsNullOrEmpty(_config))
				{
					exePath = System.Windows.Forms.Application.ExecutablePath + " /s" + $" /c {_config}";
				}
				else
				{
					exePath = System.Windows.Forms.Application.ExecutablePath + " /s";
				}

				// Получаем выбранный день недели
				int dayOfWeek = (int)nudExecPeriod.Value;

				string arguments = $"/CREATE /TN \"SCADAExport\" /RU \"SYSTEM\" /TR \"{exePath}\" /SC WEEKLY /D {GetDayOfWeekString(dayOfWeek)} /ST {dtpExecTime.Value:HH:mm}";

				DeleteTasks("SCADAExport");

				// Создаем объект ProcessStartInfo
				ProcessStartInfo startInfo = new()
				{
					FileName = "schtasks.exe",
					Arguments = arguments,
					UseShellExecute = false,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					Verb = "runas"
				};

				// Запускаем процесс
				using Process? process = Process.Start(startInfo);
				// Читаем вывод процесса
				string output = process.StandardOutput.ReadToEnd();
				string error = process.StandardError.ReadToEnd();

				// Проверяем результат
				if (process.ExitCode == 0)
				{
					Console.WriteLine("Задание успешно создано в планировщике.");
				}
				else
				{
					Console.WriteLine($"Ошибка при создании задания: {error}");
				}
			}
			else
			{
				// Пользователь не является администратором
				MessageBox.Show("Запустите приложение с правами администратора.");
			}
		}

		static string GetDayOfWeekString(int dayOfWeek)
		{
			return dayOfWeek switch
			{
				1 => "MON",
				2 => "TUE",
				3 => "WED",
				4 => "THU",
				5 => "FRI",
				6 => "SAT",
				7 => "SUN",
				_ => "",
			};
		}

		static void DeleteTasks(string taskName)
		{
			ProcessStartInfo startInfo = new()
			{
				FileName = "schtasks.exe",
				Arguments = $"/DELETE /TN \"{taskName}\" /F",
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true
			};

			using Process? process = Process.Start(startInfo);
			if (process != null)
			{
				process.WaitForExit();
				int exitCode = process.ExitCode;

				if (exitCode == 0)
				{
					Console.WriteLine($"Задача '{taskName}' успешно удалена.");
				}
				else
				{
					Console.WriteLine($"Неудалось удалить задачу '{taskName}'. Код ошибки: {exitCode}");
				}
			}
			else
			{
				Console.WriteLine("Не удалось запустить процесс.");
			}
		}

		private void configToolStripmenuItem_Click(object sender, EventArgs e)
		{
			if (treeView.SelectedNode != null)
			{
				txtConfigName.Text = treeView.SelectedNode.Text;
				txtConfigNameId.Text = treeView.SelectedNode.Tag.ToString();
			}
		}
	}
}