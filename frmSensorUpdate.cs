using System.Data;
using System.Xml.Linq;

namespace SQLBuilder
{
	public partial class frmSensorUpdate : Form
	{
		private int _itemId;
		private int _sensorId;

		public frmSensorUpdate(int itemId, int sensorId)
		{
			InitializeComponent();
			_itemId = itemId; // Сохраняем переданный ID элемента
			_sensorId = sensorId; // Сохраняем переданный ID сенсора
		}

		private void frmSensorUpdate_Load(object sender, EventArgs e)
		{
			LoadSensor("departments.xml");
		}

		private void LoadSensor(string xmlFilePath)
		{
			XDocument doc = XDocument.Load(xmlFilePath);

			var sensorInfo = doc.Descendants("Node")
				.Where(node => (int)node.Attribute("Id") == _itemId)
				.Elements("Sensor")
				.Where(sensor => (int)sensor.Attribute("Id") == _sensorId)
				.Select(sensor => new
				{
					Id = (int)sensor.Attribute("Id"),
					Name = (string)sensor.Attribute("Name"),
					Type = (string)sensor.Attribute("Type"),
					Description = (string)sensor.Attribute("Description"),
					UserDescription = (string)sensor.Attribute("UserDescription"),
					Prefix = (string)sensor.Attribute("Prefix")
				})
				.FirstOrDefault();

			if (sensorInfo != null)
			{
				txtName.Text = sensorInfo.UserDescription;
				nudPrefix.Text = sensorInfo.Prefix;
			}
			else
			{
				Console.WriteLine("Sensor не найден.");
			}
		}

		private void SaveSensor(string xmlFilePath)
		{
			XDocument doc = XDocument.Load(xmlFilePath);

			var sensor = doc.Descendants("Node")
				.Where(node => (int)node.Attribute("Id") == _itemId)
				.Elements("Sensor")
				.Where(sensor => (int)sensor.Attribute("Id") == _sensorId)
				.FirstOrDefault();

			if (sensor != null)
			{
				sensor.SetAttributeValue("UserDescription", xmlHelper.RemoveInvalidXmlChars(txtName.Text));
				sensor.SetAttributeValue("Prefix", xmlHelper.RemoveInvalidXmlChars(nudPrefix.Text));

				doc.Save(xmlFilePath); // Сохранение изменений 
			}
		}

		private void cmdOK_Click(object sender, EventArgs e)
		{
			SaveSensor("departments.xml");
			this.Close();
		}
	}
}