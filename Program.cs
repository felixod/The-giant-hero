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
			// ��������� ������� �����
			if (string.IsNullOrEmpty(workingDirectory))
			{
				throw new InvalidOperationException("������� ���������� �� ����� ���� null ��� ������.");
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