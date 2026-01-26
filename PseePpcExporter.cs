using Microsoft.Data.SqlClient;
using OfficeOpenXml;
using SQLBuilder.ini;
using System.Data;

namespace SQLBuilder
{
    /// <summary>
    /// Класс для экспорта данных из хранимой процедуры sp_psee_ppc
    /// </summary>
    public class PseePpcExporter
    {
        private readonly string _connectionString;
        private readonly string _config;
        private readonly bool _silent;

        public PseePpcExporter(string dataSource, string userId, string password, string initialCatalog, 
                              bool integratedSecurity, bool trustServerCertificate, string config = "", bool silent = false)
        {
            SqlConnectionStringBuilder builder = new()
            {
                DataSource = dataSource,
                UserID = userId,
                Password = password,
                InitialCatalog = initialCatalog,
                IntegratedSecurity = integratedSecurity,
                TrustServerCertificate = trustServerCertificate
            };
            
            _connectionString = builder.ConnectionString;
            _config = config;
            _silent = silent;
        }

        /// <summary>
        /// Экспорт данных из хранимой процедуры в CSV файл
        /// </summary>
        /// <param name="days">Глубина дней</param>
        /// <param name="dt">Дата отсчета (если null, используется текущая дата)</param>
        public async Task ExportToCsvAsync(int days = 4, DateTime? dt = null)
        {
            try
            {
                Log.Write("Начало экспорта в CSV из хранимой процедуры sp_psee_ppc");
                
                DataTable dataTable = await ExecuteStoredProcedureAsync(days, dt ?? DateTime.Now);
                
                string filePath = GenerateFilePath("sp_psee_ppc_output", ".csv");
                WriteDataTableToCsv(dataTable, filePath);
                
                Log.Write($"Экспорт в CSV завершен. Файл: {filePath}");
                
                if (!_silent)
                {
                    MessageBox.Show($"Экспорт в CSV завершен. Файл: {filePath}");
                }
            }
            catch (Exception ex)
            {
                Log.Write($"Ошибка при экспорте в CSV: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Экспорт данных из хранимой процедуры в Excel файл
        /// </summary>
        /// <param name="days">Глубина дней</param>
        /// <param name="dt">Дата отсчета (если null, используется текущая дата)</param>
        public async Task ExportToExcelAsync(int days = 4, DateTime? dt = null)
        {
            try
            {
                Log.Write("Начало экспорта в Excel из хранимой процедуры sp_psee_ppc");
                
                DataTable dataTable = await ExecuteStoredProcedureAsync(days, dt ?? DateTime.Now);
                
                string filePath = GenerateFilePath("sp_psee_ppc_output", ".xlsx");
                await WriteDataTableToExcelAsync(dataTable, filePath);
                
                Log.Write($"Экспорт в Excel завершен. Файл: {filePath}");
                
                if (!_silent)
                {
                    MessageBox.Show($"Экспорт в Excel завершен. Файл: {filePath}");
                }
            }
            catch (Exception ex)
            {
                Log.Write($"Ошибка при экспорте в Excel: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Выполнение хранимой процедуры sp_psee_ppc
        /// </summary>
        /// <param name="days">Глубина дней</param>
        /// <param name="dt">Дата отсчета</param>
        /// <returns>DataTable с результатами</returns>
        private async Task<DataTable> ExecuteStoredProcedureAsync(int days, DateTime dt)
        {
            DataTable dataTable = new();
            
            using SqlConnection connection = new(_connectionString);
            await connection.OpenAsync();
            
            using SqlCommand command = new("sp_psee_ppc", connection)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = int.MaxValue
            };
            
            command.Parameters.AddWithValue("@days", days);
            command.Parameters.AddWithValue("@dt", dt);
            
            using SqlDataAdapter adapter = new(command);
            await Task.Run(() => adapter.Fill(dataTable));
            
            return dataTable;
        }

        /// <summary>
        /// Запись DataTable в CSV файл
        /// </summary>
        /// <param name="dataTable">Таблица данных</param>
        /// <param name="filePath">Путь к файлу</param>
        public static void WriteDataTableToCsv(DataTable dataTable, string filePath)
        {
            try
            {
                using StreamWriter writer = new(filePath);
                
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

                Console.WriteLine($"Данные успешно записаны в файл: {filePath}");
                Log.Write($"Данные успешно записаны в файл: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка при записи в CSV: {ex.Message}");
                Log.Write($"Произошла ошибка при записи в CSV: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Запись DataTable в Excel файл
        /// </summary>
        /// <param name="dataTable">Таблица данных</param>
        /// <param name="filePath">Путь к файлу</param>
        private async Task WriteDataTableToExcelAsync(DataTable dataTable, string filePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using ExcelPackage excelPackage = new();
            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("sp_psee_ppc_data");
            
            worksheet.Cells["A1"].LoadFromDataTable(dataTable, true);
            
            // Установка ширины колонок
            for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
            {
                worksheet.Column(col).Width = 20;
            }
            
            // Сохранение Excel файла
            FileInfo excelFile = new(filePath);
            await excelPackage.SaveAsAsync(excelFile);
        }

        /// <summary>
        /// Генерация пути к файлу
        /// </summary>
        /// <param name="fileNameWithoutExtension">Имя файла без расширения</param>
        /// <param name="fileExtension">Расширение файла</param>
        /// <returns>Полный путь к файлу</returns>
        private string GenerateFilePath(string fileNameWithoutExtension, string fileExtension)
        {
            IniFile iniFile = new(string.IsNullOrEmpty(_config) ? "config.ini" : $"config_{_config}.ini");
            string filePath = iniFile.ReadKey("FILENAME", "PathToResultFolder");
            string currentDateTime = DateTime.Now.ToString("yyyyMMdd_HH_mm_ss");

            if (iniFile.KeyExists("FILENAME", "AddDateToFileResult"))
            {
                string addDateToFileResultString = iniFile.ReadKey("FILENAME", "AddDateToFileResult");
                if (bool.TryParse(addDateToFileResultString, out bool addDateToFileResult) && addDateToFileResult)
                {
                    return $"{filePath}\\{fileNameWithoutExtension}_{currentDateTime}{fileExtension}";
                }
            }
            
            return $"{filePath}\\{fileNameWithoutExtension}{fileExtension}";
        }
    }
}