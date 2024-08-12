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
			// To customize application configuration such as set high DPI settings or default font,
			// see https://aka.ms/applicationconfiguration.
			//if (args.Length > 0 && args[0] == "/s")
			//{
			//	Log.Separate();
			//	Log.Write("������ ���������� � ����� ������");

			//	string workingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			//	// ��������� ������� �����
			//	Directory.SetCurrentDirectory(workingDirectory);

			//	ApplicationConfiguration.Initialize();
			//	Form frm = new frmMain(true)
			//	{
			//		Visible = false,
			//		ShowInTaskbar = false,
			//		WindowState = FormWindowState.Minimized
			//};
			//	Application.Run(frm);
			//}
			//else
			//{
			//	ApplicationConfiguration.Initialize();
			//	Application.Run(new frmMain(false));
			//}

			string workingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			// ��������� ������� �����
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
						Log.Write("������ ���������� � ����� ������");
					}
					else if (args[i] == "/c" && i + 1 < args.Length)
					{
						configurationName = args[i + 1];
						Log.Write($"������������ ������������: {configurationName}");


						// ������ �������� ������������ �� ��������� ��� xml-�����
						if (!string.IsNullOrEmpty(configurationName))
						{
							_department = $"departments_{configurationName}.xml";
					
							string departmentFilePath = Path.Combine(workingDirectory, _department);
							string sourceFilePath = Path.Combine(workingDirectory, "default.dat");

							// �������� ������� �����
							if (!File.Exists(departmentFilePath))
							{
								// ���� ���� �� ����������, �������� default.dat � ����� ����
								File.Copy(sourceFilePath, departmentFilePath);
								Console.WriteLine($"���� '{departmentFilePath}' ��� ���������� �� 'default.dat'.");
							}
							else
							{
								Console.WriteLine($"���� '{departmentFilePath}' ��� ����������.");
							}
						}
						else
						{
							Console.WriteLine("��� ������������ �� ������ ��� �����.");
							// MessageBox.Show (configurationName);
							i++; // ���������� ��������� ��������, ��� ��� ��� �������� ������������
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

				// ����� ����� ������������ configurationName ��� ��������� ����������, ���� ��� ����������

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