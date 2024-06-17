
using Microsoft.Data.SqlClient;
using SQLBuilder.ini;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System;
using System.Threading.Tasks;
using System.Diagnostics.Eventing.Reader;
using System.Data.Common;
using System.IO;
using System.Reflection.Metadata;
using System.Windows.Forms;
using System.Collections.Generic;
using static SQLBuilder.frmMain;
using static System.ComponentModel.Design.ObjectSelectorEditor;
using System.Xml.Linq;
using Microsoft.Identity.Client.NativeInterop;
using static Azure.Core.HttpHeader;

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
            // ��������� �� INI-����� ��� ����� � SQL-��������
            if (iniFile.KeyExists("FILENAME", "SQL_File_Name"))
                txtSQLFileName.Text = iniFile.ReadKey("FILENAME", "SQL_File_Name");
            else
                txtSQLFileName.Text = "";
            // ��������� �� INI-����� �������� ���� ������
            if (iniFile.KeyExists("FILENAME", "SQL_DB_InitialCatalog"))
                txtInitialCatalog.Text = iniFile.ReadKey("FILENAME", "SQL_DB_InitialCatalog");
            else
                txtInitialCatalog.Text = "";
            // ��������� �� INI-����� ������ ������������ ���� ������
            if (iniFile.KeyExists("FILENAME", "SQL_DB_Pass"))
                txtSQLDBPass.Text = Decrypt(iniFile.ReadKey("FILENAME", "SQL_DB_Pass"), Enumerable.Range(0, 32).Select(x => (byte)x).ToArray());
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
            Log.Write("�������� ���������� �� ini-����� ���������");
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
            iniFile.WriteKey("FILENAME", "SQL_File_Name", txtSQLFileName.Text);

            iniFile.WriteKey("FILENAME", "SQL_DB_DataSource", txtDataSource.Text);
            iniFile.WriteKey("FILENAME", "SQL_DB_UserID", txtUserID.Text);
            byte[] key = Enumerable.Range(0, 32).Select(x => (byte)x).ToArray();
            iniFile.WriteKey("FILENAME", "SQL_DB_Pass", Encrypt(txtSQLDBPass.Text, key));
            iniFile.WriteKey("FILENAME", "SQL_DB_InitialCatalog", txtInitialCatalog.Text);
            iniFile.WriteKey("FILENAME", "SQL_DB_IntegratedSecurity", chkIntegratedSecurity.Checked.ToString());
            iniFile.WriteKey("FILENAME", "SQL_DB_TrustServerCertificate", chkTrustServerCertificate.Checked.ToString());
            Log.Write("������ ���������� � ini-���� ���������");

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
            Log.Write("������ �����������");
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
            Log.Write("������ �����������");
            return Encoding.UTF8.GetString(output.ToArray());
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
                Log.Write(string.Concat("�������� ������ (DataSource): ", sDataSource));
                Log.Write(string.Concat("��� ������������ (UserID): ", sUserID));
                Log.Write(string.Concat("������ (Password): ", "*********"));
                Log.Write(string.Concat("���� ������ (InitialCatalog): ", sInitialCatalog));
                Log.Write(string.Concat("SQL-������ � ���� ������: ", strSQL));
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
                                Log.Write("�������� ������� ���������.");
                                MessageBox.Show("�������� ������� ���������");
                            }
                        }
                    }
                    catch (SqlException e)
                    {
                        Log.Write(string.Concat("�������� ������ �� ����� ���������� sql-�������: ", e.ToString()));
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
            // ��������� ���� ������ � ������, ���� �������� �������� �����������
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

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (treeView.SelectedNode.Tag != null)
            {
                RemoveNodeById("departments.xml", (int)treeView.SelectedNode.Tag);
                treeView.Nodes.Remove(treeView.SelectedNode);
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
            }
            else
            {
                insertToolStripMenuItem.Enabled = true;
                updateToolStripMenuItem.Enabled = false;
                deleteToolStripMenuItem.Enabled = false;
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
            } else 
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
            XmlElement?root = doc.DocumentElement;
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
            if (xmlNode.Attributes != null)
            {
                XmlAttribute ?nameAttribute = xmlNode.Attributes["Name"];
                if (nameAttribute != null)
                {
                    treeNode.Name = nameAttribute.Value;
                    treeNode.Text = nameAttribute.Value; // ���������� Name ��� �������� Text
                }

                XmlAttribute ?codeAttribute = xmlNode.Attributes["Id"];
                if (codeAttribute != null)
                {
                    treeNode.Tag = int.Parse(codeAttribute.Value); // ��������� Id � Tag
                }
            }

            foreach (XmlNode childNode in xmlNode.ChildNodes)
            {
                treeNode.Nodes.Add(CreateTreeNode(childNode)); // ���������� ��������� �������� ����
            }
            return treeNode;
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
                // ������ � ������� ������
                dataTable.Load(dataReader);
                // �������� ����� ��������
                columns = dataTable.Columns.Count;
                // ������� ���������
                if (includeHeaderAsFirstRow)
                {
                    for (int index = 0; index < columns; index++)
                    {
                        row += (dataTable.Columns[index]);
                        if (index < columns - 1)
                            row += separator;
                    }
                    // ��������� ����������� (����� ������)
                    row += Environment.NewLine;
                }
                // ��������� ������
                csvRows.Append(row);

                // ������� ���������� ������ �� ������ ������
                for (int rowIndex = 0; rowIndex < dataTable.Rows.Count; rowIndex++)
                {
                    row = ""; // ������� ���������� ��� ����� ������
                    for (int index = 0; index <= columns - 1; index++)
                    {
                        string value = dataTable.Rows[rowIndex][index].ToString();
                        // ���� ��� ������� - ������
                        if (dataTable.Rows[rowIndex][index] is string)
                        {
                            // ���� � �������� ������������ ������� �������, ������ ������ �� ��� �� ������� �������.
                            if (value.Contains('"', StringComparison.CurrentCulture))
                                value = value.Replace("\"", "\"\"");

                            // ���� � �������� ���� �����������, �������� ��� �������� ���������.
                            if (value.Contains(separator, StringComparison.CurrentCulture))
                                value = "\"" + value + "\"";

                            //���� ������ �������� ������� ����� ������, ������� ��
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
                        // ��������� �����������
                        if (index < columns - 1)
                            row += separator;
                    }
                    // ������� ����������� ����� ���������� �������
                    dataTable.Rows[rowIndex][columns - 1].ToString().ToString().Replace(separator, " ");
                    row += Environment.NewLine;
                    // ��������� ����� ������
                    csvRows.Append(row);
                }
            }
            catch (Exception)
            {
                throw;
            }
            Log.Write("�������� CSV-����� ���������.");
            return csvRows.ToString();
        }
    }
}