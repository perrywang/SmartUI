using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Collections;
namespace SUI.Base.Resource
{
    public class SUIResourceManager
    {
        private static SUIResourceManager m_ResourceManager = null;
        private Hashtable m_Handlers = new Hashtable();
        public SUIResourceManager() :this(true)
        {}
        public SUIResourceManager(bool isDefaultHandler)
        {
            if (isDefaultHandler)
            {
                SUIWin32DialogStringHandler.Register(this);
                SUIWin32StringHandler.Register(this);
                SUIManagedStringHandler.Register(this);
            }
        }
        public void RegisterResourceHandler(SUIStringResourceHandler stringHandler)
        {
            this.m_Handlers[stringHandler.StringType] = stringHandler;
        }
        public static string GetInternationalString(string DllPath, object stringID, StringResourceType stringType)
        {
            SUIStringResource stringResource = new SUIStringResource(DllPath, stringID, stringType);
            return GetInternationalString(stringResource);
        }
        public static string GetInternationalString(SUIStringResource stringResource)
        {
            if (m_ResourceManager == null)
            {
                m_ResourceManager = new SUIResourceManager();
            }
            SUIStringResourceHandler handler = (SUIStringResourceHandler)m_ResourceManager.m_Handlers[stringResource.StringType];
            return handler.ExtractString(stringResource);
        }
        public static string GetInternationalString(string DllPath, string baseName, object stringID)
        {
            SUIStringResource stringResource = new SUIStringResource(DllPath, baseName, stringID);
            return GetInternationalString(stringResource);
        }

        
    }
}
