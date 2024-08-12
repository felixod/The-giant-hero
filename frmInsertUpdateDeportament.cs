using System.Xml;
using System.Xml.Linq;


namespace SQLBuilder
{
	public partial class frmInsertUpdateDeportament : Form
	{
		private int _itemId;
		private bool _insert;

		public (string Name, string Prefix, string Code) GetAttributesById(string xmlFilePath, int nodeId)
		{
			XmlDocument doc = new();
			doc.Load(xmlFilePath);

			XmlNode node = doc.SelectSingleNode($"//Node[@Id='{nodeId.ToString()}']");

			if (node != null)
			{
				string name = node.Attributes["Name"]?.InnerText;
				string prefix = node.Attributes["Prefix"]?.InnerText;
				string code = node.Attributes["Code"]?.InnerText;

				return (name, prefix, code);
			}

			return (null, null, null);
		}


		public static int FindMaxIdAndIncrement(string xmlFilePath)
		{

			if (File.Exists(xmlFilePath))
			{
				XDocument xmlDoc = XDocument.Load(xmlFilePath);

				// Находим максимальный Id среди всех элементов Node
				int maxId = xmlDoc.Descendants("Node")
								  .Attributes("Id")
								  .Select(Id => (int)Id)
								  .Max();

				// Увеличиваем максимальный Id на 1
				return maxId + 1;
			}
			else
			{
				return 0;
			}

		}

		public static void AddElementToParentById(string xmlFilePath, int parentId, string newName, string newPrefix, string newCode, int Id)
		{
			if (File.Exists(xmlFilePath))
			{
				XDocument xmlDoc = XDocument.Load(xmlFilePath);
				// Находим родительский элемент с заданным id
				#pragma warning disable CS8600 // Преобразование литерала, допускающего значение NULL или возможного значения NULL в тип, не допускающий значение NULL.
				XElement parentElement = xmlDoc.Descendants("Node").FirstOrDefault(node => node.Attribute("Id")?.Value == parentId.ToString());
				#pragma warning restore CS8600 // Преобразование литерала, допускающего значение NULL или возможного значения NULL в тип, не допускающий значение NULL.

				if (parentElement != null)
				{
					// Создаем новый элемент
					XElement newElement = new("Node",
						new XAttribute("Name", xmlHelper.RemoveInvalidXmlChars(newName)),
						new XAttribute("Prefix", xmlHelper.RemoveInvalidXmlChars(newPrefix)),
						new XAttribute("Code", xmlHelper.RemoveInvalidXmlChars(newCode)),
						new XAttribute("Id", Id)
					);

					// Добавляем новый элемент к родительскому
					parentElement.Add(newElement);

					// Сохраняем изменения в XML файле
					xmlDoc.Save(xmlFilePath);
				}
			}




		}

		public static void UpdateElementById(string xmlFilePath, int Id, string newName, string newPrefix, string newCode)
		{
			if (File.Exists(xmlFilePath))
			{
				XDocument xmlDoc = XDocument.Load(xmlFilePath);

				// Находим элемент с заданным id
#pragma warning disable CS8600 // Преобразование литерала, допускающего значение NULL или возможного значения NULL в тип, не допускающий значение NULL.
				XElement elementToUpdate = xmlDoc.Descendants("Node").FirstOrDefault(node => node.Attribute("Id")?.Value == Id.ToString());
#pragma warning restore CS8600 // Преобразование литерала, допускающего значение NULL или возможного значения NULL в тип, не допускающий значение NULL.

				if (elementToUpdate != null)
				{
					// Обновляем атрибуты элемента
					elementToUpdate.SetAttributeValue("Name", xmlHelper.RemoveInvalidXmlChars(newName));
					elementToUpdate.SetAttributeValue("Prefix", xmlHelper.RemoveInvalidXmlChars(newPrefix));
					elementToUpdate.SetAttributeValue("Code", xmlHelper.RemoveInvalidXmlChars(newCode));

					// Сохраняем изменения в XML файле
					xmlDoc.Save(xmlFilePath);
				}
			}

		}


		public frmInsertUpdateDeportament(bool insert, int itemId)
		{
			InitializeComponent();
			_itemId = itemId; // Сохраняем переданный ID элемента
			_insert = insert;
		}

		private void frmInsertUpdateDeportament_Load(object sender, EventArgs e)
		{
			UpdateButton();
			if (!_insert)
			{
				var result = GetAttributesById(Program._department, _itemId);
				txtName.Text = result.Name;
				txtPrefix.Text = result.Prefix;
				txtCode.Text = result.Code;
			}
		}

		private void cmdOK_Click(object sender, EventArgs e)
		{
			if (_insert)
			{
				int newId = FindMaxIdAndIncrement(Program._department);
				if (_itemId == 0)
				{
					AddElementToParentById(Program._department, _itemId, txtName.Text, txtPrefix.Text, txtCode.Text, newId);
				}
				else
				{
					AddElementToParentById(Program._department, _itemId, txtName.Text, txtPrefix.Text, txtCode.Text, newId);
				}
			}
			else
			{
				UpdateElementById(Program._department, _itemId, txtName.Text, txtPrefix.Text, txtCode.Text);
			}

			this.Close();
		}

		private void UpdateButton()
		{
			// Проверяем, заполнены ли все три поля
			bool isNameFilled = !string.IsNullOrWhiteSpace(txtName.Text);

			// Если поле заполнено, включаем кнопку, иначе отключаем
			cmdOK.Enabled = isNameFilled;
		}

		private void cmdClose_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void txtName_TextChanged(object sender, EventArgs e)
		{
			UpdateButton();
		}
	}
}
