using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
namespace SUI.Base.Resource
{
    public class SUIResource
    {
        private string  m_resourceDll;
        private object m_stringID;
        public SUIResource(string resDll,object stringID)
        {
            m_resourceDll = resDll;
            m_stringID = stringID;
        }
        public string ResourceDll
        {
            get
            {
                return m_resourceDll;
            }
        }
        public object StringID
        {
            get
            {
                return m_stringID;
            }
        }
    }
    public enum StringResourceType : int
    {
        Win32DialogString = 5,
        Win32String,
        ManagedString
    }
    public class SUIStringResource : SUIResource
    {
        private StringResourceType m_stringType;
        private Hashtable m_assemblyAndBaseName = new Hashtable();
        public SUIStringResource(string resourceFile, object stringID, StringResourceType stringType)
            : base(resourceFile,stringID)
        {
            m_stringType = stringType;
        }
        public SUIStringResource(string resourceFile, string baseName,object stringID)
            : base(resourceFile, stringID)
        {
            m_stringType = StringResourceType.ManagedString;
            if (m_assemblyAndBaseName.ContainsKey(resourceFile))
                return;
            m_assemblyAndBaseName.Add(resourceFile, baseName);
        }
        public StringResourceType StringType
        {
            get
            {
                return m_stringType;
            }
        }
        public Hashtable ManagedAssembly
        {
            get
            {
                return m_assemblyAndBaseName;
            }
        }

    }
}
