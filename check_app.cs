// Проверка всех компонентов приложения SQLBuilder

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
    class AppChecker
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Проверка компонентов приложения SQLBuilder ===\n");

            // 1. Проверка основных файлов
            CheckFiles();
            
            // 2. Проверка основных классов
            CheckClasses();
            
            // 3. Проверка элементов управления для вкладки PSEE PPC
            CheckPseePpcControls();
            
            // 4. Проверка функциональности экспорта
            CheckExportFunctionality();
            
            Console.WriteLine("\n=== Проверка завершена ===");
        }

        static void CheckFiles()
        {
            Console.WriteLine("1. Проверка наличия основных файлов:");
            
            string[] requiredFiles = {
                "Program.cs",
                "frmMain.cs", 
                "frmMain.Designer.cs",
                "PseePpcExporter.cs",
                "ini/Crypt.cs",
                "ini/IniFile.cs",
                "Log.cs"
            };
            
            foreach (string file in requiredFiles)
            {
                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), file);
                bool exists = File.Exists(fullPath);
                Console.WriteLine($"   {(exists ? "+" : "-")} {file} {(exists ? "найден" : "ОТСУТСТВУЕТ!")}");
            }
            Console.WriteLine();
        }

        static void CheckClasses()
        {
            Console.WriteLine("2. Проверка основных классов:");
            
            // Проверка, что классы могут быть скомпилированы
            Type? mainFormType = typeof(frmMain);
            Type? pseePpcExporterType = typeof(PseePpcExporter);
            Type? iniFileType = typeof(IniFile);
            Type? cryptType = typeof(Crypt);
            Type? logType = typeof(Log);
            
            Console.WriteLine($"   + frmMain: {mainFormType?.Name ?? "НЕ НАЙДЕН"}");
            Console.WriteLine($"   + PseePpcExporter: {pseePpcExporterType?.Name ?? "НЕ НАЙДЕН"}");
            Console.WriteLine($"   + IniFile: {iniFileType?.Name ?? "НЕ НАЙДЕН"}");
            Console.WriteLine($"   + Crypt: {cryptType?.Name ?? "НЕ НАЙДЕН"}");
            Console.WriteLine($"   + Log: {logType?.Name ?? "НЕ НАЙДЕН"}");
            Console.WriteLine();
        }

        static void CheckPseePpcControls()
        {
            Console.WriteLine("3. Проверка элементов управления для вкладки PSEE PPC:");
            
            // Создаем форму для проверки элементов управления
            frmMain form = new frmMain();
            
            // Проверяем наличие элементов управления
            bool tabPageExists = form.GetType().GetField("tpgPseePpc", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance) != null;
                
            bool groupBoxExists = form.GetType().GetField("gpbPseePpc", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance) != null;
                
            bool exportButtonExists = form.GetType().GetField("cmdExportPseePpc", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance) != null;
                
            bool daysNumericUpDownExists = form.GetType().GetField("nudPseePpcDays", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance) != null;
                
            bool dateDateTimePickerExists = form.GetType().GetField("dtpPseePpcDate", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance) != null;
                
            bool formatComboBoxExists = form.GetType().GetField("cbxPseePpcFormat", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance) != null;
            
            Console.WriteLine($"   + Вкладка PSEE PPC: {(tabPageExists ? "+" : "-")} {(tabPageExists ? "найдена" : "ОТСУТСТВУЕТ!")}");
            Console.WriteLine($"   + Группировка: {(groupBoxExists ? "+" : "-")} {(groupBoxExists ? "найдена" : "ОТСУТСТВУЕТ!")}");
            Console.WriteLine($"   + Кнопка экспорта: {(exportButtonExists ? "+" : "-")} {(exportButtonExists ? "найдена" : "ОТСУТСТВУЕТ!")}");
            Console.WriteLine($"   + Поле дней: {(daysNumericUpDownExists ? "+" : "-")} {(daysNumericUpDownExists ? "найдено" : "ОТСУТСТВУЕТ!")}");
            Console.WriteLine($"   + Поле даты: {(dateDateTimePickerExists ? "+" : "-")} {(dateDateTimePickerExists ? "найдено" : "ОТСУТСТВУЕТ!")}");
            Console.WriteLine($"   + Комбобокс формата: {(formatComboBoxExists ? "+" : "-")} {(formatComboBoxExists ? "найден" : "ОТСУТСТВУЕТ!")}");
            Console.WriteLine();
        }

        static void CheckExportFunctionality()
        {
            Console.WriteLine("4. Проверка функциональности экспорта:");
            
            // Проверяем, что PseePpcExporter может быть создан
            try
            {
                // Создаем временный экземпляр (без подключения к БД)
                PseePpcExporter exporter = new PseePpcExporter(
                    "dummy_server", 
                    "dummy_user", 
                    "dummy_pass", 
                    "dummy_db", 
                    false, 
                    false
                );
                
                Console.WriteLine("   + PseePpcExporter может быть создан");
                
                // Проверяем, что методы экспорта существуют
                var exportToExcelMethod = typeof(PseePpcExporter).GetMethod("ExportToExcelAsync");
                var exportToCsvMethod = typeof(PseePpcExporter).GetMethod("ExportToCsvAsync");
                
                Console.WriteLine($"   + Метод ExportToExcelAsync: {(exportToExcelMethod != null ? "+" : "-")}");
                Console.WriteLine($"   + Метод ExportToCsvAsync: {(exportToCsvMethod != null ? "+" : "-")}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   - Ошибка при проверке PseePpcExporter: {ex.Message}");
            }
            
            Console.WriteLine();
        }
    }
}