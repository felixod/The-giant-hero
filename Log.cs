namespace SQLBuilder
{
	internal class Log
	{
		private static string? _config;
		private static string fFileNameLog = @"SQLBuilder.log";

		public static void ConfigPrefix(string? config)
		{
			_config = config;
		}

		// Читаем весь лог в строку
		public static string Read()
		{
			if (_config != null)
			{
				fFileNameLog = $"SQLBuilder_{_config}.log";
			}
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
			if (_config != null)
			{
				fFileNameLog = $"SQLBuilder_{_config}.log";
			}
			File.AppendAllText(fFileNameLog, string.Concat(sLog, Environment.NewLine));
		}

		// Записываем разделитель в лог-файл
		public static void Separate()
		{
			Write(string.Concat("----- ", DateTime.Now.ToString(), " -----"));
		}
	}
}
