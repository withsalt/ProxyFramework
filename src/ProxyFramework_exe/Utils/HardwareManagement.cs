using System;
using System.Collections.Generic;
using System.Management;
using System.Text;

namespace ProxyFramework.Utils
{
    public class HardwareManagement
    {
        public static string GetCPUID()
        {
            ManagementClass mc = new ManagementClass("Win32_Processor");
            ManagementObjectCollection moc = mc.GetInstances();
            string strID = null;
            foreach (ManagementObject mo in moc)
            {
                strID = mo.Properties["ProcessorId"].Value.ToString();
                break;
            }
            return strID;
        }
    }
}
