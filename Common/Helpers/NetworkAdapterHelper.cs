using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using TaskTip.Common.Extends;

namespace TaskTip.Common.Helpers
{
    public static class NetworkAdapterHelper
    {
        public static void SetIPAddress(string adapterName, string ipAddress = "", string subnetMask = "", string gateway = "")
        {
            string qry = "SELECT * FROM MSFT_NetAdapter WHERE Virtual=False";
            ManagementScope scope = new ManagementScope(@"\\.\ROOT\StandardCimv2");
            ObjectQuery query = new ObjectQuery(qry);
            ManagementObjectSearcher mos = new ManagementObjectSearcher(scope, query);
            ManagementObjectCollection moc = mos.Get();

            foreach (ManagementObject mo in moc)
            {
                if ((bool)mo["IPEnabled"] == true && mo["Caption"].ToString().Contains(adapterName))
                {
                    ManagementBaseObject newIP = mo.GetMethodParameters("EnableStatic");
                    if(!ipAddress.IsNullOrEmpty()) newIP["IPAddress"] = new string[] { ipAddress };
                    if (!subnetMask.IsNullOrEmpty()) newIP["SubnetMask"] = new string[] { subnetMask };
                    mo.InvokeMethod("EnableStatic", newIP, null);

                    ManagementBaseObject newGateway = mo.GetMethodParameters("SetGateways");
                    if (!gateway.IsNullOrEmpty()) newGateway["DefaultIPGateway"] = new string[] { gateway };
                    mo.InvokeMethod("SetGateways", newGateway, null);
                }
            }
        }
        public static void ShowNetworkInterfaces()
        {
            SetIPAddress("");
            IPGlobalProperties computerProperties = IPGlobalProperties.GetIPGlobalProperties();
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            Console.WriteLine("Interface information for {0}.{1}     ",
                    computerProperties.HostName, computerProperties.DomainName);
            if (nics == null || nics.Length < 1)
            {
                Console.WriteLine("  No network interfaces found.");
                return;
            }

            Console.WriteLine("  Number of interfaces .................... : {0}", nics.Length);
            foreach (NetworkInterface adapter in nics)
            {
                IPInterfaceProperties properties = adapter.GetIPProperties();
                Console.WriteLine();
                Console.WriteLine(adapter.Description);
                Console.WriteLine(String.Empty.PadLeft(adapter.Description.Length, '='));
                Console.WriteLine("  Interface type .......................... : {0}", adapter.NetworkInterfaceType);
                Console.WriteLine("  Physical Address ........................ : {0}",
                           adapter.GetPhysicalAddress().ToString());
                Console.WriteLine("  Operational status ...................... : {0}",
                    adapter.OperationalStatus);
                string versions = "";

                // Create a display string for the supported IP versions.
                if (adapter.Supports(NetworkInterfaceComponent.IPv4))
                {
                    versions = "IPv4";
                }
                if (adapter.Supports(NetworkInterfaceComponent.IPv6))
                {
                    if (versions.Length > 0)
                    {
                        versions += " ";
                    }
                    versions += "IPv6";
                }
                Console.WriteLine("  IP version .............................. : {0}", versions);

                // The following information is not useful for loopback adapters.
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                {
                    continue;
                }
                Console.WriteLine("  DNS suffix .............................. : {0}",
                    properties.DnsSuffix);

                string label;
                if (adapter.Supports(NetworkInterfaceComponent.IPv4))
                {
                    IPv4InterfaceProperties ipv4 = properties.GetIPv4Properties();
                    Console.WriteLine("  MTU...................................... : {0}", ipv4.Mtu);
                    if (ipv4.UsesWins)
                    {

                        IPAddressCollection winsServers = properties.WinsServersAddresses;
                        if (winsServers.Count > 0)
                        {
                            label = "  WINS Servers ............................ :";
                        }
                    }
                }

                Console.WriteLine("  DNS enabled ............................. : {0}",
                    properties.IsDnsEnabled);
                Console.WriteLine("  Dynamically configured DNS .............. : {0}",
                    properties.IsDynamicDnsEnabled);
                Console.WriteLine("  Receive Only ............................ : {0}",
                    adapter.IsReceiveOnly);
                Console.WriteLine("  Multicast ............................... : {0}",
                    adapter.SupportsMulticast);

                Console.WriteLine();
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
