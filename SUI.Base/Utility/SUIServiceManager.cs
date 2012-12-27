using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceProcess;
using SUI.Base.Win;

namespace SUI.Base.Utility
{
    public class SUIServiceManager
    {
        private static SUIServiceManager instance = null;
        private SUIServiceManager()
        { }

        public static SUIServiceManager GetInstance()
        {
            if(instance == null)
                instance = new SUIServiceManager();
            return instance;
        }

        public IDictionary<string,ServiceController> Services
        {
            get
            {
                IDictionary<string, ServiceController> dictionary = new Dictionary<string, ServiceController>();
                ServiceController[] services = ServiceController.GetServices();
                foreach (ServiceController sc in services)
                {
                    dictionary.Add(sc.ServiceName, sc);
                }
                return dictionary;
            }
        }

        public ServiceControllerStatus QueryServiceStatus(string serviceName)
        {
            try
            {
                return Services[serviceName].Status;
            }
            catch
            {
                throw new Exception("Can not find expected service.");
            }
        }

        public bool StopService(string serviceName)
        {
            try
            {
                ServiceController sc = Services[serviceName];
                sc.Stop();
                sc.WaitForStatus(ServiceControllerStatus.Stopped,new TimeSpan(0,0,60));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool StartService(string serviceName)
        {
            try
            {
                ServiceController sc = Services[serviceName];
                sc.Start();
                sc.WaitForStatus(ServiceControllerStatus.Running,new TimeSpan(0,0,60));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool RestartService(string serviceName)
        {
            try
            {
                StopService(serviceName);
                StartService(serviceName);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
