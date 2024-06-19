using System.Runtime.InteropServices;
using System.Text;

namespace SQLBuilder
{

    namespace ini
    {
        public class IniFile
        {
            [DllImport("kernel32")]
            private extern static long WritePrivateProfileString(string section, string key, string value, string filePath);
            [DllImport("kernel32")]
            private extern static int GetPrivateProfileString(string section, string key, string @default, StringBuilder retVal, int size, string filePath);

            private string IniPath;

            public IniFile(string iniPath)
            {
                this.IniPath = new FileInfo(iniPath).FullName.ToString();
            }

            public bool KeyExists(string section, string key)
            {
                return (this.ReadKey(section, key).Length > 0);
            }

            public string ReadKey(string section, string key)
            {
                StringBuilder retVal = new StringBuilder(0xFF);
                IniFile.GetPrivateProfileString(section, key, "", retVal, 0xFF, this.IniPath);
                return retVal.ToString();
            }

            public void WriteKey(string section, string key, string value)
            {
                IniFile.WritePrivateProfileString(section, key, value, this.IniPath);
            }

            public void DeleteKey(string section, string key)
            {
                this.WriteKey(section, key, "");
            }

            public void DeleteSection(string section)
            {
                this.WriteKey(section, "", "");
            }
        }
    }
}