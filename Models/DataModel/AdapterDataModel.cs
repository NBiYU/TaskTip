using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTip.Models.DataModel
{
    public class AdapterDataModel
    {
        /// <summary>
        /// 适配器描述（适配器的名称或描述）
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 适配器的物理地址（MAC 地址）
        /// </summary>
        public string MACAddress { get; set; }
        /// <summary>
        /// 是否启用了 DHCP
        /// </summary>
        public bool DHCPEnabled { get; set; }
        /// <summary>
        /// 分配给适配器的 IP 地址（可能是一个数组）
        /// </summary>
        public string[] IPAddress { get; set; }
        /// <summary>
        /// 适配器的子网掩码（可能是一个数组）
        /// </summary>
        public string[] IPSubnet { get; set; }
        /// <summary>
        /// 默认网关（可能是一个数组）
        /// </summary>
        public string[] DefaultIPGateway { get; set; }
        /// <summary>
        /// DNS 服务器列表
        /// </summary>
        public string[] DNSServerSearchOrder { get; set; }
        /// <summary>
        /// DNS 主机名
        /// </summary>
        public string DNSHostName { get; set; }
        /// <summary>
        /// DNS 域
        /// </summary>
        public string DNSDomain { get; set; }
        /// <summary>
        /// 提供 DHCP 服务的服务器
        /// </summary>
        public string DHCPServer { get; set; }
        /// <summary>
        /// 网络适配器是否启用 IP
        /// </summary>
        public bool IPEnabled { get; set; }
    }
}
