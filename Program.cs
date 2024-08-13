using SQLBuilder.ini;
using System.Reflection;
using System.Windows.Forms;

namespace SQLBuilder
{
	internal static class Program
	{
		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		/// 

		public static string _department = "departments.xml";

		[STAThread]
		static void Main(string[] args)
		{
			string? workingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			// Установка рабочей папки
			if (string.IsNullOrEmpty(workingDirectory))
			{
				throw new InvalidOperationException("Рабочая директория не может быть null или пустой.");
			}
			Directory.SetCurrentDirectory(workingDirectory);

			if (args.Length > 0)
			{
				bool isSilentMode = false;
				string? configurationName = null;

				for (int i = 0; i < args.Length; i++)
				{
					if (args[i] == "/s")
					{
						isSilentMode = true;
						Log.Separate();
						Log.Write("Запуск приложения в тихом режиме");
					}
					else if (args[i] == "/c" && i + 1 < args.Length)
					{
						configurationName = args[i + 1];
						Log.Write($"Используется конфигурация: {configurationName}");


						// Меняем название конфигурации по умолчанию для xml-файла
						if (!string.IsNullOrEmpty(configurationName))
						{
							_department = $"departments_{configurationName}.xml";
					
							string departmentFilePath = Path.Combine(workingDirectory, _department);
							string sourceFilePath = Path.Combine(workingDirectory, "default.dat");

							// Проверка наличия файла
							if (!File.Exists(departmentFilePath))
							{
								// Если файл не существует, копируем default.dat в новый файл
								File.Copy(sourceFilePath, departmentFilePath);
								Console.WriteLine($"Файл '{departmentFilePath}' был скопирован из 'default.dat'.");
							}
							else
							{
								Console.WriteLine($"Файл '{departmentFilePath}' уже существует.");
							}
						}
						else
						{
							Console.WriteLine("Имя конфигурации не задано или пусто.");
							i++; // Пропускаем следующий аргумент, так как это название конфигурации
						}
					}
				}


				ApplicationConfiguration.Initialize();

				Form frm = new frmMain(isSilentMode, configurationName)
				{
					Visible = !isSilentMode,
					ShowInTaskbar = !isSilentMode,
					WindowState = isSilentMode ? FormWindowState.Minimized : FormWindowState.Normal
				};

				// Здесь можно использовать configurationName для настройки приложения, если это необходимо

				Application.Run(frm);
			}
			else
			{
				ApplicationConfiguration.Initialize();
				Application.Run(new frmMain(false, String.Empty));
			}

		}
	}
}