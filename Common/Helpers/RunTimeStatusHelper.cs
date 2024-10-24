using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace TaskTip.Common
{
    public static class RunTimeStatusHelper
    {
        #region 获取计数器信息
        public static string GetAllStatus()
        {
            var str = new StringBuilder();
            var currentStr = string.Empty;
            PerformanceCounterCategory[] categories = PerformanceCounterCategory.GetCategories();

            foreach (var category in categories)
            {
                try
                {
                    currentStr = $"CategoryName : {category.CategoryName} : {category.CategoryHelp}";
                    str.AppendLine(currentStr);
                    string[] instanceNames = category.GetInstanceNames();
                    foreach (string instanceName in instanceNames)
                    {
                        currentStr = $"\tInstanceName : {instanceName}";
                        str.AppendLine(currentStr);
                        PerformanceCounter[] counters = category.GetCounters(instanceName);
                        foreach (var counter in counters)
                        {
                            currentStr = $"\t\tCounter : {counter.CounterName} : {counter.CounterHelp}";
                            str.AppendLine(currentStr);
                        }
                    }
                }
                catch (Exception ex)
                {
                    str.AppendLine($"{currentStr}:×:{ex.Message}");
                }

            }
            var s = str.ToString();
            return s;
        }

        public static List<string> GetTargetCounters(string cagetory, string instance)
        {
            var instances = new PerformanceCounterCategory(cagetory);

            throw new NotImplementedException();
        }

        public static List<string> GetTargetInstanceNames(string cagetoryName)
        {
            var cagetory = new PerformanceCounterCategory(cagetoryName);
            return cagetory.GetInstanceNames().ToList();
        }

        #endregion

        #region 同步获取实时信息
        public static string GetCPUStatus()
        {
            var counter = new PerformanceCounter(categoryName: "Processor", "% Processor Time", "_Total");
            var cpuUsage = counter.NextValue();
            System.Threading.Thread.Sleep(100);
            cpuUsage = counter.NextValue();
            return $"{cpuUsage}%";
        }

        public static string GetMemoryStatus()
        {
            var searcher = new ManagementObjectSearcher("SELECT TotalVisibleMemorySize, FreePhysicalMemory FROM Win32_OperatingSystem");
            ulong totalMemory = 0;
            ulong freeMemory = 0;
            foreach (var item in searcher.Get())
            {
                totalMemory += (ulong)item["TotalVisibleMemorySize"];
                freeMemory += (ulong)item["FreePhysicalMemory"];
            }
            double totalMb = totalMemory / 1024.0;
            double usedMb = (totalMemory - freeMemory) / 1024.0;
            return $"{usedMb / totalMb * 100}%";
        }

        public static string GetNetSpeedStatus()
        {
            // 获取网络接口上传速率
            PerformanceCounter downloadCounter = new PerformanceCounter("Network Interface", "Bytes Received/sec", "Intel[R] Wi-Fi 6 AX201 160MHz");
            PerformanceCounter uploadCounter = new PerformanceCounter("Network Interface", "Bytes Sent/sec", "Intel[R] Wi-Fi 6 AX201 160MHz");

            var downloadUnit = 0;
            var uploadUnit = 0;
            var downloadSpeed = downloadCounter.NextValue();
            var uploadSpeed = uploadCounter.NextValue();
            System.Threading.Thread.Sleep(100);
            downloadSpeed = downloadCounter.NextValue();
            uploadSpeed = uploadCounter.NextValue();
            while (downloadSpeed > 1024)
            {
                downloadSpeed /= 1024;
                downloadUnit++;
            }
            while (uploadSpeed > 1024)
            {
                uploadSpeed /= 1024;
                uploadUnit++;
            }
            return $"↑{uploadSpeed} {uploadUnit switch { 0 => "b/s", 1 => "Kb/s", 2 => "Mb/s", 3 => "Gb/s", _ => "????" }} | ↓{downloadSpeed} {downloadUnit switch { 0 => "b/s", 1 => "Kb/s", 2 => "Mb/s", 3 => "Gb/s", _ => "????" }}";
        }

        public static string GetPhysicalDiskStatus()
        {
            // 获取磁盘总活动时间百分比
            PerformanceCounter diskCounter = new PerformanceCounter("PhysicalDisk", "% Disk Time", "_Total");
            var usage = diskCounter.NextValue();
            System.Threading.Thread.Sleep(100);
            usage = diskCounter.NextValue();
            // 读取当前磁盘使用率
            return $"{usage}%";
        }

        public static string GetGPUStatus()
        {
            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PerfFormattedData_GPUPerformanceCounters_GPUAdapter");

            foreach (var obj in searcher.Get())
            {
                Console.WriteLine("GPU Utilization: {0} %", obj["UtilizationPercentage"]);
                Console.WriteLine("GPU Memory Usage: {0} %", obj["FBUsagePercentage"]);
            }
            return "";
        }

        #endregion

        #region 异步获取实时信息
        public static async Task<string> GetCPUStatusAsync()
        {
            var counter = new PerformanceCounter(categoryName: "Processor", "% Processor Time", "_Total");
            var cpuUsage = counter.NextValue();
            await Task.Delay(500);
            cpuUsage = counter.NextValue();
            return $"{cpuUsage:0.00}%";
        }

        public static Task<string> GetMemoryStatusAsync()
        {
            var searcher = new ManagementObjectSearcher("SELECT TotalVisibleMemorySize, FreePhysicalMemory FROM Win32_OperatingSystem");
            ulong totalMemory = 0;
            ulong freeMemory = 0;
            foreach (var item in searcher.Get())
            {
                totalMemory += (ulong)item["TotalVisibleMemorySize"];
                freeMemory += (ulong)item["FreePhysicalMemory"];
            }
            double totalMb = totalMemory / 1024.0;
            double usedMb = (totalMemory - freeMemory) / 1024.0;
            return Task.FromResult($"{usedMb / totalMb * 100:0.00}%");
        }

        public static async Task<string> GetNetDownloadSpeedStatusAsync(string cardName)
        {
            try
            {
                // 获取网络接口上传速率
                var downloadCounter = new PerformanceCounter("Network Adapter", "Bytes Received/sec", cardName.Replace("(", "[").Replace(")", "]"));


                var downloadUnit = 0;
                var downloadSpeed = downloadCounter.NextValue();
                await Task.Delay(1000);
                downloadSpeed = downloadCounter.NextValue();
                while (downloadSpeed > 1024)
                {
                    downloadSpeed /= 1024;
                    downloadUnit++;
                }
                return $"{downloadSpeed:0.00} {downloadUnit switch { 0 => "B/s", 1 => "KB/s", 2 => "MB/s", 3 => "GB/s", _ => "??" }}";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return "???";
            }

        }

        public static async Task<string> GetNetUploadSpeedStatusAsync(string cardName)
        {
            var uploadCounter = new PerformanceCounter("Network Adapter", "Bytes Sent/sec", cardName.Replace("(", "[").Replace(")", "]"));
            var uploadUnit = 0;
            var uploadSpeed = uploadCounter.NextValue();
            await Task.Delay(1000);
            uploadSpeed = uploadCounter.NextValue();
            while (uploadSpeed > 1024)
            {
                uploadSpeed /= 1024;
                uploadUnit++;
            }
            return $"{uploadSpeed:0.00} {uploadUnit switch { 0 => "B/s", 1 => "KB/s", 2 => "MB/s", 3 => "GB/s", _ => "??" }}";
        }

        public static async Task<string> GetPhysicalDiskStatusAsync()
        {
            // 获取磁盘总活动时间百分比
            PerformanceCounter diskCounter = new PerformanceCounter("PhysicalDisk", "% Disk Time", "_Total");
            var usage = diskCounter.NextValue();
            await Task.Delay(1000);
            usage = diskCounter.NextValue();
            // 读取当前磁盘使用率
            return $"{usage:0.00}%";
        }
        #endregion
    }
}
