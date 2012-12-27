using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Threading;
using System.Resources;
using System.Globalization;
namespace SUI.Base.Resource
{
    public class SUIResourceFileParser
    {
        private ResourceManager rm;
        public static string PrefixOfCultureName
        {
            get
            {
                CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                if (cultureInfo.LCID == 1028 || cultureInfo.LCID == 2052)  //if language is traditional Chinese or Simplied Chinese , adopting different measurement
                {
                    return cultureInfo.ThreeLetterWindowsLanguageName.ToLower();
                }
                else
                {
                    return cultureInfo.TwoLetterISOLanguageName;
                }
            }
        }
        public SUIResourceFileParser(string BaseName, Assembly assembly)
        {
            this.rm = new ResourceManager(BaseName,assembly);
        }
        public string GetString(string StringID)
        {
            try
            {
                return rm.GetString(StringID);
            }
            catch (Exception e)
            {
                throw new SUIException("Get string from .resource file failed!");
            }
        }
    }
}
