using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using System.Text.RegularExpressions;
using SUI.Base.SUIExceptions;
using Microsoft.Win32;
using System.Diagnostics;
using SUI.Base.UIControls.WinControls.Win32;
using System.Threading;
using SUI.Base.Win;
using EnvDTE80;
using System.Collections;
using SUI.Base.UIControls.WinControls.Dotnet;

namespace SUI.Base.Utility
{
    public abstract class SUIUtil
    {
        public static string PREFIX_OF_DY_CONTROL = "CO";
        public static string DELIMITER_OF_DY_CONTROL = "_";
        public static IDictionary<string, string> RuntimeCache = new Dictionary<string, string>();
        public static IDictionary Environments
        {
            get
            {
                return System.Environment.GetEnvironmentVariables();
            }
        }
        public static string GenerateDlgGUID(SUIDialog sui)
        {
            string DlgGUID = "NoItem";
            SUIWindow firstChild = null;
            IntPtr ptr = SUIWinAPIs.GetWindow(sui.WindowHandle, 5);
            if (IntPtr.Zero.Equals(ptr))
                return DlgGUID;
            firstChild = new SUIWindow(ptr);
            if (firstChild != null)
            {
                DlgGUID = firstChild.ClassName.Trim() + (uint)firstChild.WindowID + (firstChild.Width - firstChild.X);
            }
            return DlgGUID;
        }
        public static string GenerateDlgBaseName(SUIDialog suiDlg)
        {
            string Name = "NoTitleDialog";
            if (!suiDlg.WindowText.Equals(string.Empty))
            {
                Name = suiDlg.WindowText.Replace(" ", "") + "_Dialog";
            }
            return Name;
        }
        public static string GetRandomNumber()
        {
            Random rm = new Random();
            return rm.Next(1, 20).ToString();
        }
        public static int MakeIntWParam(int loWord, int hiWord)
        {
            return ((hiWord * 0x10000) | (loWord & 0xffff));
        }

        public static IntPtr MakeWParam(int loWord, int hiWord)
        {
            return new IntPtr(MakeIntWParam(loWord, hiWord));
        }

        private static string isVistaStr = null;
        private static bool isXP = System.Environment.OSVersion.VersionString.Contains(@"5.1"); // 5.1 is XP version
        public static bool IsCurrentOSVista
        {
            get
            {
                string OSVrsion = System.Environment.OSVersion.VersionString;
                if (OSVrsion.IndexOf("Windows NT 6.") > -1)
                {
                    //string value = GetRegistryKeyValue(Registry.LocalMachine,@"SOFTWARE\Microsoft\Windows NT\CurrentVersion","ProductName");
                    //if (value.IndexOf("Vista") > -1)
                    //{
                        return true;
                    //}
                    //return false;
                }
                    
                return false;

                //if (isVistaStr == null)
                //{
                //    string vista = Environment.GetEnvironmentVariable("Vista", EnvironmentVariableTarget.User);
                //    if (vista != null && vista.Equals("1"))
                //        isVistaStr = Boolean.TrueString;
                //    else
                //        isVistaStr = Boolean.FalseString;
                //}
                //return Boolean.Parse(isVistaStr);
            }
        }
        public static bool IsCurrentOSXP
        {
            get
            {
                return isXP;
            }
        }

        private static string isX64Str = null;
        public static bool IsX64
        {
            get
            {
                if (isX64Str == null)
                {
                    string x64 = Environment.GetEnvironmentVariable("x64", EnvironmentVariableTarget.User);
                    if (x64 != null && x64.Equals("1"))
                        isX64Str = Boolean.TrueString;
                    else
                        isX64Str = Boolean.FalseString;
                }
                return Boolean.Parse(isX64Str);
            }
        }

        public static String getStringFromTxt(String fileName)
        {
            try
            {
                StreamReader sr = new StreamReader(fileName);
                String result = sr.ReadToEnd();
                return result;
            }
            catch (Exception e)
            {
                throw (new SUIFileException(fileName,"ERROR when read the txt file of ",SUIFileException.FileOperationType.Read,e));
            }
        }

        //this method can save the file while the file is not exist.
        public static String getStringFromTxtWithSave(String fileName,String content)
        {
            if (File.Exists(fileName))
            {
                return getStringFromTxt(fileName);
            }
            else
            {
                try
                {
                    //StreamWriter writer = File.CreateText(fileName);
                    //writer.Write(content);
                    //writer.Flush();
                    //writer.Close();
                    OutputFileFromString(content, fileName);
                    return content;
                }
                catch(Exception e)
                {
                    throw (new SUIFileException(fileName, "SUIERROR when write the txt file of ",SUIFileException.FileOperationType.Write,e));
                }
            }
        }

        public static void OutputFileFromString(string str,string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                StreamWriter writer = File.CreateText(fileName);
                writer.Write(str);
                writer.Flush();
                writer.Close();
            }catch(Exception e)
            {
                throw (new SUIFileOutPutException(e));
            }
        }
        ////this method save the temp map if included in the design time html
        //public static void saveMapsForHtml(string html,string standardFileName)
        //{
        //    MatchCollection mc;
        //    String systemp = SpecialDirectories.Temp;
        //    String tempImgPath = "\\cr_tmp_image_\\dynamic_images\\";
        //    Regex r = new Regex(systemp+tempImgPath+"cr_tmp_image_\\[a-f0-9]{8}\\-[a-f0-9]{4}\\-[a-f0-9]{4}\\-[a-f0-9]{4}\\-[a-f0-9]{12}\\.png", RegexOptions.IgnoreCase);
        //    mc = r.Matches(html);
        //    for (int i = 0; i < mc.Count; i++)
        //    {
        //        string imgPath = mc[i].Value.Substring(1);
        //        try{
        //            SUIBitmap bitmap = new SUIBitmap(imgPath);
        //            bitmap.Save(Env standardFileName);
        //        }catch(Exception e)
        //        {

        //        }
        //    }
        //}

        public static string getFilePathFromFileFullPath(string fileFullPath)
        {
            string path = null;
            try
            {
                path = FileSystem.GetParentPath(fileFullPath);
            }
            catch (Exception e)
            {
                return null;
            }

            return path;
        }

        public static string getFileNameFromFileFullPath(string filePath)
        {
            //if the filepath is just the file name, the path will return a zero length string.
            string path = getFilePathFromFileFullPath(filePath);

            if (path == null)
            {
                return null;
            }

            return (filePath.Replace(path + "\\", ""));
        }

        public static string convertHtml(string oldStr)
        {
            //guid
            MatchCollection mc;
            Regex r = new Regex("\\.[a-f0-9]{10}\\-[a-f0-9]{4}\\-[a-f0-9]{4}\\-[a-f0-9]{4}\\-[a-f0-9]{12}\\-[a-f0-9]{1}", RegexOptions.IgnoreCase);
            mc = r.Matches(oldStr);
            String result = oldStr;
            for (int i = 0; i < mc.Count; i++)
            {
                string replaceString = mc[i].Value.Substring(1);
                string newString = "1234567890" + i;
                result = result.Replace(replaceString, newString);
            }

            //temp rpt id
            r = new Regex("[a-f0-9]{8}\\-[a-f0-9]{4}\\-[a-f0-9]{4}\\-[a-f0-9]{4}\\-[a-f0-9]{12}", RegexOptions.IgnoreCase);
            mc = r.Matches(result);
            for (int i = 0; i < mc.Count; i++)
            {
                string replaceString = mc[i].Value;
                string newString = "1234567890" + i;
                result = result.Replace(replaceString, newString);
            }
            //r = new Regex("map[0-9]{18}_[0-9]{1}",RegexOptions.IgnoreCase);
            //mc = r.Matches(result);
            //for (int i = 0; i < mc.Count; i++)
            //{
            //    string replaceString = mc[i].Value.Substring(1);
            //    string newString = "1234567890" + i;
            //    result = result.Replace(replaceString, newString);
            //}

            //r = new Regex("[a-f0-9]{8}\\-[a-f0-9]{4}\\-[a-f0-9]{4}\\-[a-f0-9]{4}\\-[a-f0-9]{12}", RegexOptions.IgnoreCase);
            //mc = r.Matches(result);
            //for (int i = 0; i < mc.Count; i++)
            //{
            //    string replaceString = mc[i].Value.Substring(1);
            //    string newString = "1234567890" + i;
            //    result = result.Replace(replaceString, newString);
            //}
            return result;
        }

        public static bool IsStringNullOrEmpty(string str)
        {
            return (str == null || str.Equals(string.Empty));
        }

        //get the string of specified index after the sourceString split by split string,
        //the index is begin from 1,
        public static string getStringAfterSpliting(string sourceStr, string splitStr, int index)
        {
            string[] strArray = sourceStr.Split(splitStr.ToCharArray(), StringSplitOptions.None);
            return strArray[index-1];
        }
        
        public static void SetRegistryKeyValue(RegistryKey rootKey, string subkeyName, string name, object value)
        {
            RegistryKey key = null;
            try
            {
                rootKey.OpenSubKey(subkeyName);
                if (key == null)
                {
                    RegistryKey parentKey = rootKey;
                    foreach (string keyName in subkeyName.Split('\\'))
                    {
                        key = parentKey.CreateSubKey(keyName);
                        parentKey = key;
                    }
                }
                key.SetValue(name, value);
            }
            catch
            {
                //Ignor this exception...
                //The worst case is to write registry during execution of next case.
            }
            finally
            {
                if (key != null)
                    key.Close();
            }
        }

        public static string GetRegistryKeyValue(RegistryKey rootKey, string subkeyName, string name)
        {

            RegistryKey parentKey = rootKey;
            RegistryKey key = null;
            try
            {
                foreach (string keyName in subkeyName.Split('\\'))
                {
                    key = parentKey.OpenSubKey(keyName);
                    parentKey = key;
                }
                return key.GetValue(name).ToString();
            }
            catch
            {
                //Ignor this exception...
                //The worst case is to write registry during execution of next case.
                return null;
            }
            finally
            {
                if (key != null)
                    key.Close();
            }
        }

        public static bool IsManagedProcess(Process proc)
        {
            bool ismanaged = false;
	        for(int i = 0;i<proc.Modules.Count;i++) {
		        if(proc.Modules[i].ModuleName.Equals("mscorlib.dll") ||
			        proc.Modules[i].ModuleName.Equals("mscorlib.ni.dll")) {
				        //make sure its version 2.0
				        System.Reflection.AssemblyName name = System.Reflection.AssemblyName.GetAssemblyName(proc.Modules[i].FileName);
				        if ((!name.Equals(string.Empty))&& name.Version.Major == 2) {
					        ismanaged = true;
				        }
			        break;
		        }
	        }
            return false;
        }
        public static void KillProcess(string processName)
        {
            Process[] ps = Process.GetProcessesByName(processName);

            if (ps.Length != 0)
            {
                foreach (Process p in ps)
                {
                    try
                    {
                        p.Kill();
                        p.WaitForExit(10000);
                    }
                    catch
                    { }
                }
            }
        }

        public static DTE2 GetDTEObjectFromProcess(Process pro)
        {
            DTE2 dte = null;
            Hashtable table = SUIWinAPIs.GetRunningObjectTable();
            foreach (string key in table.Keys)
            {
                if (key.Contains(pro.Id.ToString()))
                {
                    dte = (DTE2)table[key];
                }
            }
            return dte;
        }
        public static string GetIDNameFromDotNetCtrl(SUIDotNetControl ctrl)
        {
            string name = "";
            name = ctrl.ID;

            if (name.Length <= 0)
            {
                SUIDotNetControl myCtrl = ctrl;
                int index = 0;
                IList<SUIWindow> ancestors = TrackWinAncestors(ctrl);
                while (myCtrl.ID.Length <= 0 || index >= ancestors.Count)
                {
                    name += PREFIX_OF_DY_CONTROL;
                    name += DELIMITER_OF_DY_CONTROL;
                    for (int i = 0; myCtrl.Parent != null && i <= myCtrl.Parent.Children.Count; i++)
                    {
                        if (myCtrl.Parent.Children[i].WindowHandle.Equals(myCtrl.WindowHandle))
                        {
                            name += i;
                            name += DELIMITER_OF_DY_CONTROL;
                            break;
                        }
                    }
                    if (myCtrl.Parent != null)
                    {
                        myCtrl = new SUIDotNetControl(myCtrl.Parent);
                        if (myCtrl.ID.Length > 0)
                        {
                            name += myCtrl.ID;
                            break;
                        }
                    }
                    else
                        break;
                    index++;
                }
            }

            return name;
        }

        public static SUIDotNetControl GetObjectFromName(SUIDotNetControl rootForm, string name)
        {
            string prefix = PREFIX_OF_DY_CONTROL + DELIMITER_OF_DY_CONTROL;
            string temp = "";

            if (name.IndexOf(prefix) >= 0)
            {
                ArrayList indexList = new ArrayList();
                temp = name;
                while (temp.IndexOf(prefix) >= 0)
                {
                    indexList.Add(int.Parse(temp[prefix.Length].ToString()));
                    temp = temp.Substring(prefix.Length + 2, temp.Length - prefix.Length - 2);
                }
                SUIDotNetControl myCtrl = rootForm.FindControl(temp);
                if (myCtrl == null) return null;
                for (int i = 0; i < indexList.Count; i++)
                {
                    myCtrl = new SUIDotNetControl(myCtrl.Children[(int)(indexList[indexList.Count - i - 1])]);
                }
                return myCtrl;
            }
            else
            {
                return rootForm.FindControl(name);
            }
        }

        private static IList<SUIWindow> TrackWinAncestors(SUIWindow actionOnSomething)
        {
            IList<SUIWindow> allAncestorsincludingSelf = new List<SUIWindow>();
            while (!actionOnSomething.ClassName.Equals("#32769"))
            {
                allAncestorsincludingSelf.Add(actionOnSomething);
                actionOnSomething = actionOnSomething.Ancestor;
            }
            Trace.WriteLine("total parent is : " + allAncestorsincludingSelf.Count);
            return allAncestorsincludingSelf;
        }

        //This utility method is used to search environment variable from current process, current user and then local machine.
        public static string GetEnvironmentVariable(string variable)
        {
            string varValue = Environment.GetEnvironmentVariable(variable, EnvironmentVariableTarget.Process);
            if(varValue == null)
                varValue = Environment.GetEnvironmentVariable(variable, EnvironmentVariableTarget.User);
            if(varValue == null)
                Environment.GetEnvironmentVariable(variable, EnvironmentVariableTarget.Machine);
            return varValue;
        }

        public static class SystemStrings
        {
            public static string ProgramFiles = "Program Files";
            public static string EnvProgramFilesName = "ProgramFiles";
        }
    }
}
