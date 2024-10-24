using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using TaskTip.Common.Extends;
using TaskTip.Models.DataModel;

namespace TaskTip.Common.Helpers
{
    public static class NetworkAdapterHelper
    {
        public static ManagementObject GetNetworkAdapterInstance(string adapterName)
        {
            string query = "SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = TRUE";

            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection moc = searcher.Get();

            foreach (ManagementObject mo in moc)
            {
                if ((bool)mo["IPEnabled"] == true && mo["Caption"].ToString().Contains(adapterName))
                {
                    return mo;
                }
            }
            return null;
        }
        public static AdapterDataModel GetNetworkAdapterInfo(string adapterName)
        {
            var mo = GetNetworkAdapterInstance(adapterName);
            if (mo != null)
            {
                return mo.GetNetworkAdapterInfo();
            }
            return null;
        }
        public static AdapterDataModel GetNetworkAdapterInfo(this ManagementObject mo)
        {
            try
            {
                return new AdapterDataModel
                {
                    IPEnabled = (bool)mo["IPEnabled"],
                    IPAddress = (string[])mo["IPAddress"],
                    Description = (string)mo["Description"],
                    MACAddress = (string)mo["MACAddress"],
                    DHCPEnabled = (bool)mo["DHCPEnabled"],
                    IPSubnet = (string[])mo["IPSubnet"],
                    DefaultIPGateway = (string[])mo["DefaultIPGateway"],
                    DNSServerSearchOrder = (string[])mo["DNSServerSearchOrder"],
                    DNSHostName = (string)mo["DNSHostName"],
                    DNSDomain = (string)mo["DNSDomain"],
                    DHCPServer = (string)mo["DHCPServer"]
                };
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static void SetIPWithDnsAddress(string adapterName, string ipAddress = "", string subnetMask = "", string gateway = "", string dns = "")
        {
            string query = "SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = TRUE";

            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection moc = searcher.Get();

            foreach (ManagementObject mo in moc)
            {
                if ((bool)mo["IPEnabled"] == true && mo["Caption"].ToString().Contains(adapterName))
                {
                    ManagementBaseObject newIP = mo.GetMethodParameters("EnableStatic");
                    if (!ipAddress.IsNullOrEmpty()) newIP["IPAddress"] = new string[] { ipAddress };
                    if (subnetMask.IsNullOrEmpty()) subnetMask = "255.255.255.0";
                    newIP["SubnetMask"] = new string[] { subnetMask };
                    mo.InvokeMethod("EnableStatic", newIP, null);

                    ManagementBaseObject newGateway = mo.GetMethodParameters("SetGateways");
                    if (!gateway.IsNullOrEmpty()) newGateway["DefaultIPGateway"] = new string[] { gateway };
                    mo.InvokeMethod("SetGateways", newGateway, null);

                    if (!dns.IsNullOrEmpty())
                    {
                        ManagementBaseObject newDNS = mo.GetMethodParameters("SetDNSServerSearchOrder");
                        newDNS["DNSServerSearchOrder"] = dns;
                        mo.InvokeMethod("SetDNSServerSearchOrder", newDNS, null);
                    }
                    break;
                }
            }
        }
        public static void SetIPWithDnsAddress(this ManagementObject mo, string ipAddress = "", string subnetMask = "", string gateway = "", string dns = "")
        {
            ManagementBaseObject newIP = mo.GetMethodParameters("EnableStatic");
            if (!ipAddress.IsNullOrEmpty()) newIP["IPAddress"] = new string[] { ipAddress };
            if (subnetMask.IsNullOrEmpty()) subnetMask = "255.255.255.0";
            newIP["SubnetMask"] = new string[] { subnetMask };
            mo.InvokeMethod("EnableStatic", newIP, null);

            ManagementBaseObject newGateway = mo.GetMethodParameters("SetGateways");
            if (!gateway.IsNullOrEmpty()) newGateway["DefaultIPGateway"] = new string[] { gateway };
            mo.InvokeMethod("SetGateways", newGateway, null);

            if (!dns.IsNullOrEmpty())
            {
                ManagementBaseObject newDNS = mo.GetMethodParameters("SetDNSServerSearchOrder");
                newDNS["DNSServerSearchOrder"] = dns;
                mo.InvokeMethod("SetDNSServerSearchOrder", newDNS, null);
            }
        }
        public static void SetIPWithDnsDHCP(string adapterName, bool DHCP = false)
        {
            string query = "SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = TRUE";

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
            {
                foreach (ManagementObject mo in searcher.Get())
                {
                    if (mo["Description"] != null && mo["Description"].ToString().Contains(adapterName))
                    {
                        if (DHCP)
                        {
                            mo.InvokeMethod("EnableDHCP", null);
                            var dnsParams = mo.GetMethodParameters("SetDNSServerSearchOrder");
                            dnsParams["DNSServerSearchOrder"] = null;
                            var dnsResult = mo.InvokeMethod("SetDNSServerSearchOrder", dnsParams, null);
                        }

                        break;
                    }
                }
            }
        }
        public static void SetIPWithDnsDHCP(this ManagementObject mo, bool DHCP = false)
        {
            if (DHCP)
            {
                mo.InvokeMethod("EnableDHCP", null);
                var dnsParams = mo.GetMethodParameters("SetDNSServerSearchOrder");
                dnsParams["DNSServerSearchOrder"] = null;
                var dnsResult = mo.InvokeMethod("SetDNSServerSearchOrder", dnsParams, null);
            }
        }
        public static List<string> GetNetworkInterfaces()
        {
            string qry = "SELECT * FROM MSFT_NetAdapter WHERE Virtual=False";
            ManagementScope scope = new ManagementScope(@"\\.\ROOT\StandardCimv2");
            ObjectQuery query = new ObjectQuery(qry);
            ManagementObjectSearcher mos = new ManagementObjectSearcher(scope, query);
            ManagementObjectCollection moc = mos.Get();
            var strs = new List<string>();
            foreach (ManagementObject mo in moc)
            {
                string driverDescription = mo["DriverDescription"]?.ToString();

                strs.Add(driverDescription);
            }
            return strs;
        }
    }
}
