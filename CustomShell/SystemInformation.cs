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
        Coloring color;
        // The ascii art SHOULD be a specific size such as 32x32 or close to that
        public SystemInformation()
        {
            GetIcon();
        }

        private void CompileInformation()
        {
            string os = string.Empty;
            string cpu = string.Empty;
            string gpu = string.Empty;
            string ram = string.Empty;
            string uptime = string.Empty;

            GetOS();
            GetCPU();
            GetGPU();
            GetRAM();
            GetIcon();
            GetUptime();
            GetMobo();
        }

        private string GetMobo()
        {
            string mobo = string.Empty;

            return mobo;
        }

        private string GetOS()
        {
            string os = string.Empty;
            string arch = string.Empty;
            string version = string.Empty;

            searcher = new ManagementObjectSearcher("select Caption, OSArchitecture, Version from Win32_OperatingSystem");
            foreach (ManagementObject share in searcher.Get())
            {
                foreach (PropertyData PC in share.Properties)
                {
                    if (PC.Name == "Caption")
                    {
                        os = PC.Value.ToString();
                        os = os.Trim();
                    }
                    else if (PC.Name == "OSArchitecture")
                    {
                        arch = PC.Value.ToString();
                        arch = arch.Trim();
                    }
                    else if(PC.Name == "Version")
                    {
                        version = PC.Value.ToString();
                        version = version.Trim();
                    }
                }
            }
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
                        ram = MainController.controller.FormatBytes(Convert.ToInt32(ram));//It is highly unclear what size MaxCapacity returns
                    }
                }
            }
            return ram;
        }
        private string GetUptime()
        {
            string uptime = string.Empty;

            return uptime;
        }

        private void GetIcon()
        {
            //This is too large, needs to be replaced
            string icon =
                $@"________ /\\\\\\\\\_   __/\\\\____________ /\\\\_    __/\\\\\\\\\\\\____    _______________    _______________
                    _____ /\\\////////__   _\/\\\\\\________/\\\\\\_     _\/\\\////////\\\__    _______________    _______________
                     ___ /\\\/ __________   _\/\\\//\\\____/\\\//\\\_     _\/\\\______\//\\\_    ______/\\\_____    ______/\\\_____
                      __ /\\\_____________   _\/\\\\///\\\/\\\/_\/\\\_     _\/\\\_______\/\\\_    _____\/\\\_____    _____\/\\\_____
                       _\/\\\_____________    _\/\\\__\///\\\/___\/\\\_     _\/\\\_______\/\\\_    __/\\\\\\\\\\\_    __/\\\\\\\\\\\_
                        _\//\\\____________    _\/\\\____\///_____\/\\\_     _\/\\\_______\/\\\_    _\/////\\\///__    _\/////\\\///__
                         __\///\\\__________    _\/\\\_____________\/\\\_     _\/\\\_______/\\\__    _____\/\\\_____    _____\/\\\_____
                          ____\////\\\\\\\\\_    _\/\\\_____________\/\\\_     _\/\\\\\\\\\\\\/___    _____\///______    _____\///______
                           _______\/////////__    _\///______________\///__     _\////////////_____    _______________    _______________";

            MainController.controller.outputBox.AppendText(icon);
        }
    }
}
