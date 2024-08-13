using System.Text;
using System.Xml;

namespace SQLBuilder
{
	public static class xmlHelper
	{
		public static string RemoveInvalidXmlChars(string input)
		{
			StringBuilder cleanedString = new();

			foreach (char c in input)
			{
				if (XmlConvert.IsXmlChar(c))
				{
					cleanedString.Append(c);
				}
			}

			return cleanedString.ToString();
		}
	}

	public class TreeViewXmlLoader
	{
		// Метод для загрузки TreeView из XML
		public static void LoadTreeViewFromXml(System.Windows.Forms.TreeView treeView, string fileName)
		{
			XmlDocument xmlDoc = new();
			if (File.Exists(fileName))
			{
				xmlDoc.Load(fileName);
				// Проверка на наличие корневого элемента
				if (xmlDoc.DocumentElement == null)
				{
					throw new InvalidOperationException("XML-документ не содержит корневого элемента.");
				}
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
			// Создание объявления XML
			XmlDeclaration? xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
			XmlElement? root = doc.DocumentElement;
			doc.InsertBefore(xmlDeclaration, root);

			// Создание корневого элемента
			XmlElement rootElement = doc.CreateElement("Node");
			rootElement.SetAttribute("Name", "Святогор");
			rootElement.SetAttribute("Prefix", "");
			rootElement.SetAttribute("Id", "0");
			rootElement.SetAttribute("Code", "");

			doc.AppendChild(rootElement);

			// Сохранение файла
			doc.Save(filePath);
		}

		// Рекурсивный метод для создания TreeNode из XML узла
		private static TreeNode CreateTreeNode(XmlNode xmlNode)
		{
			TreeNode treeNode = new();
			if (xmlNode.Attributes != null && xmlNode.Name == "Node")
			{
				XmlAttribute? nameAttribute = xmlNode.Attributes["Name"];
				if (nameAttribute != null)
				{
					treeNode.Name = nameAttribute.Value;
					treeNode.Text = nameAttribute.Value; // Используем Name для свойства Text
				}

				XmlAttribute? codeAttribute = xmlNode.Attributes["Id"];
				if (codeAttribute != null)
				{
					treeNode.Tag = int.Parse(codeAttribute.Value); // Сохраняем Id в Tag
				}
			}

			foreach (XmlNode childNode in xmlNode.ChildNodes)
			{
				if (childNode.Name == "Node")
				{
					treeNode.Nodes.Add(CreateTreeNode(childNode)); // Рекурсивно добавляем дочерние узлы
				}
			}
			return treeNode;
		}
	}
}
