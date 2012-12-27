using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace SUI.Base.Utility
{
    public class IniFile
    {
        private string path;
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section,
            string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,
                 string key, string def, StringBuilder retVal,
            int size, string filePath);
        [DllImport("KERNEL32.DLL", EntryPoint = "GetPrivateProfileStringW",
         SetLastError = true,
         CharSet = CharSet.Unicode, ExactSpelling = true,
         CallingConvention = CallingConvention.StdCall)]
        private static extern int GetPrivateProfileString(
          string lpAppName,
          string lpKeyName,
          string lpDefault,
          string lpReturnString,
          int nSize,
          string lpFilename);

        public IniFile(string INIPath)
        {
            try
            {
                FileInfo file = new FileInfo(INIPath);
                if (!file.Exists || !file.Extension.Equals(".ini"))
                {
                    throw new Exception();
                }
            }
            catch
            {
                throw new Exception("Fail to read data from this file.");
            }

            path = INIPath;
        }

        public void WriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.path);
        }


        public string ReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(1024);
            int i = GetPrivateProfileString(Section, Key, "", temp,
                                            1024, this.path);
            return temp.ToString();

        }

        public List<string> SectionList
        {
            get
            {
                string temp = new string(' ', 65536);
                GetPrivateProfileString(null, null, null, temp, 65536, this.path);
                List<string> result = new List<string>(temp.Split('\0'));
                result.RemoveRange(result.Count - 2, 2);
                return result;
            }
        }

        public List<string> ReadKeyList(string section)
        {
            string returnString = new string(' ', 65536);
            GetPrivateProfileString(section, null, null, returnString, 65536, this.path);
            List<string> result = new List<string>(returnString.Split('\0'));
            result.RemoveRange(result.Count - 2, 2);
            return result;
        }
    } 
}
