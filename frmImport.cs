using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.Common;

namespace SQLBuilder
{
	public partial class frmImport : Form
	{
		public frmImport()
		{
			InitializeComponent();
		}

		private void cmdImport_Click(object sender, EventArgs e)
		{
			txtLog.Text = Log.Read();
			using SqlConnection sourceConnection = new(txtConnectionString.Text);
			using SqlConnection destinationConnection = new(txtDestinationString.Text);
			Log.Separate();
			try { sourceConnection.Open(); } catch { Log.Write("Ошибка подключения к базе источника"); }
			try { destinationConnection.Open(); } catch { Log.Write("Ошибка подключения к базе назначения"); }
			Log.Write("Подключения к базе источника и базе назначения открыты");

			DataTable tables = sourceConnection.GetSchema("Tables");
			Log.Write("Получена схема данных источника");

			foreach (DataRow tableRow in tables.Rows)
			{
				string tableName = tableRow["TABLE_NAME"].ToString();

				// Read data from the source table
				try
				{
					string selectQuery = $"SELECT * FROM {tableName}";
					using SqlCommand selectCommand = new(selectQuery, sourceConnection);
					Log.Write($"Выполняем запрос: {selectQuery}");
					using SqlDataReader reader = selectCommand.ExecuteReader();
					// Create a new table in the destination database
					using SqlBulkCopy bulkCopy = new(destinationConnection);
					Log.Write($"Копируем таблицу: {tableName}");
					bulkCopy.DestinationTableName = tableName;
					bulkCopy.WriteToServer(reader);
					Log.Write($"Копирование завершено.");
					Log.Separate();
					txtLog.Text = Log.Read();
				}
				catch { Log.Write($"Ошибка копирования таблицы: {tableName}"); }
			}
		}
	}
}
