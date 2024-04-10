using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLBuilder
{
	internal class Log
	{
		private static readonly string fFileNameLog = @"SQLBuilder.log";

		// Читаем весь лог в строку
		public static string Read()
		{
			if (File.Exists(fFileNameLog))
			{
				return File.ReadAllText(fFileNameLog);
			}
			else
			{
				return ""; 
			}
		}

		// Записываем строку в лог-файл
		public static void Write(string sLog)
		{
			File.AppendAllText(fFileNameLog, string.Concat(sLog,Environment.NewLine));
		}

		// Записываем разделитель в лог-файл
		public static void Separate()
		{
			Write(string.Concat("----- ",DateTime.Now.ToString(), " -----"));
		}
	}
}
