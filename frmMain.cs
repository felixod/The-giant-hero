
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using OfficeOpenXml;
using SQLBuilder.ini;
using System.Data;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using OfficeOpenXml.Utils;
using System.Security.Permissions;
using System.Security.Principal;

namespace SQLBuilder
{
	public partial class frmMain : Form
	{
		private bool _silent;
		private Stopwatch stopwatch = new();
		private System.Windows.Forms.Timer timer;

		public frmMain(bool silent)
		{
			_silent = silent;
			InitializeComponent();

		}
		private void frmMain_Load(object sender, EventArgs e)
		{
			lblVersion.Text = $"������: {Application.ProductVersion}";
			rtbLog.Text = Log.Read();
			Log.Separate();
			Log.Write("������ ����������");
			IniFile iniFile = new("config.ini");
			// ������ SCHEDULE
			// ��������� �� INI-����� ��������� ����
			if (iniFile.KeyExists("SCHEDULE", "Start_data"))
				dtpStartExecution.Value = Convert.ToDateTime(iniFile.ReadKey("SCHEDULE", "Start_data"));
			else
				dtpStartExecution.Value = DateTime.Now;
			// ��������� �� INI-����� ����� ����������
			if (iniFile.KeyExists("SCHEDULE", "Execution_time"))
				dtpExecTime.Value = DateTime.Now.Date.Add(Convert.ToDateTime(iniFile.ReadKey("SCHEDULE", "Execution_time")).TimeOfDay);
			else
				dtpExecTime.Value = DateTime.Now;
			// ��������� �� INI-����� ������ ����������
			if (iniFile.KeyExists("SCHEDULE", "Execution_period"))
				nudExecPeriod.Value = int.Parse(iniFile.ReadKey("SCHEDULE", "Execution_period"));
			else
				nudExecPeriod.Value = 1;
			// ������ INTERVAL
			// ��������� �� INI-����� ��������� ���� �������
			if (iniFile.KeyExists("INTERVAL", "Start_data"))
				dtpStartData.Value = Convert.ToDateTime(iniFile.ReadKey("INTERVAL", "Start_data"));
			else
				dtpStartData.Value = DateTime.Now;
			// ��������� �� INI-����� �������� ���� �������
			if (iniFile.KeyExists("INTERVAL", "Final_data"))
				dtpFinalData.Value = Convert.ToDateTime(iniFile.ReadKey("INTERVAL", "Final_data"));
			else
				dtpFinalData.Value = DateTime.Now;
			// ������ FILENAME

			// ��������� �� INI-����� ������������ � ��� ������������
			if (iniFile.KeyExists("FILENAME", "Config_Name"))
			{
				txtConfigName.Text = iniFile.ReadKey("FILENAME", "Config_Name");
				this.Text = $"SQLBuilder ({txtConfigName.Text})";
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
			// ��������� �� INI-����� ��� ����� � ����������� ������
			if (iniFile.KeyExists("FILENAME", "XLSX_File_Name"))
				txtResultsFileName.Text = iniFile.ReadKey("FILENAME", "XLSX_File_Name");
			else
				txtResultsFileName.Text = "";
			// ��������� �� INI-����� �������� ��������� �� � ����� ����� ���������� ����
			if (iniFile.KeyExists("FILENAME", "XLSX_File_Name_Date"))
				chkResultsFileNameAddDate.Checked = Convert.ToBoolean(iniFile.ReadKey("FILENAME", "XLSX_File_Name_Date"));
			else
				chkResultsFileNameAddDate.Checked = false;
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
			if (iniFile.KeyExists("FILENAME", "SQL_DB_IntegratedSecurity"))
				chkIntegratedSecurity.Checked = Convert.ToBoolean(iniFile.ReadKey("FILENAME", "SQL_DB_IntegratedSecurity"));
			else
				chkIntegratedSecurity.Checked = false;
			IntegratedSecurity();
			// ��������� �� INI-����� ������� � ����������� �������
			if (iniFile.KeyExists("FILENAME", "SQL_DB_TrustServerCertificate"))
				chkTrustServerCertificate.Checked = Convert.ToBoolean(iniFile.ReadKey("FILENAME", "SQL_DB_TrustServerCertificate"));
			else
				chkTrustServerCertificate.Checked = true;

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

			Log.Write("�������� ���������� �� ini-����� ���������");
			TextBoxRead();
			if (_silent)
			{
				this.Visible = false;
				this.ShowInTaskbar = false;
				this.WindowState = FormWindowState.Minimized;
				cmdExport_Click(sender, e);
			}
		}


		/// <summary>
		/// ������� ����� ��� ���������� ������ ��� ���������
		/// </summary>
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
		/// ������� ����� ��� ���������� ������ ��� ��������� ��������
		/// </summary>
		private void OpenSensorForm(bool insert)
		{
			if (treeView.SelectedNode != null)
			{
				frmSensor form = new(insert, (int)treeView.SelectedNode.Tag, treeView.SelectedNode.Text);
				form.ShowDialog();
			}
			LoadSensors();
		}


		/// <summary>
		/// ����������� �������. �������� ������ �������
		/// </summary>
		private void LoadDepartments()
		{
			int id = 0;
			if (treeView.SelectedNode != null)
			{
				id = (int)treeView.SelectedNode.Tag;
			}

			treeView.Nodes.Clear();
			TreeViewXmlLoader.LoadTreeViewFromXml(treeView, "departments.xml");

			SelectNodeByTag(id);
		}


		/// <summary>
		/// �������� ��������
		/// </summary>
		private void LoadSensors()
		{
			if (treeView.SelectedNode != null)
			{
				LoadSensorForIdXML("departments.xml", (int)treeView.SelectedNode.Tag);
			}
		}

		private void LoadSensorForIdXML(string xmlFilePath, int nodeId)
		{

			if (File.Exists(xmlFilePath))
			{
				XmlDocument doc = new();
				doc.Load(xmlFilePath); // ���� � ������ XML �����
				listView.Items.Clear();

				XmlNode node = doc.SelectSingleNode($"//Node[@Id='{nodeId}']");
				if (node != null)
				{
					foreach (XmlNode sensorNode in node.SelectNodes("Sensor"))
					{
						var param = new ListViewItem(new[] { sensorNode.Attributes["Id"].Value,
															 sensorNode.Attributes["Name"].Value,
															 sensorNode.Attributes["Type"].Value,
															 sensorNode.Attributes["Description"].Value })
						{
							Tag = nodeId
						};
						listView.Items.Add(param);
					}
				}
			}
		}

		// ����������� ����� ��� ������
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

		// ����������� ����� ��� ������ � �������� �����
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

		private void cmdClose_Click(object sender, EventArgs e)
		{
			Log.Write("���������� ������ ����������");
			Log.Separate();
			Application.Exit();
		}

		private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
		{
			SaveIni();
		}

		private void SaveIni()
		{
			IniFile iniFile = new("config.ini");
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
			iniFile.WriteKey("FILENAME", "XLSX_File_Name", txtResultsFileName.Text);
			iniFile.WriteKey("FILENAME", "SQL_DB_DataSource", txtDataSource.Text);
			iniFile.WriteKey("FILENAME", "SQL_DB_UserID", txtUserID.Text);
			byte[] key = Enumerable.Range(0, 32).Select(x => (byte)x).ToArray();
			iniFile.WriteKey("FILENAME", "SQL_DB_Pass", Crypt.Encrypt(txtSQLDBPass.Text, key));
			iniFile.WriteKey("FILENAME", "SQL_DB_InitialCatalog", txtInitialCatalog.Text);
			iniFile.WriteKey("FILENAME", "SQL_DB_IntegratedSecurity", chkIntegratedSecurity.Checked.ToString());
			iniFile.WriteKey("FILENAME", "SQL_DB_TrustServerCertificate", chkTrustServerCertificate.Checked.ToString());
			iniFile.WriteKey("FILENAME", "XLSX_File_Name_Date", chkResultsFileNameAddDate.Checked.ToString());
			Log.Write("������ ���������� � ini-���� ���������");
		}

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

		private void cmdExport_Click(object sender, EventArgs e)
		{
			try
			{
				using StreamReader reader = new(txtSQLFileName.Text);
				// ������ ������ �� �����
				string strSQL = reader.ReadToEnd();

				Main(txtDataSource.Text, txtUserID.Text, txtSQLDBPass.Text, txtInitialCatalog.Text, chkIntegratedSecurity.Checked, chkTrustServerCertificate.Checked, strSQL);
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

					string xmlFilePath = "departments.xml";
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
					IniFile iniFile = new("config.ini");
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
					}
					catch (Exception ex)
					{
						// ��������� ������ ����������
						Console.WriteLine("��������� ������: " + ex.Message);
						// �������������� ��������, ��������, ����������� ������
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
						string filePath = iniFile.ReadKey("FILENAME", "XLSX_File_Name");

						if (chkResultsFileNameAddDate.Checked)
						{
							// �������� ������� ���� � �����
							string currentDateTime = DateTime.Now.ToString("yyyyMMdd_HH_mm_ss");

							// ��������� ��� ����� � ����������
							string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
							string fileExtension = Path.GetExtension(filePath);

							// ��������� ����� ��� �����
							filePath = $"{fileNameWithoutExtension}_{currentDateTime}{fileExtension}";
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
				}
			}
		}

		private void Timer_Tick(object sender, EventArgs e)
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
					RemoveNodeById("departments.xml", (int)treeView.SelectedNode.Tag);
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
			RemoveSensors(listView, "departments.xml");
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
			SaveIni();
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
			ofdResultsFileName.Title = "���������� �������� ���� Microsoft Excel";
			ofdResultsFileName.InitialDirectory = @"C:\";
			ofdResultsFileName.Filter = "����� Microsoft Excel|*.xlsx";
			ofdResultsFileName.InitialDirectory = Application.StartupPath;
			if (!string.IsNullOrEmpty(txtResultsFileName.Text))
				ofdResultsFileName.FileName = txtResultsFileName.Text;
			if (ofdResultsFileName.ShowDialog() == DialogResult.OK)
				Log.Write(string.Concat("������ ���� Microsoft Excel ��� �������� ����������� ������: ", ofdResultsFileName.FileName));
			txtResultsFileName.Text = ofdResultsFileName.FileName;
		}

		private void cmdTasks_Click(object sender, EventArgs e)
		{

			WindowsIdentity identity = WindowsIdentity.GetCurrent();
			WindowsPrincipal principal = new(identity);
			if (principal.IsInRole(WindowsBuiltInRole.Administrator))
			{
				// ��������� ���, ��������� ���� ��������������
				// ���� � ������������ �����, ������� ����� ���������
				string exePath = System.Windows.Forms.Application.ExecutablePath + " /s";

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
				using Process process = Process.Start(startInfo);
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

			using Process process = Process.Start(startInfo);
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

	public class TreeViewXmlLoader
	{
		// ����� ��� �������� TreeView �� XML
		public static void LoadTreeViewFromXml(System.Windows.Forms.TreeView treeView, string fileName)
		{
			XmlDocument xmlDoc = new();
			if (File.Exists(fileName))
			{
				xmlDoc.Load(fileName);
				TreeNode rootNode = CreateTreeNode(xmlDoc.DocumentElement);
				treeView.Nodes.Add(rootNode);
			}
			else
			{
				CreateEmptyXmlFile(fileName);
				LoadTreeViewFromXml(treeView, fileName);
			}
		}

		private static void CreateEmptyXmlFile(string filePath)
		{
			XmlDocument doc = new();
			// �������� ���������� XML
			XmlDeclaration? xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
			XmlElement? root = doc.DocumentElement;
			doc.InsertBefore(xmlDeclaration, root);

			// �������� ��������� ��������
			XmlElement rootElement = doc.CreateElement("Node");
			rootElement.SetAttribute("Name", "��������");
			rootElement.SetAttribute("Prefix", "");
			rootElement.SetAttribute("Id", "0");
			rootElement.SetAttribute("Code", "");

			doc.AppendChild(rootElement);

			// ���������� �����
			doc.Save(filePath);
		}

		// ����������� ����� ��� �������� TreeNode �� XML ����
		private static TreeNode CreateTreeNode(XmlNode xmlNode)
		{
			TreeNode treeNode = new();
			if (xmlNode.Attributes != null && xmlNode.Name == "Node")
			{
				XmlAttribute? nameAttribute = xmlNode.Attributes["Name"];
				if (nameAttribute != null)
				{
					treeNode.Name = nameAttribute.Value;
					treeNode.Text = nameAttribute.Value; // ���������� Name ��� �������� Text
				}

				XmlAttribute? codeAttribute = xmlNode.Attributes["Id"];
				if (codeAttribute != null)
				{
					treeNode.Tag = int.Parse(codeAttribute.Value); // ��������� Id � Tag
				}
			}

			foreach (XmlNode childNode in xmlNode.ChildNodes)
			{
				if (childNode.Name == "Node")
				{
					treeNode.Nodes.Add(CreateTreeNode(childNode)); // ���������� ��������� �������� ����
				}
			}
			return treeNode;
		}
	}