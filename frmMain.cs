using Microsoft.VisualBasic.ApplicationServices;
using SQLBuilder.ini;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;
using System.Diagnostics.Eventing.Reader;
using System.Data.Common;
using System.IO;
using System.Reflection.Metadata;
using System.Windows.Forms;

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
            rtbLog.Text = Log.Read();
            Log.Separate();
            Log.Write("Запуск приложения");
            IniFile iniFile = new IniFile("config.ini");
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
            // Загружаем из INI-файла название базы данных
            if (iniFile.KeyExists("FILENAME", "SQL_DB_InitialCatalog"))
                txtInitialCatalog.Text = iniFile.ReadKey("FILENAME", "SQL_DB_InitialCatalog");
            else
                txtInitialCatalog.Text = "";
            // Загружаем из INI-файла пароль пользователя базы данных
            if (iniFile.KeyExists("FILENAME", "SQL_DB_Pass"))
                txtSQLDBPass.Text = Decrypt(iniFile.ReadKey("FILENAME", "SQL_DB_Pass"), Enumerable.Range(0, 32).Select(x => (byte)x).ToArray());
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
            Log.Write("Загрузка параметров из ini-файла завершена");
        }

        private void cmdClose_Click(object sender, EventArgs e)
        {
            Log.Write("Завершение работы приложения");
            Log.Separate();
            Application.Exit();
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
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

            iniFile.WriteKey("FILENAME", "SQL_DB_DataSource", txtDataSource.Text);
            iniFile.WriteKey("FILENAME", "SQL_DB_UserID", txtUserID.Text);
            byte[] key = Enumerable.Range(0, 32).Select(x => (byte)x).ToArray();
            iniFile.WriteKey("FILENAME", "SQL_DB_Pass", Encrypt(txtSQLDBPass.Text, key));
            iniFile.WriteKey("FILENAME", "SQL_DB_InitialCatalog", txtInitialCatalog.Text);
            iniFile.WriteKey("FILENAME", "SQL_DB_IntegratedSecurity", chkIntegratedSecurity.Checked.ToString());
            iniFile.WriteKey("FILENAME", "SQL_DB_TrustServerCertificate", chkTrustServerCertificate.Checked.ToString());
            Log.Write("Запись параметров в ini-файл завершена");

        }

        private static string Encrypt(string text, byte[] key)
        {
            using Aes aes = Aes.Create();
            aes.Key = key;
            using MemoryStream ms = new();
            ms.Write(aes.IV);
            using (CryptoStream cs = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Write, true))
            {
                cs.Write(Encoding.UTF8.GetBytes(text));
            }
            Log.Write("Строка зашифрована");
            return Convert.ToBase64String(ms.ToArray());
        }

        private static string Decrypt(string base64, byte[] key)
        {
            using MemoryStream ms = new(Convert.FromBase64String(base64));
            byte[] iv = new byte[16];
            ms.Read(iv);
            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            using CryptoStream cs = new(ms, aes.CreateDecryptor(), CryptoStreamMode.Read, true);
            using MemoryStream output = new();
            cs.CopyTo(output);
            Log.Write("Строка дешифрована");
            return Encoding.UTF8.GetString(output.ToArray());
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
            _ = Main(txtDataSource.Text, txtUserID.Text, txtSQLDBPass.Text, txtInitialCatalog.Text, chkIntegratedSecurity.Checked, chkTrustServerCertificate.Checked, strSQL);
            rtbLog.Text = Log.Read();

            static async Task Main(string sDataSource, string sUserID, string sPassword, string sInitialCatalog, bool bIntegratedSecurity, bool bTrustServerCertificate, string strSQL)
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
                using (SqlConnection connection = new(builder.ConnectionString))
                {
                    try
                    {
                        await connection.OpenAsync();

                        using (SqlCommand command = new(strSQL, connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                string csv = reader.ToCSV(true, ";");
                                File.WriteAllText("Test.csv", csv);
                                Log.Write("Выгрузка успешно завершена.");
                                MessageBox.Show("Выгрузка успешно завершена");
                            }
                        }
                    }
                    catch (SqlException e)
                    {
                        Log.Write(string.Concat("Возникла ошибка во время выполнения sql-запроса: ", e.ToString()));
                        MessageBox.Show(e.ToString());
                    }

                }
            }
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

        private void cmdStructure_Click(object sender, EventArgs e)
        {
            var myForm = new frmStructure();
            //myForm.ShowDialog();
            myForm.Show();
        }
    }

    public static class Reader
	{
		public static string ToCSV(this IDataReader dataReader, bool includeHeaderAsFirstRow = true, string separator = ",")
		{
			DataTable dataTable = new();
			StringBuilder csvRows = new();
			string row = "";
			int columns;
			try
			{
				// Грузим в таблицу данные
				dataTable.Load(dataReader);
				// Получаем число столбцов
				columns = dataTable.Columns.Count;
				// Создаем заголовок
				if (includeHeaderAsFirstRow)
				{
					for (int index = 0; index < columns; index++)
					{
						row += (dataTable.Columns[index]);
						if (index < columns - 1)
							row += separator;
					}
					// Добавляем разделитель (новая строка)
					row += Environment.NewLine;
				}
				// Добавляем строку
				csvRows.Append(row);

				// Создаем текстровую строку из строки данных
				for (int rowIndex = 0; rowIndex < dataTable.Rows.Count; rowIndex++)
				{
					row = ""; // Очищаем переменную для новой строки
					for (int index = 0; index <= columns - 1; index++)
					{
						string value = dataTable.Rows[rowIndex][index].ToString();
						// Если тип столбца - строка
						if (dataTable.Rows[rowIndex][index] is string)
						{
							// Если в значении используются двойные кавычки, меняем каждый из них на двойные кавычки.
							if (value.Contains('"', StringComparison.CurrentCulture))
								value = value.Replace("\"", "\"\"");

							// Если в значении есть разделитель, окружаем его двойными кавычками.
							if (value.Contains(separator, StringComparison.CurrentCulture))
								value = "\"" + value + "\"";

							//Если строка содержит символы новой строки, удаляем их
							while (value.Contains('\r'))
							{
								value = value.Replace("\r", "");
							}
							while (value.Contains('\n'))
							{
								value = value.Replace("\n", "");
							}
						}
						row += value;
						// Добавляем разделитель
						if (index < columns - 1)
							row += separator;
					}
					// Удаляем разделитель после последнего столбца
					dataTable.Rows[rowIndex][columns - 1].ToString().ToString().Replace(separator, " ");
					row += Environment.NewLine;
					// Добавляем новую строку
					csvRows.Append(row);
				}
			}
			catch (Exception)
			{
				throw;
			}
			Log.Write("Создание CSV-файла завершено.");
			return csvRows.ToString();
		}
	}
}