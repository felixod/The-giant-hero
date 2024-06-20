using System.Security.Cryptography;
using System.Text;

namespace SQLBuilder
{
	internal class Crypt
	{
		public static string Encrypt(string text, byte[] key)
		{
			using Aes aes = Aes.Create();
			aes.Key = key;
			using MemoryStream ms = new();
			ms.Write(aes.IV);
			using (CryptoStream cs = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Write, true))
			{
				cs.Write(Encoding.UTF8.GetBytes(text));
			}
			Log.Write("Строка зашифрована");
			return Convert.ToBase64String(ms.ToArray());
		}

		public static string Decrypt(string base64, byte[] key)
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
			Log.Write("Строка дешифрована");
			return Encoding.UTF8.GetString(output.ToArray());
		}
	}
}
