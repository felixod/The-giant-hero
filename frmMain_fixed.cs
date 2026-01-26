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
            // Сохранение параметров для sp_psee_ppc
            iniFile.WriteKey("FILENAME", "PseePpc_Days", nudPseePpcDays.Value.ToString());
            iniFile.WriteKey("FILENAME", "PseePpc_Date", dtpPseePpcDate.Value.ToString("yyyy-MM-dd"));
            iniFile.WriteKey("FILENAME", "PseePpc_Format", cbxPseePpcFormat.SelectedIndex.ToString());
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
                Console.WriteLine($"Ошибка при загрузке значения доверия к сертификату сервера: {ex.Message}");
                Log.Write($"Ошибка при загрузке значения доверия к сертификату сервера: {ex.Message}");
                chkTrustServerCertificate.Checked = true; // Устанавливаем значение по умолчанию в случае ошибки
            }

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

            // Загружаем параметры для PSEE PPC
            try
            {
                if (iniFile.KeyExists("FILENAME", "PseePpc_Days"))
                {
                    string pseePpcDaysValue = iniFile.ReadKey("FILENAME", "PseePpc_Days");
                    
                    if (int.TryParse(pseePpcDaysValue, out int days))
                    {
                        nudPseePpcDays.Value = days;
                    }
                    else
                    {
                        nudPseePpcDays.Value = 4; // значение по умолчанию
                    }
                }
                else
                {
                    nudPseePpcDays.Value = 4; // значение по умолчанию
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке параметра PseePpc_Days: {ex.Message}");
                Log.Write($"Ошибка при загрузке параметра PseePpc_Days: {ex.Message}");
                nudPseePpcDays.Value = 4; // значение по умолчанию
            }

            try
            {
                if (iniFile.KeyExists("FILENAME", "PseePpc_Date"))
                {
                    string pseePpcDateValue = iniFile.ReadKey("FILENAME", "PseePpc_Date");
                    
                    if (DateTime.TryParse(pseePpcDateValue, out DateTime date))
                    {
                        dtpPseePpcDate.Value = date;
                    }
                    else
                    {
                        dtpPseePpcDate.Value = DateTime.Now; // значение по умолчанию
                    }
                }
                else
                {
                    dtpPseePpcDate.Value = DateTime.Now; // значение по умолчанию
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке параметра PseePpc_Date: {ex.Message}");
                Log.Write($"Ошибка при загрузке параметра PseePpc_Date: {ex.Message}");
                dtpPseePpcDate.Value = DateTime.Now; // значение по умолчанию
            }

            try
            {
                if (iniFile.KeyExists("FILENAME", "PseePpc_Format"))
                {
                    string pseePpcFormatValue = iniFile.ReadKey("FILENAME", "PseePpc_Format");
                    
                    if (int.TryParse(pseePpcFormatValue, out int formatIndex) && formatIndex >= 0 && formatIndex < cbxPseePpcFormat.Items.Count)
                    {
                        cbxPseePpcFormat.SelectedIndex = formatIndex;
                    }
                    else
                    {
                        cbxPseePpcFormat.SelectedIndex = 0; // значение по умолчанию
                    }
                }
                else
                {
                    cbxPseePpcFormat.SelectedIndex = 0; // значение по умолчанию
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке параметра PseePpc_Format: {ex.Message}");
                Log.Write($"Ошибка при загрузке параметра PseePpc_Format: {ex.Message}");
                cbxPseePpcFormat.SelectedIndex = 0; // значение по умолчанию
            }
        }

        private void IntegratedSecurity()
        {
            if (chkIntegratedSecurity.Checked)
            {
                txtUserID.Enabled = false;
                txtSQLDBPass.Enabled = false;
            }
            else
            {
                txtUserID.Enabled = true;
                txtSQLDBPass.Enabled = true;
            }
        }

        private void chkIntegratedSecurity_CheckedChanged(object sender, EventArgs e)
        {
            IntegratedSecurity();
        }

        private void cmdClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private async void cmdExport_Click(object sender, EventArgs e)
        {
            try
            {
                // Проверка заполнения полей
                if (string.IsNullOrEmpty(txtDataSource.Text))
                {
                    MessageBox.Show("Поле 'Сервер' должно быть заполнено.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(txtInitialCatalog.Text))
                {
                    MessageBox.Show("Поле 'Название БД' должно быть заполнено.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!chkIntegratedSecurity.Checked && string.IsNullOrEmpty(txtUserID.Text))
                {
                    MessageBox.Show("Поле 'Пользователь' должно быть заполнено.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!chkIntegratedSecurity.Checked && string.IsNullOrEmpty(txtSQLDBPass.Text))
                {
                    MessageBox.Show("Поле 'Пароль' должно быть заполнено.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(txtSQLFileName.Text))
                {
                    MessageBox.Show("Поле 'Имя файла' должно быть заполнено.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(txtResultsFileName.Text))
                {
                    MessageBox.Show("Поле 'Имя папки для результатов' должно быть заполнено.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Открываем соединение с базой данных
                SqlConnectionStringBuilder builder = new()
                {
                    DataSource = txtDataSource.Text,
                    UserID = txtUserID.Text,
                    Password = txtSQLDBPass.Text,
                    InitialCatalog = txtInitialCatalog.Text,
                    IntegratedSecurity = chkIntegratedSecurity.Checked,
                    TrustServerCertificate = chkTrustServerCertificate.Checked
                };

                Log.Write("Начало экспорта");
                Log.Write($"    Путь к SQL-файлу: {txtSQLFileName.Text}");
                Log.Write($"    Путь к папке с результатами: {txtResultsFileName.Text}");
                Log.Write($"    Начальная дата: {dtpStartData.Value:dd.MM.yyyy}");
                Log.Write($"    Конечная дата: {dtpFinalData.Value:dd.MM.yyyy}");

                // Читаем SQL-запрос из файла
                string sqlQuery = File.ReadAllText(txtSQLFileName.Text);

                // Заменяем плейсхолдеры в SQL-запросе
                sqlQuery = sqlQuery.Replace("{d1}", dtpStartData.Value.ToString("yyyy-MM-dd"));
                sqlQuery = sqlQuery.Replace("{d2}", dtpFinalData.Value.ToString("yyyy-MM-dd"));

                Log.Write("    Выполнение SQL-запроса:");

                using SqlConnection connection = new(builder.ConnectionString);
                await connection.OpenAsync();

                using SqlCommand command = new(sqlQuery, connection);
                command.CommandTimeout = int.MaxValue;

                using SqlDataAdapter adapter = new(command);
                DataTable dataTable = new();
                await Task.Run(() => adapter.Fill(dataTable));

                Log.Write($"    Кол-во строк в результате: {dataTable.Rows.Count}");
                Log.Write($"    Кол-во столбцов в результате: {dataTable.Columns.Count}");

                string connectionString = builder.ConnectionString;

                // Формирование имени файла
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(txtSQLFileName.Text);
                string currentDateTime = DateTime.Now.ToString("yyyyMMdd_HH_mm_ss");
                string filePath;

                if (chkResultsFileNameAddDate.Checked)
                {
                    filePath = $"{txtResultsFileName.Text}\\{fileNameWithoutExtension}_{currentDateTime}";
                }
                else
                {
                    filePath = $"{txtResultsFileName.Text}\\{fileNameWithoutExtension}";
                }

                if (cbxFormat.SelectedIndex == 0)
                {
                    Log.Write("    Microsoft Excel");
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    using ExcelPackage excelPackage = new();
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add(fileNameWithoutExtension);
                    worksheet.Cells["A1"].LoadFromDataTable(dataTable, true);

                    // Установка ширины колонок
                    for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                    {
                        worksheet.Column(col).Width = 20;
                    }

                    // Сохранение Excel файла
                    FileInfo excelFile = new($"{filePath}.xlsx");
                    await excelPackage.SaveAsAsync(excelFile);
                    Log.Write($"    Выгрузка в файл: {filePath}.xlsx");
                }
                else
                {
                    Log.Write("    CSV");
                    using StreamWriter writer = new($"{filePath}.csv");

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

                    Log.Write($"    Выгрузка в файл: {filePath}.csv");
                }

                Log.Write("Экспорт завершен");
                if (!_silent)
                {
                    MessageBox.Show("Экспорт завершен");
                }
            }
            catch (Exception ex)
            {
                Log.Write($"    Ошибка: {ex.Message}");
                if (!_silent)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            }
        }

        private async void cmdExportPseePpc_Click(object sender, EventArgs e)
        {
            try
            {
                PseePpcExporter exporter = new(
                        txtDataSource.Text,
                        txtUserID.Text,
                        txtSQLDBPass.Text,
                        txtInitialCatalog.Text,
                        chkIntegratedSecurity.Checked,
                        chkTrustServerCertificate.Checked,
                        _config,
                        _silent
                );

                int days = (int)nudPseePpcDays.Value;
                DateTime date = dtpPseePpcDate.Value;

                if (cbxPseePpcFormat.SelectedIndex == 0)
                {
                    Log.Write("    Microsoft Excel  sp_psee_ppc");
                    await exporter.ExportToExcelAsync(days, date);
                }
                else
                {
                    Log.Write("    CSV  sp_psee_ppc");
                    await exporter.ExportToCsvAsync(days, date);
                }
            }
            catch (Exception ex)
            {
                Log.Write($"    sp_psee_ppc: {ex.Message}");
                if (!_silent)
                {
                    MessageBox.Show($"  : {ex.Message}");
                }
            }
        }

        private void cmdSQLFileName_Click(object sender, EventArgs e)
        {
            ofdSQLFileName.Filter = "SQL Files (*.sql)|*.sql|All Files (*.*)|*.*";
            ofdSQLFileName.DefaultExt = "*.sql";
            ofdSQLFileName.FileName = "";

            if (ofdSQLFileName.ShowDialog() == DialogResult.OK)
            {
                txtSQLFileName.Text = ofdSQLFileName.FileName;
                Console.WriteLine(ofdSQLFileName.FileName);
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            ReadIniFile();
            LoadSensors();
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveIniFile();
        }

        /// <summary>
        /// Открыть форму для добавления записи или изменения департамента
        /// </summary>
        /// <param name="isInsert">Если true, то форма открывается для добавления, иначе - для изменения</param>
        private void OpenForm(bool isInsert)
        {
            // Получаем ID выбранного департамента
            int id = 0;
            if (treeView.SelectedNode != null)
            {
                id = (int)treeView.SelectedNode.Tag;
            }

            // Открываем форму
            if (isInsert)
            {
                // Если добавление, то передаем ID родительской записи
                frmInsertUpdateDeportament form = new(id, isInsert);
                form.ShowDialog();
            }
            else
            {
                // Если изменение, то передаем ID текущей записи
                frmInsertUpdateDeportament form = new(id, isInsert);
                form.ShowDialog();
            }
        }

        /// <summary>
        /// Открыть форму для добавления сенсора
        /// </summary>
        /// <param name="isInsert">Если true, то форма открывается для добавления, иначе - для изменения</param>
        private void OpenSensorForm(bool isInsert)
        {
            // Получаем ID выбранного департамента
            int id = 0;
            if (treeView.SelectedNode != null)
            {
                id = (int)treeView.SelectedNode.Tag;
            }

            // Открываем форму
            if (isInsert)
            {
                // Если добавление, то передаем ID родительской записи
                frmSensor form = new(id, isInsert);
                form.ShowDialog();
            }
            else
            {
                // Если изменение, то передаем ID текущей записи
                frmSensor form = new(id, isInsert);
                form.ShowDialog();
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

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Получаем ID выбранного департамента
            int id = 0;
            if (treeView.SelectedNode != null)
            {
                id = (int)treeView.SelectedNode.Tag;
            }

            // Удаляем департамент из XML-файла
            XDocument doc = XDocument.Load(Program._department);
            var departments = doc.Descendants("Department").Where(x => (int)x.Attribute("Id") == id).ToList();
            foreach (var department in departments)
            {
                department.Remove();
            }
            doc.Save(Program._department);

            // Обновляем дерево
            LoadTree();
        }

        private void LoadTree()
        {
            // Загрузка дерева департаментов из XML-файла
            XDocument doc = XDocument.Load(Program._department);
            treeView.Nodes.Clear();

            foreach (XElement element in doc.Root.Elements())
            {
                TreeNode node = new(element.Attribute("Name").Value)
                {
                    Tag = element.Attribute("Id").Value
                };
                LoadChildNodes(element, node);
                treeView.Nodes.Add(node);
            }
        }

        private void LoadChildNodes(XElement element, TreeNode node)
        {
            foreach (XElement childElement in element.Elements())
            {
                TreeNode childNode = new(childElement.Attribute("Name").Value)
                {
                    Tag = childElement.Attribute("Id").Value
                };
                LoadChildNodes(childElement, childNode);
                node.Nodes.Add(childNode);
            }
        }

        private void LoadSensors()
        {
            // Очищаем список
            listView.Items.Clear();

            // Проверяем, что выбран узел в дереве
            if (treeView.SelectedNode == null)
            {
                return;
            }

            // Загружаем сенсоры из XML-файла
            XDocument doc = XDocument.Load(Program._department);
            
            // Находим все сенсоры для выбранного департамента
            var sensors = doc.Descendants("Sensor")
                .Where(x => (int)x.Attribute("DepartmentId") == int.Parse((string)treeView.SelectedNode.Tag));
            
            foreach (var sensor in sensors)
            {
                ListViewItem item = new(sensor.Attribute("Id").Value);
                item.SubItems.Add(sensor.Attribute("Name").Value);
                item.SubItems.Add(sensor.Attribute("Type").Value);
                item.SubItems.Add(sensor.Attribute("Description").Value);
                
                listView.Items.Add(item);
            }
        }

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

                string arguments = $"/CREATE /TN \\\"SCADAExport\\\" /RU \\\"SYSTEM\\\" /TR \\\"{exePath}\\\" /SC WEEKLY /D {GetDayOfWeekString(dayOfWeek)} /ST {dtpExecTime.Value:HH:mm}";

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
                Arguments = $"/DELETE /TN \\\"{taskName}\\\" /F",
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