using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
}
