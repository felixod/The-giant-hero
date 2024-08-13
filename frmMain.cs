
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
		/// ����������� ������ �������� ����� ����������
		/// </summary>
		/// <param name="silent">���� true, ���������� ����������� � ����� ������ ��� ����������� ������������ ����������</param>
		/// <param name="config">���� �� null, ���������� ����������� � ��������� ������������� ����������</param>
		public frmMain(bool silent = false, string? config = null)
		{
			_silent = silent;
			_config = config;

			// �������� �������� ���� ����� ��-���������
			if (!string.IsNullOrEmpty(_config))
			{
				Log.ConfigPrefix(_config);
			}
			InitializeComponent();

		}


		/// <summary>
		/// ���������� ������� �������� ��������� � ���������������� ����
		/// </summary>
		private void SaveIniFile()
		{
			// ������������ ���������������� ���� � ���������, ���� ������� �������
			IniFile iniFile = new(string.IsNullOrEmpty(_config) ? "config.ini" : $"config_{_config}.ini");

			// ������ SCHEDULE
			iniFile.WriteKey("SCHEDULE", "Start_data", dtpStartExecution.Value.Date.ToLongDateString());
			iniFile.WriteKey("SCHEDULE", "Execution_time", dtpExecTime.Value.ToShortTimeString());
			iniFile.WriteKey("SCHEDULE", "Execution_period", nudExecPeriod.Value.ToString());
			// ������ INTERVAL
			iniFile.WriteKey("INTERVAL", "Start_data", dtpStartData.Value.Date.ToLongDateString());
			iniFile.WriteKey("INTERVAL", "Final_data", dtpFinalData.Value.Date.ToLongDateString());
			// TODO ��� ���-�� ��� ����������� ������ � ����������� �������, ���� ���������
			// ������ FILENAME
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
			Log.Write("������ ���������� � ini-���� ���������");
		}

		/// <summary>
		/// ������ ��������� �� ini-����� � ��������� ���������� �������� �����
		/// </summary>
		private void ReadIniFile()
		{
			// ������������ ���������������� ���� � ���������, ���� ������� �������
			IniFile iniFile = new(string.IsNullOrEmpty(_config) ? "config.ini" : $"config_{_config}.ini");

			// ������ SCHEDULE
			// ��������� �� INI-����� ��������� ����
			try
			{
				if (iniFile.KeyExists("SCHEDULE", "Start_data"))
				{
					string startData = iniFile.ReadKey("SCHEDULE", "Start_data");
					// ���������, �������� �� ������ ���������� �����
					if (DateTime.TryParse(startData, out DateTime parsedDate))
					{
						dtpStartExecution.Value = parsedDate;
					}
					else
					{
						// ���� ������ �� �������� ���������� �����, ������������� ������� �����
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
				// ��������� ����������, ��������, ����������� ������
				Console.WriteLine($"������: {ex.Message}");
				Log.Write($"������: {ex.Message}");
				dtpStartExecution.Value = DateTime.Now; // ������������� ������� ����� � ������ ������
			}
			try
			{
				// ���������, ���������� �� ���� "Execution_time" � ������ "SCHEDULE"
				if (iniFile.KeyExists("SCHEDULE", "Execution_time"))
				{
					string executionTimeString = iniFile.ReadKey("SCHEDULE", "Execution_time");

					// ���������, �������� �� ������ ���������� �����/��������
					if (DateTime.TryParse(executionTimeString, out DateTime executionTime))
					{
						// ������������� ����� ����������, �������� ����� � ������� ����
						dtpExecTime.Value = DateTime.Now.Date.Add(executionTime.TimeOfDay);
					}
					else
					{
						// ���� ������ �� �������� ���������� �����/��������, ������������� ������� �����
						Console.WriteLine($"������: '{executionTimeString}' �� �������� ���������� ��������. ������������� ������� �����.");
						dtpExecTime.Value = DateTime.Now;
					}
				}
				else
				{
					// ���� ���� �� ����������, ������������� ������� �����
					dtpExecTime.Value = DateTime.Now;
				}
			}
			catch (Exception ex)
			{
				// ��������� ����������, ��������, ����������� ������
				Console.WriteLine($"������: {ex.Message}");
				Log.Write($"������: {ex.Message}");
				dtpExecTime.Value = DateTime.Now; // ������������� ������� ����� � ������ ������
			}
			try
			{
				// ���������, ���������� �� ���� "Execution_period" � ������ "SCHEDULE"
				if (iniFile.KeyExists("SCHEDULE", "Execution_period"))
				{
					string executionPeriodString = iniFile.ReadKey("SCHEDULE", "Execution_period");

					// ���������, �������� �� ������ ���������� ����� ������
					if (int.TryParse(executionPeriodString, out int executionPeriod))
					{
						// ������������� ������ ����������
						nudExecPeriod.Value = executionPeriod;
					}
					else
					{
						// ���� ������ �� �������� ���������� ����� ������, ������������� �������� �� ���������
						Console.WriteLine($"������: '{executionPeriodString}' �� �������� ���������� ����� ������. ������������� �������� �� ��������� 1.");
						Log.Write($"������: '{executionPeriodString}' �� �������� ���������� ����� ������. ������������� �������� �� ��������� 1.");
						nudExecPeriod.Value = 1;
					}
				}
				else
				{
					// ���� ���� �� ����������, ������������� �������� �� ���������
					nudExecPeriod.Value = 1;
				}
			}
			catch (Exception ex)
			{
				// ��������� ����������, ��������, ����������� ������
				Console.WriteLine($"������ ��� �������� ������� ����������: {ex.Message}");
				Log.Write($"������ ��� �������� ������� ����������: {ex.Message}");
				nudExecPeriod.Value = 1; // ������������� �������� �� ��������� � ������ ������
			}

			// ������ INTERVAL
			try
			{
				// ���������, ���������� �� ���� "Start_data" � ������ "INTERVAL"
				if (iniFile.KeyExists("INTERVAL", "Start_data"))
				{
					string startDataString = iniFile.ReadKey("INTERVAL", "Start_data");

					// ���������, �������� �� ������ ���������� �����
					if (DateTime.TryParse(startDataString, out DateTime startData))
					{
						// ������������� ��������� ���� �������
						dtpStartData.Value = startData;
					}
					else
					{
						// ���� ������ �� �������� ���������� �����, ������������� ������� �����
						Console.WriteLine($"������: '{startDataString}' �� �������� ���������� �����. ������������� ������� �����.");
						Log.Write($"������: '{startDataString}' �� �������� ���������� �����. ������������� ������� �����.");
						dtpStartData.Value = DateTime.Now;
					}
				}
				else
				{
					// ���� ���� �� ����������, ������������� ������� �����
					dtpStartData.Value = DateTime.Now;
				}
			}
			catch (Exception ex)
			{
				// ��������� ����������, ��������, ����������� ������
				Console.WriteLine($"������ ��� �������� ��������� ���� �������: {ex.Message}");
				Log.Write($"������ ��� �������� ��������� ���� �������: {ex.Message}");
				dtpStartData.Value = DateTime.Now; // ������������� ������� ����� � ������ ������
			}

			try
			{
				// ���������, ���������� �� ���� "Final_data" � ������ "INTERVAL"
				if (iniFile.KeyExists("INTERVAL", "Final_data"))
				{
					string finalDataString = iniFile.ReadKey("INTERVAL", "Final_data");

					// ���������, �������� �� ������ ���������� �����
					if (DateTime.TryParse(finalDataString, out DateTime finalData))
					{
						// ������������� �������� ���� �������
						dtpFinalData.Value = finalData;
					}
					else
					{
						// ���� ������ �� �������� ���������� �����, ������������� ������� �����
						Console.WriteLine($"������: '{finalDataString}' �� �������� ���������� �����. ������������� ������� �����.");
						Log.Write($"������: '{finalDataString}' �� �������� ���������� �����. ������������� ������� �����.");
						dtpFinalData.Value = DateTime.Now;
					}
				}
				else
				{
					// ���� ���� �� ����������, ������������� ������� �����
					dtpFinalData.Value = DateTime.Now;
				}
			}
			catch (Exception ex)
			{
				// ��������� ����������, ��������, ����������� ������
				Console.WriteLine($"������ ��� �������� �������� ���� �������: {ex.Message}");
				Log.Write($"������ ��� �������� �������� ���� �������: {ex.Message}");
				dtpFinalData.Value = DateTime.Now; // ������������� ������� ����� � ������ ������
			}

			// ������ FILENAME
			// ��������� �� INI-����� ������������ � ��� ������������
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

			// ��������� �� INI-����� ��� ����� � SQL-��������
			if (iniFile.KeyExists("FILENAME", "SQL_File_Name"))
				txtSQLFileName.Text = iniFile.ReadKey("FILENAME", "SQL_File_Name");
			else
				txtSQLFileName.Text = "";

			// ��������� �� INI-����� ���� �� ����� � ������������
			if (iniFile.KeyExists("FILENAME", "PathToResultFolder"))
				txtResultsFileName.Text = iniFile.ReadKey("FILENAME", "PathToResultFolder");
			else
				txtResultsFileName.Text = "";

			try
			{
				// ���������, ���������� �� ���� "AddDateToFileResult" � ������ "FILENAME"
				if (iniFile.KeyExists("FILENAME", "AddDateToFileResult"))
				{
					string addDateToFileResultString = iniFile.ReadKey("FILENAME", "AddDateToFileResult");

					// ���������, �������� �� ������ ���������� ������� ���������
					if (bool.TryParse(addDateToFileResultString, out bool addDateToFileResult))
					{
						// ������������� �������� ��������
						chkResultsFileNameAddDate.Checked = addDateToFileResult;
					}
					else
					{
						// ���� ������ �� �������� ���������� ������� ���������, ������������� �������� �� ���������
						Console.WriteLine($"������: '{addDateToFileResultString}' �� �������� ���������� ������� ���������. ������������� �������� �� ��������� (false).");
						Log.Write($"������: '{addDateToFileResultString}' �� �������� ���������� ������� ���������. ������������� �������� �� ��������� (false).");
						chkResultsFileNameAddDate.Checked = false;
					}
				}
				else
				{
					// ���� ���� �� ����������, ������������� �������� �� ���������
					chkResultsFileNameAddDate.Checked = false;
				}
			}
			catch (Exception ex)
			{
				// ��������� ����������, ��������, ����������� ������
				Console.WriteLine($"������ ��� �������� �������� ���������� ���� � ����� ����� ����������: {ex.Message}");
				Log.Write($"������ ��� �������� �������� ���������� ���� � ����� ����� ����������: {ex.Message}");
				chkResultsFileNameAddDate.Checked = false; // ������������� �������� �� ��������� � ������ ������
			}

			// ��������� �� INI-����� �������� ���� ������
			if (iniFile.KeyExists("FILENAME", "SQL_DB_InitialCatalog"))
				txtInitialCatalog.Text = iniFile.ReadKey("FILENAME", "SQL_DB_InitialCatalog");
			else
				txtInitialCatalog.Text = "";

			// ��������� �� INI-����� ������ ������������ ���� ������
			if (iniFile.KeyExists("FILENAME", "SQL_DB_Pass"))
				txtSQLDBPass.Text = Crypt.Decrypt(iniFile.ReadKey("FILENAME", "SQL_DB_Pass"), Enumerable.Range(0, 32).Select(x => (byte)x).ToArray());
			else
				txtSQLDBPass.Text = "";

			// ��������� �� INI-����� ��� ������������ ���� ������
			if (iniFile.KeyExists("FILENAME", "SQL_DB_UserID"))
				txtUserID.Text = iniFile.ReadKey("FILENAME", "SQL_DB_UserID");
			else
				txtUserID.Text = "";

			// ��������� �� INI-����� ��� ��� ip-����� �������
			if (iniFile.KeyExists("FILENAME", "SQL_DB_DataSource"))
				txtDataSource.Text = iniFile.ReadKey("FILENAME", "SQL_DB_DataSource");
			else
				txtDataSource.Text = "";

			// ��������� �� INI-����� ��� ������������ �����������
			try
			{
				// ���������, ���������� �� ���� "SQL_DB_IntegratedSecurity" � ������ "FILENAME"
				if (iniFile.KeyExists("FILENAME", "SQL_DB_IntegratedSecurity"))
				{
					string integratedSecurityString = iniFile.ReadKey("FILENAME", "SQL_DB_IntegratedSecurity");

					// ���������, �������� �� ������ ���������� ������� ���������
					if (bool.TryParse(integratedSecurityString, out bool integratedSecurity))
					{
						// ������������� �������� ��������
						chkIntegratedSecurity.Checked = integratedSecurity;
					}
					else
					{
						// ���� ������ �� �������� ���������� ������� ���������, ������������� �������� �� ���������
						Console.WriteLine($"������: '{integratedSecurityString}' �� �������� ���������� ������� ���������. ������������� �������� �� ��������� (false).");
						Log.Write($"������: '{integratedSecurityString}' �� �������� ���������� ������� ���������. ������������� �������� �� ��������� (false).");
						chkIntegratedSecurity.Checked = false;
					}
				}
				else
				{
					// ���� ���� �� ����������, ������������� �������� �� ���������
					chkIntegratedSecurity.Checked = false;
				}
			}
			catch (Exception ex)
			{
				// ��������� ����������, ��������, ����������� ������
				Console.WriteLine($"������ ��� �������� ���� ������������ �����������: {ex.Message}");
				Log.Write($"������ ��� �������� ���� ������������ �����������: {ex.Message}");
				chkIntegratedSecurity.Checked = false; // ������������� �������� �� ��������� � ������ ������
				
			}
			IntegratedSecurity();

			// ��������� �� INI-����� ������� � ����������� �������
			try
			{
				// ���������, ���������� �� ���� "SQL_DB_TrustServerCertificate" � ������ "FILENAME"
				if (iniFile.KeyExists("FILENAME", "SQL_DB_TrustServerCertificate"))
				{
					string trustServerCertificateString = iniFile.ReadKey("FILENAME", "SQL_DB_TrustServerCertificate");

					// ���������, �������� �� ������ ���������� ������� ���������
					if (bool.TryParse(trustServerCertificateString, out bool trustServerCertificate))
					{
						// ������������� �������� ��������
						chkTrustServerCertificate.Checked = trustServerCertificate;
					}
					else
					{
						// ���� ������ �� �������� ���������� ������� ���������, ������������� �������� �� ���������
						Console.WriteLine($"������: '{trustServerCertificateString}' �� �������� ���������� ������� ���������. ������������� �������� �� ��������� (true).");
						Log.Write($"������: '{trustServerCertificateString}' �� �������� ���������� ������� ���������. ������������� �������� �� ��������� (true).");
						chkTrustServerCertificate.Checked = true;
					}
				}
				else
				{
					// ���� ���� �� ����������, ������������� �������� �� ���������
					chkTrustServerCertificate.Checked = true;
				}
			}
			catch (Exception ex)
			{
				// ��������� ����������, ��������, ����������� ������
				Console.WriteLine($"������ ��� �������� ������� � ����������� �������: {ex.Message}");
				Log.Write($"������ ��� �������� ������� � ����������� �������: {ex.Message}");
				chkTrustServerCertificate.Checked = true; // ������������� �������� �� ��������� � ������ ������
			}

			// ��������� �� INI-����� ������, � ������� ����� �������������� �������
			if (iniFile.KeyExists("FILENAME", "Format_Export"))
			{
				string formatExportValue = iniFile.ReadKey("FILENAME", "Format_Export");
				if (int.TryParse(formatExportValue, out int selectedIndex))
				{
					cbxFormat.SelectedIndex = selectedIndex;
				}
				else
				{
					cbxFormat.SelectedIndex = 0; // ������������� �������� �� ���������, ���� ������� �� ������
				}
			}
			else
			{
				cbxFormat.SelectedIndex = 0; // ������������� �������� �� ���������, ���� ���� �� ����������
			}
			Log.Write("�������� ���������� �� ini-����� ���������");
		}


		/// <summary>
		/// ��������� ������� �������� ����� � ������
		/// </summary>
		private void frmMain_Load(object sender, EventArgs e)
		{
			rtbLog.Text = Log.Read();
			Log.Separate();
			Log.Write("������ ����������");

			ReadIniFile();
			LoadDepartments();

			// �������� ���� ��-���������, ���� �� �������� � ������������
			string inputText = txtConfigNameId.Text;
			if (string.IsNullOrWhiteSpace(inputText))
			{
				Console.WriteLine("���� �� ������ ���� ������.");
			}
			else
			{
				if (int.TryParse(inputText, out int configNameId))
				{
					// ���� �������������� ������ �������, �������� �����
					SelectNodeByTag(configNameId);
				}
				else
				{
					Console.WriteLine("������� ���������� ����� �����.");
				}
			}

			// ��������� ������� � ����� ������
			if (_silent)
			{
				this.Visible = false;
				this.ShowInTaskbar = false;
				this.WindowState = FormWindowState.Minimized;
				cmdExport_Click(sender, e);
			}
			else {
				// ���� ����� ����� ����������, ������ ��������� � ������ ��� � ����������� ����
				this.Text = $"{txtConfigName.Text} (������������: {Program._department})";
				lblVersion.Text = $"������: {Application.ProductVersion}";
				TextBoxRead();
			}

		}

		/// <summary>
		/// ������� ����� ��� ���������� �������� ������ ��� ��� ���������
		/// </summary>
		/// <param name="insert">���� true, ����� ����������� � ������ ���������� ��������, ����� � ������ ��������������</param>
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
		/// ������� ����� ��� ���������� ������ � �������� ��� �� ���������
		/// </summary>
		/// <param name="insert">���� true, ����� ����������� � ������ ���������� ��������, ����� � ������ ��������������</param>
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
		/// �������� ������ �������������
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
		/// ������� ������� ��� �������� ��������. ��������� ������� ����������� �������� ������
		/// </summary>
		private void LoadSensors()
		{
			if (treeView.SelectedNode != null)
			{
				LoadSensorForIdXML(Program._department, (int)treeView.SelectedNode.Tag);
			}
		}

		/// <summary>
		/// ������� ������� ��� �������� ��������. ��������� ������� ����������� �������� ������
		/// </summary>
		/// <param name="xmlFilePath">������ �� ���������������� ���� XML</param>
		/// <param name="nodeId">������������� �������������</param>
		private void LoadSensorForIdXML(string xmlFilePath, int nodeId)
		{

			if (File.Exists(xmlFilePath))
			{
				XmlDocument doc = new();
				doc.Load(xmlFilePath); // ���� � ������ XML �����
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
						// ��������� ������, ����� ���� �� ������� (���� ����������)
						Console.WriteLine("��� ����� Sensor ��� ���������.");
						Log.Write("��� ����� Sensor ��� ���������.");
					}
				}
			}
		}

		/// <summary>
		/// ����� �������� ������ �� ��������������
		/// </summary>
		/// <param name="id">��� ��������������</param>
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
				// ���� ���� ����� �������� ����, ����������� ����� ����������
				SelectNodeByTagRecursive(node, id);
			}
		}

		/// <summary>
		/// ����������� ����� ��� ������ � �������� �����
		/// </summary>
		/// <param name="parentNode">������ �� �������� � ��������� ������</param>
		/// <param name="id">��� ��������������</param>
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
		/// ��������� ������� ������� �� ������ "�������"
		/// </summary>
		private void cmdClose_Click(object sender, EventArgs e)
		{
			Log.Write("���������� ������ ����������");
			Log.Separate();
			Application.Exit();
		}

		/// <summary>
		/// ��������� ������� �������� �����
		/// </summary>
		private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
		{
			SaveIniFile();
		}

		/// <summary>
		/// ��������� ������� ������� �� ������ "cmdSQLFileName"
		/// </summary>
		private void cmdSQLFileName_Click(object sender, EventArgs e)
		{
			ofdSQLFileName.Title = "���������� �������� ���� � �������� SQL";
			ofdSQLFileName.InitialDirectory = @"C:\";
			ofdSQLFileName.Filter = "����� SQL|*.sql";
			ofdSQLFileName.InitialDirectory = Application.StartupPath;
			if (!string.IsNullOrEmpty(txtSQLFileName.Text))
				ofdSQLFileName.FileName = txtSQLFileName.Text;
			if (ofdSQLFileName.ShowDialog() == DialogResult.OK)
				Log.Write(string.Concat("������ ���� SQL-�������: ", ofdSQLFileName.FileName));
			txtSQLFileName.Text = ofdSQLFileName.FileName;
		}

		/// <summary>
		/// ��������� ������� ������� �� ������ "cmdExport"
		/// </summary>
		private async void cmdExport_Click(object sender, EventArgs e)
		{
			try
			{
				if (cbxFormat.SelectedIndex == 0)
				{
					Log.Write("��������� � ���� ������� Microsoft Excel");
					//TODO: �������� ������������������ �������. ����� ������������
					await ExportToExcelAsync1();
				}
				else
				{
					//TODO: �������� ������������������ �������. ����� ������������
					Log.Write("��������� � ���� ������� CSV");
					await ExportToCSVAsync1();
				}
			}
			catch (Exception ex)
			{
				Log.Write($"������ ��� ��������: {ex.Message}");
				// �������������� ��������� ������, ���� ����������
			}
		}

		/// <summary>
		/// ���������� ������ �� DataTable � CSV-����.
		/// </summary>
		/// <param name="dataTable">������ �� ����� ������.</param>
		/// <param name="filePath">���� �� ����� � �������������.</param>
		public static void WriteDataTableToCsv(DataTable dataTable, string filePath)
		{
			try
			{
				using (StreamWriter writer = new(filePath))
				{
					// ������ ���������� ��������
					for (int i = 0; i < dataTable.Columns.Count; i++)
					{
						writer.Write(dataTable.Columns[i]);
						if (i < dataTable.Columns.Count - 1)
						{
							writer.Write(","); // �����������
						}
					}
					writer.WriteLine(); // ������� �� ����� ������

					// ������ ������ �����
					foreach (DataRow row in dataTable.Rows)
					{
						for (int i = 0; i < dataTable.Columns.Count; i++)
						{
							writer.Write(row[i].ToString());
							if (i < dataTable.Columns.Count - 1)
							{
								writer.Write(","); // �����������
							}
						}
						writer.WriteLine(); // ������� �� ����� ������
					}
				}

				Console.WriteLine($"������ ������� �������� � ����: {filePath}");
				Log.Write($"������ ������� �������� � ����: {filePath}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"��������� ������ ��� ������ � CSV: {ex.Message}");
				Log.Write($"��������� ������ ��� ������ � CSV: {ex.Message}");
			}
		}

		/// <summary>
		/// ����������� ������� ��� �������� ������ � CSV-����.
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
				MessageBox.Show("���� �� ������: " + fnfEx.Message);
			}
			catch (Exception ex)
			{
				MessageBox.Show("��������� ������: " + ex.Message);
			}
		}

		/// <summary>
		/// ����������� ������� ��� �������� ������ � Excel-����.
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
				MessageBox.Show("���� �� ������: " + fnfEx.Message);
			}
			catch (Exception ex)
			{
				MessageBox.Show("��������� ������: " + ex.Message);
			}
		}

		/// <summary>
		/// ������ SQL-������ �� ���������� �����.
		/// </summary>
		/// <param name="filePath">���� � �����, ����������� SQL-������.</param>
		/// <returns>������, ���������� SQL-������.</returns>
		private async Task<string> ReadSqlFromFileAsync(string filePath)
		{
			using StreamReader reader = new(filePath);
			return await reader.ReadToEndAsync();
		}

		/// <summary>
		/// ��������� ������� ������ �� ���� ������ � ��������� ������ (CSV ��� Excel).
		/// </summary>
		/// <param name="sDataSource">�������� ������ ��� ����������� � ���� ������.</param>
		/// <param name="sUserID">��� ������������ ��� ����������� � ���� ������.</param>
		/// <param name="sPassword">������ ��� ����������� � ���� ������.</param>
		/// <param name="sInitialCatalog">��� ���� ������.</param>
		/// <param name="bIntegratedSecurity">����, �����������, ������������ �� ��������������� ������������.</param>
		/// <param name="bTrustServerCertificate">����, �����������, �������� �� ����������� �������.</param>
		/// <param name="strSQL">SQL-������ ��� ����������.</param>
		/// <param name="exportType">��� �������� (CSV ��� Excel).</param>
		/// <returns>������, �������������� ����������� ��������.</returns>
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

				Log.Write("�������� ������� ���������.");
				if (!_silent)
				{
					MessageBox.Show("�������� ������� ���������");
				}
			}
			catch (SqlException e)
			{
				Log.Write($"�������� ������ �� ����� ���������� sql-�������: {e}");
				MessageBox.Show(e.ToString());
			}
			finally
			{
				cmdExport.Enabled = true;
			}
		}

		/// <summary>
		/// �������� ������ ����������� � ���� ������.
		/// </summary>
		/// <param name="sDataSource">�������� ������ ��� ����������� � ���� ������.</param>
		/// <param name="sUserID">��� ������������ ��� ����������� � ���� ������.</param>
		/// <param name="sInitialCatalog">��� ���� ������.</param>
		/// <param name="strSQL">SQL-������ ��� ����������.</param>
		private void LogConnectionDetails(string sDataSource, string sUserID, string sInitialCatalog, string strSQL)
		{
			Log.Write($"�������� ������ (DataSource): {sDataSource}");
			Log.Write($"��� ������������ (UserID): {sUserID}");
			Log.Write($"������ (Password): *********");
			Log.Write($"���� ������ (InitialCatalog): {sInitialCatalog}");
			Log.Write($"SQL-������ � ���� ������: {strSQL}");
		}

		/// <summary>
		/// ������ �������������� �������� �� XML-�����.
		/// </summary>
		/// <returns>������, ���������� �������������� ��������, ����������� ��������.</returns>
		private static string ReadSensorIds()
		{
			string xmlFilePath = Program._department;
			XmlDocument xmlDoc = new();
			xmlDoc.Load(xmlFilePath);
			XmlNodeList? sensorNodes = xmlDoc.SelectNodes("//Sensor");

			// �������� �� null � ������� ������ ������, ���� ��� ��������
			if (sensorNodes == null || sensorNodes.Count == 0)
			{
				return string.Empty; // ��� ����� ��������� ����������, ���� ��� ��������
			}

			return string.Join(",", sensorNodes.Cast<XmlNode>().Select(node => node.Attributes["Id"].Value));
		}

		/// <summary>
		/// ��������� SQL-������ � ���������� ���������� � ���� DataTable.
		/// </summary>
		/// <param name="connection">���������� � ����� ������.</param>
		/// <param name="strSQL">SQL-������ ��� ����������.</param>
		/// <param name="selectedIds">�������������� �������� ��� ���������� ������.</param>
		/// <param name="startDate">���� ������ ��� ���������� ������.</param>
		/// <param name="endDate">���� ��������� ��� ���������� ������.</param>
		/// <returns>DataTable, ���������� ���������� ���������� SQL-�������.</returns>
		private async Task<DataTable> ExecuteSqlQueryAsync(SqlConnection connection, string strSQL, string selectedIds, DateTime startDate, DateTime endDate)
		{
			DataTable dataTable = new();
			using SqlCommand command = new(strSQL, connection)
			{
				CommandTimeout = Int32.MaxValue // ���������� �������� � ��������
			};

			command.Parameters.AddWithValue("@selected_ids", selectedIds);
			command.Parameters.AddWithValue("@start_date", startDate);
			command.Parameters.AddWithValue("@end_date", endDate);

			using SqlDataAdapter adapter = new(command);
			await Task.Run(() => adapter.Fill(dataTable));
			return dataTable;
		}

		/// <summary>
		/// ���������� ���� � ����� ��� �������� ������.
		/// </summary>
		/// <param name="fileNameWithoutExtension">��� ����� ��� ����������.</param>
		/// <param name="fileExtension">���������� �����.</param>
		/// <returns>������, �������������� ������ ���� � �����.</returns>
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
		/// ���������� ������ �� DataTable � Excel-����.
		/// </summary>
		/// <param name="dataTable">������ �� ����� ������.</param>
		/// <param name="filePath">���� �� Excel-�����.</param>
		/// <returns>������, �������������� ����������� ��������.</returns>
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

			// ��������� ������ ���� �������
			for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
			{
				worksheet.Column(col).Width = 11; // ���������� ������ ������� �� 11
			}
			worksheet.Column(1).Width = 20;
			worksheet.Cells["A1"].Value = "�����";
			worksheet.Cells["B1"].Value = "����";
			worksheet.Cells["C1"].Value = "�����";
			worksheet.Cells["D1"].Value = "���";
			worksheet.Cells["E1"].Value = "���";
			worksheet.Cells["F1"].Value = "���";

			// ���������� Excel �����
			FileInfo excelFile = new(filePath);
			await excelPackage.SaveAsAsync(excelFile);

			worksheet.InsertRow(2, 1); // ��������� ���� ������ ������ ����� ������ ������
			worksheet.InsertRow(3, 1); // ��������� ���� ������ ������ ����� ������ ������

			// ������ XML ����� ��� ��������� ����� sensor � �� ��������

			string xmlFilePath = Program._department;
			XmlDocument xmlDoc = new();
			xmlDoc.Load(xmlFilePath);
			XmlNodeList? sensorNodes = xmlDoc.SelectNodes("//Sensor");

			// �������� �� null � ������� ������ ������, ���� ��� ��������
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
				// �������������� �������� �� ������ ����� sensor � �� ��������
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

			// ���������� ������ ������ � ��������� �������
			for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
			{
				worksheet.Cells[2, col].Value = col - 1; // ������������� �������� �������� �������
			}
			worksheet.Cells["A2"].Style.Numberformat.Format = "@";

			// ������� ������ � ������ A3 � ���������� �����
			worksheet.Cells["A3"].Value = "�����\n������� ���������:\n1-�������,\n2-��������,\n3-������� �������\n4 - ������� �����������";
			worksheet.Cells["A3"].Style.WrapText = true; // �������� ������� ������
			worksheet.Cells["A3"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center; // ����������� ����� �� ������

			// ���������� ������� ������� � �������
			worksheet.Cells["B3"].Value = 3;
			worksheet.Cells["C3"].Value = 3;
			worksheet.Cells["D3"].Value = 3;
			worksheet.Cells["E3"].Value = 3;
			worksheet.Cells["F3"].Value = 3;

			// ���������� ������ ����� ����� ������� �������
			worksheet.Column(2).Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
			// ���������� ������ ����� ����� ������ ������
			worksheet.Row(2).Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

			// ���������� ��������� � Excel ����
			await excelPackage.SaveAsAsync(excelFile);
		}

		/// <summary>
		/// ����������� ������� ���������� ������ �� ���� ������ � CSV-����
		/// </summary>
		private async Task ExportToCSVAsync1()
		{
			try
			{
				using StreamReader reader = new(txtSQLFileName.Text);
				// ������ ������ �� �����
				string strSQL = reader.ReadToEnd();

				await Main(txtDataSource.Text, txtUserID.Text, txtSQLDBPass.Text, txtInitialCatalog.Text, chkIntegratedSecurity.Checked, chkTrustServerCertificate.Checked, strSQL);
				rtbLog.Text = Log.Read();
			}
			catch (FileNotFoundException fnfEx)
			{
				// ��������� ������, ����� ���� �� ������
				MessageBox.Show("���� �� ������: " + fnfEx.Message);
				// �������������� ��������, ��������, ����������� ������������
			}
			catch (Exception ex)
			{
				// ��������� ������ ����������
				MessageBox.Show("��������� ������: " + ex.Message);
				// �������������� ��������, ��������, ����������� ������
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
				Log.Write(string.Concat("�������� ������ (DataSource): ", sDataSource));
				Log.Write(string.Concat("��� ������������ (UserID): ", sUserID));
				Log.Write(string.Concat("������ (Password): ", "*********"));
				Log.Write(string.Concat("���� ������ (InitialCatalog): ", sInitialCatalog));
				Log.Write(string.Concat("SQL-������ � ���� ������: ", strSQL));

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
					// ������������ ���������������� ���� � ���������, ���� ������� �������
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
						CommandTimeout = Int32.MaxValue // ���������� �������� � ��������
					};
					command.Parameters.AddWithValue("@selected_ids", selectedIds);
					command.Parameters.AddWithValue("@start_date", Convert.ToDateTime(iniFile.ReadKey("INTERVAL", "Start_data")));
					command.Parameters.AddWithValue("@end_date", Convert.ToDateTime(iniFile.ReadKey("INTERVAL", "Final_data")));

					SqlDataAdapter adapter = new(command);
					DataTable dataTable = new();

					Log.Write("�������� ���������� �������. ��������, ������ ����� ����������� ����� ��������������� �����.");
					TextBoxRead();
					int i = tabMain.SelectedIndex;
					tabMain.SelectedIndex = 4;
					// ������������� �� ������� RowsUpdated
					adapter.RowUpdated += Adapter_RowUpdated;

					//Stopwatch stopwatch = new();
					stopwatch.Reset();
					stopwatch.Start();

					timer = new System.Windows.Forms.Timer
					{
						Interval = 5000 // 5 ������
					};
					timer.Tick += Timer_Tick;
					timer.Start();

					// ��������� DataTable
					try
					{
						await Task.Run(() => adapter.Fill(dataTable));
					}
					catch (SqlException sqlEx)
					{
						// ��������� ������ SQL
						Console.WriteLine("��������� ������ SQL: " + sqlEx.Message);
						// �������������� ��������, ��������, ����������� ������
						cmdExport.Enabled = true;
					}
					catch (Exception ex)
					{
						// ��������� ������ ����������
						Console.WriteLine("��������� ������: " + ex.Message);
						// �������������� ��������, ��������, ����������� ������
						cmdExport.Enabled = true;
					}

					stopwatch.Stop();

					timer.Stop();
					TimeSpan elapsedTime = stopwatch.Elapsed;

					Log.Write($"����� ���������� �������: {elapsedTime.TotalSeconds} ������");
					TextBoxRead();

					// ������������ �� ������� RowsUpdated
					adapter.RowUpdated -= Adapter_RowUpdated;

					tabMain.SelectedIndex = i;

					// Save Excel package to a file
					string filePath = iniFile.ReadKey("FILENAME", "PathToResultFolder");
					// �������� ������� ���� � �����
					string currentDateTime = DateTime.Now.ToString("yyyyMMdd_HH_mm_ss");

					// ��������� ��� ����� � ����������
					string fileNameWithoutExtension = "output";
					string fileExtension = ".csv";

					if (chkResultsFileNameAddDate.Checked)
					{
						// ��������� ����� ��� �����
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
						// ��������� ����� ��� �����
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

					// ������ ������ �� CSV
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

					// ��������� ���������� ��������
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
									// ����� ����� �������� ������ ��� ������ �������� � ������ ������
									// ��������, � ������ 3 (������ 2)
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

					// ������ ���������� ������ ������� � CSV
					using (StreamWriter writer = new(filePath))
					{
						foreach (var row in csvData)
						{
							writer.WriteLine(string.Join(",", row));
						}
					}

					Log.Write("�������� ������� ���������.");
					TextBoxRead();
					if (!_silent)
					{
						MessageBox.Show("�������� ������� ���������");
					}
					cmdExport.Enabled = true;
					if (_silent)
					{
						Application.Exit();
					}
				}
				catch (SqlException e)
				{
					Log.Write(string.Concat("�������� ������ �� ����� ���������� sql-�������: ", e.ToString()));
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
				// ������ ������ �� �����
				string strSQL = reader.ReadToEnd();

				await Main(txtDataSource.Text, txtUserID.Text, txtSQLDBPass.Text, txtInitialCatalog.Text, chkIntegratedSecurity.Checked, chkTrustServerCertificate.Checked, strSQL);
				rtbLog.Text = Log.Read();
			}
			catch (FileNotFoundException fnfEx)
			{
				// ��������� ������, ����� ���� �� ������
				MessageBox.Show("���� �� ������: " + fnfEx.Message);
				// �������������� ��������, ��������, ����������� ������������
			}
			catch (Exception ex)
			{
				// ��������� ������ ����������
				MessageBox.Show("��������� ������: " + ex.Message);
				// �������������� ��������, ��������, ����������� ������
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
				Log.Write(string.Concat("�������� ������ (DataSource): ", sDataSource));
				Log.Write(string.Concat("��� ������������ (UserID): ", sUserID));
				Log.Write(string.Concat("������ (Password): ", "*********"));
				Log.Write(string.Concat("���� ������ (InitialCatalog): ", sInitialCatalog));
				Log.Write(string.Concat("SQL-������ � ���� ������: ", strSQL));
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
					// ������������ ���������������� ���� � ���������, ���� ������� �������
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
						CommandTimeout = Int32.MaxValue // ���������� �������� � ��������
					};
					command.Parameters.AddWithValue("@selected_ids", selectedIds);
					command.Parameters.AddWithValue("@start_date", Convert.ToDateTime(iniFile.ReadKey("INTERVAL", "Start_data")));
					command.Parameters.AddWithValue("@end_date", Convert.ToDateTime(iniFile.ReadKey("INTERVAL", "Final_data")));

					SqlDataAdapter adapter = new(command);
					DataTable dataTable = new();

					Log.Write("�������� ���������� �������. ��������, ������ ����� ����������� ����� ��������������� �����.");
					TextBoxRead();
					int i = tabMain.SelectedIndex;
					tabMain.SelectedIndex = 4;
					// ������������� �� ������� RowsUpdated
					adapter.RowUpdated += Adapter_RowUpdated;

					//Stopwatch stopwatch = new();
					stopwatch.Reset();
					stopwatch.Start();

					timer = new System.Windows.Forms.Timer
					{
						Interval = 5000 // 5 ������
					};
					timer.Tick += Timer_Tick;
					timer.Start();

					// ��������� DataTable
					try
					{
						await Task.Run(() => adapter.Fill(dataTable));
					}
					catch (SqlException sqlEx)
					{
						// ��������� ������ SQL
						Console.WriteLine("��������� ������ SQL: " + sqlEx.Message);
						// �������������� ��������, ��������, ����������� ������
						cmdExport.Enabled = true;
					}
					catch (Exception ex)
					{
						// ��������� ������ ����������
						Console.WriteLine("��������� ������: " + ex.Message);
						// �������������� ��������, ��������, ����������� ������
						cmdExport.Enabled = true;
					}

					stopwatch.Stop();

					timer.Stop();
					TimeSpan elapsedTime = stopwatch.Elapsed;

					Log.Write($"����� ���������� �������: {elapsedTime.TotalSeconds} ������");
					TextBoxRead();

					// ������������ �� ������� RowsUpdated
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
						// ��������� ������������ ������ �� ������ � ��������� �������� ������
						worksheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
						worksheet.Row(1).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
						worksheet.Row(1).Style.WrapText = true;
						// ��������� ������ ���� �������
						for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
						{
							worksheet.Column(col).Width = 11; // ���������� ������ ������� �� 20
						}
						worksheet.Column(1).Width = 20;
						worksheet.Cells["A1"].Value = "�����";
						worksheet.Cells["B1"].Value = "����";
						worksheet.Cells["C1"].Value = "�����";
						worksheet.Cells["D1"].Value = "���";
						worksheet.Cells["E1"].Value = "���";
						worksheet.Cells["F1"].Value = "���";

						// Save Excel package to a file
						string filePath = iniFile.ReadKey("FILENAME", "PathToResultFolder");
						// �������� ������� ���� � �����
						string currentDateTime = DateTime.Now.ToString("yyyyMMdd_HH_mm_ss");

						// ��������� ��� ����� � ����������
						string fileNameWithoutExtension = "output";
						string fileExtension = ".xlsx";

						if (chkResultsFileNameAddDate.Checked)
						{
							// ��������� ����� ��� �����
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
							// ��������� ����� ��� �����
							if (!string.IsNullOrEmpty(_config))
							{
								filePath = $"{filePath}\\{fileNameWithoutExtension}_{_config}{fileExtension}";
							}
							else
							{
								filePath = $"{filePath}\\{fileNameWithoutExtension}{fileExtension}";
							}
						}

						Log.Write($"���� ����������� � : {filePath}");
						FileInfo excelFile = new(filePath);
						excelPackage.SaveAs(excelFile);

						worksheet.InsertRow(2, 1); // ��������� ���� ������ ������ ����� ������ ������
						worksheet.InsertRow(3, 1); // ��������� ���� ������ ������ ����� ������ ������

						// ������ XML ����� ��� ��������� ����� sensor � �� ��������

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

						// �������������� �������� �� ������ ����� sensor � �� ��������
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

						// ���������� ������ ������ � ��������� �������
						for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
						{
							worksheet.Cells[2, col].Value = col - 1; // ������������� �������� �������� �������
						}
						worksheet.Cells["A2"].Style.Numberformat.Format = "@";

						// ������� ������ � ������ A3 � ���������� �����
						worksheet.Cells["A3"].Value = "�����\n������� ���������:\n1-�������,\n2-��������,\n3-������� �������\n4 - ������� �����������";
						worksheet.Cells["A3"].Style.WrapText = true; // �������� ������� ������
						worksheet.Cells["A3"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center; // ����������� ����� �� ������

						// ���������� ������� ������� � �������
						worksheet.Cells["B3"].Value = 3;
						worksheet.Cells["C3"].Value = 3;
						worksheet.Cells["D3"].Value = 3;
						worksheet.Cells["E3"].Value = 3;
						worksheet.Cells["F3"].Value = 3;

						// ���������� ������ ����� ����� ������� �������
						worksheet.Column(2).Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
						// ���������� ������ ����� ����� ������ ������
						worksheet.Row(2).Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

						// ���������� ��������� � Excel ����
						excelPackage.Save();
					}

					Log.Write("�������� ������� ���������.");
					TextBoxRead();
					if (!_silent)
					{
						MessageBox.Show("�������� ������� ���������");
					}
					cmdExport.Enabled = true;
					if (_silent)
					{
						Application.Exit();
					}
				}
				catch (SqlException e)
				{
					Log.Write(string.Concat("�������� ������ �� ����� ���������� sql-�������: ", e.ToString()));
					MessageBox.Show(e.ToString());
					cmdExport.Enabled = true;
				}
			}
		}

		private void Timer_Tick(object? sender, EventArgs e)
		{
			TimeSpan elapsedTime = stopwatch.Elapsed;
			UpdateTimeLabel(elapsedTime.TotalSeconds.ToString("F2")); // ��������� ����� �� �����
		}

		private void UpdateTimeLabel(string time)
		{
			// ��������� ����� �� �����
			Log.Write($"����� ����������: {time} ������");
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
			// �������� ���������� ����������� �����
			int updatedRows = e.RecordsAffected;

			Log.Write($"���������� {updatedRows} �����.");
			rtbLog.Text = Log.Read();
			rtbLog.SelectionStart = rtbLog.Text.Length;
			rtbLog.ScrollToCaret();

			// ������� ���������� � ���������
			Console.WriteLine($"���������� {updatedRows} �����.");
		}


		private void chkIntegratedSecurity_CheckedChanged(object sender, EventArgs e)
		{
			IntegratedSecurity();
		}

		private void IntegratedSecurity()
		{
			// ��������� ���� ������ � ������, ���� �������� �������� �����������
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
		/// ������� ������� ������ �������� � ������� �� xml �� Id
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
		/// ���������� �������. ���� ��� �������� ������������ ���� ���������� ��������� ������� ������.
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
			// ������ ID ��� ��������
			List<string> sensorIds = listView.SelectedItems.Cast<ListViewItem>().Select(item => item.Text).ToList();

			// �������� �� ListView
			foreach (ListViewItem item in listView.SelectedItems)
			{
				listView.Items.Remove(item);
			}

			// �������� �� XML
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
		/// ������� ����� ��� ���������� ������ ��� ��������� ��������
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

			fbdResult.Description = "�������� �����";
			fbdResult.ShowNewFolderButton = true; // ��������� ��������� ����� �����

			// �������� ������ � ��������� ���������
			if (fbdResult.ShowDialog() == DialogResult.OK)
			{
				string selectedPath = fbdResult.SelectedPath;
				txtResultsFileName.Text = selectedPath;
				Console.WriteLine($"������� �����: {selectedPath}");
			}
			else
			{
				Console.WriteLine("����� ����� �������.");
			}
		}

		private void cmdTasks_Click(object sender, EventArgs e)
		{

			WindowsIdentity identity = WindowsIdentity.GetCurrent();
			WindowsPrincipal principal = new(identity);
			if (principal.IsInRole(WindowsBuiltInRole.Administrator))
			{
				// ��������� ���, ��������� ���� ��������������
				// ���� � ������������ �����, ������� ����� ���������

				string exePath;

				if (!string.IsNullOrEmpty(_config))
				{
					exePath = System.Windows.Forms.Application.ExecutablePath + " /s" + $" /c {_config}";
				}
				else
				{
					exePath = System.Windows.Forms.Application.ExecutablePath + " /s";
				}

				// �������� ��������� ���� ������
				int dayOfWeek = (int)nudExecPeriod.Value;

				string arguments = $"/CREATE /TN \"SCADAExport\" /RU \"SYSTEM\" /TR \"{exePath}\" /SC WEEKLY /D {GetDayOfWeekString(dayOfWeek)} /ST {dtpExecTime.Value:HH:mm}";

				DeleteTasks("SCADAExport");

				// ������� ������ ProcessStartInfo
				ProcessStartInfo startInfo = new()
				{
					FileName = "schtasks.exe",
					Arguments = arguments,
					UseShellExecute = false,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					Verb = "runas"
				};

				// ��������� �������
				using Process? process = Process.Start(startInfo);
				// ������ ����� ��������
				string output = process.StandardOutput.ReadToEnd();
				string error = process.StandardError.ReadToEnd();

				// ��������� ���������
				if (process.ExitCode == 0)
				{
					Console.WriteLine("������� ������� ������� � ������������.");
				}
				else
				{
					Console.WriteLine($"������ ��� �������� �������: {error}");
				}
			}
			else
			{
				// ������������ �� �������� ���������������
				MessageBox.Show("��������� ���������� � ������� ��������������.");
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
					Console.WriteLine($"������ '{taskName}' ������� �������.");
				}
				else
				{
					Console.WriteLine($"��������� ������� ������ '{taskName}'. ��� ������: {exitCode}");
				}
			}
			else
			{
				Console.WriteLine("�� ������� ��������� �������.");
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