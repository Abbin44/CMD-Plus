using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;

namespace CustomShell
{
    class SystemInformation
    {
        ManagementObjectSearcher searcher;
        /* The ascii art SHOULD be a specific size such as 32x32 or close to that
         * 
         * 
         * 
         * 
         * 
        */
        public SystemInformation()
        {
            GetRAM();
        }

        private string GetOS()
        {
            //searcher = new ManagementObjectSearcher("select * from " + Key);

            string os = string.Empty;

            return os;
        }

        private string GetCPU()
        {
            string cpu = string.Empty;
            string cpuSpeed = string.Empty;

            searcher = new ManagementObjectSearcher("select Name, MaxClockSpeed from Win32_Processor");
            foreach (ManagementObject share in searcher.Get())
            {
                foreach (PropertyData PC in share.Properties)
                {
                    if (PC.Name == "Name")
                    {
                        cpu = PC.Value.ToString();
                        cpu = cpu.Trim();
                    }
                    else if (PC.Name == "MaxClockSpeed")
                    {
                        cpuSpeed = PC.Value.ToString();
                        cpuSpeed = cpuSpeed.Trim();
                        cpuSpeed = cpuSpeed.Insert(1, ",");
                    }
                }
            }

            cpu = string.Concat(cpu, "  ", "@  ", cpuSpeed, "GHz");
            return cpu;
        }

        private string GetGPU()
        {
            string gpu = string.Empty;
            searcher = new ManagementObjectSearcher("select Caption from Win32_VideoController");
            foreach (ManagementObject share in searcher.Get())
            {
                foreach (PropertyData PC in share.Properties)
                {
                    if (PC.Name == "Caption")
                    {
                        gpu = PC.Value.ToString();
                        gpu = gpu.Trim();
                    }
                }
            }

            return gpu;
        }

        private string GetRAM()
        {
            string ram = string.Empty;
            searcher = new ManagementObjectSearcher("select MaxCapacity from Win32_PhysicalMemoryArray");
            foreach (ManagementObject share in searcher.Get())
            {
                foreach (PropertyData PC in share.Properties)
                {
                    if (PC.Name == "MaxCapacity")
                    {
                        ram = PC.Value.ToString();
                        ram = ram.Trim();
                        ram = MainController.controller.FormatBytes(Convert.ToInt32(ram));
                    }
                }
            }
            return ram;
        }
    }
}
