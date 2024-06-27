using System.Reflection;
using System.Windows.Forms;

namespace SQLBuilder
{
	internal static class Program
	{
		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			// To customize application configuration such as set high DPI settings or default font,
			// see https://aka.ms/applicationconfiguration.
			if (args.Length > 0 && args[0] == "/s")
			{
				Log.Separate();
				Log.Write("Запуск приложения в тихом режиме");

				string workingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
				// Установка рабочей папки
				Directory.SetCurrentDirectory(workingDirectory);

				ApplicationConfiguration.Initialize();
				Form frm = new frmMain(true)
				{
					Visible = false,
					ShowInTaskbar = false,
					WindowState = FormWindowState.Minimized
			};
				Application.Run(frm);
			}
			else
			{
				ApplicationConfiguration.Initialize();
				Application.Run(new frmMain(false));
			}

		}
	}
}