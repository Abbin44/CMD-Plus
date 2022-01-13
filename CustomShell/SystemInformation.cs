using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Management;
using System.Windows.Forms;

namespace CustomShell
{
    class SystemInformation
    {
        MainController main = MainController.controller;
        ManagementObjectSearcher searcher;
        Coloring color = new Coloring();
        // The ascii art SHOULD be a specific size such as 32x32 or close to that
        public SystemInformation()
        {
            CompileInformation();
            main.outputBox.SelectionStart = main.outputBox.TextLength;
            main.outputBox.ScrollToCaret();
        }

        private void CompileInformation()
        {
            List<string> data = new List<string>();
            
            DrawIcon();
            int lineCounter = main.outputBox.Lines.Length - 18; //Lastline - 18
            int index = 0;
            string line = string.Empty;
            Color clr;

            data.Add(GetOS());
            data.Add(GetCPU());
            data.Add(GetGPU());
            data.Add(GetRAM());
            data.Add(GetResolution());
            data.Add(GetSystemUpTimeInfo());
            data.Add(GetMobo());

            for (int i = 0; i < data.Count; ++i)
            {
                index = main.outputBox.GetFirstCharIndexFromLine(lineCounter + 1) - 1;
                line = "    |" + data[i];
                main.outputBox.Text = main.outputBox.Text.Insert(index, line);
                ++lineCounter;
            }
            //This can be commented out to remove coloring, or switched to FindAndColorArray to color all lines with the same color
            #region Coloring 
            //string[] dataLines = data.ToArray(); //Datalines contains the same data as the data list, it needs to be copied over so that all the strings can be colored after everyting has been printed
            //for (int i = 0; i < dataLines.Length; i++)
            //{
            //    clr = color.GetRandomColor();
            //    color.FindAndColorString(dataLines[i], clr, main.outputBox);
            //}
            #endregion
        }

        private string GetResolution()
        {
            string screenWidth = Screen.PrimaryScreen.Bounds.Width.ToString();
            string screenHeight = Screen.PrimaryScreen.Bounds.Height.ToString();

            return string.Concat("Resolution: " ,screenWidth, "x", screenHeight, "px");
        }

        private string GetSystemUpTimeInfo()
        {
            try
            {
                Int64 uptimeInSeconds = Stopwatch.GetTimestamp() / Stopwatch.Frequency;
                TimeSpan time = TimeSpan.FromSeconds(uptimeInSeconds);
                return string.Concat("Uptime: ", time.Hours, "h ", time.Minutes, "m ", time.Seconds, "s ");
            }
            catch (Exception)
            {
                string upTime = "Uptime: 0h 0m 0s";
                return upTime;
            }
        }

        private string GetMobo()
        {
            string company = string.Empty;
            string model = string.Empty;
            searcher = new ManagementObjectSearcher("select Manufacturer, Product from Win32_BaseBoard");
            foreach (ManagementObject share in searcher.Get())
            {
                foreach (PropertyData PC in share.Properties)
                {
                    if (PC.Name == "Manufacturer")
                    {
                        company = PC.Value.ToString();
                        company = company.Trim();
                    }
                    else if (PC.Name == "Product")
                    {
                        model = PC.Value.ToString();
                        model = model.Trim();
                    }
                }
            }
            return string.Concat("Motherboard: ", company, model);
        }

        private string GetOS()
        {
            string name = string.Empty;
            string arch = string.Empty;
            string version = string.Empty;
            searcher = new ManagementObjectSearcher("select Caption, OSArchitecture, Version from Win32_OperatingSystem");
            foreach (ManagementObject share in searcher.Get())
            {
                foreach (PropertyData PC in share.Properties)
                {
                    if (PC.Name == "Caption")
                    {
                        name = PC.Value.ToString();
                        name = name.Trim();
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
            return string.Concat("OS: ", name, ", ", arch, " v", version);
        }

        private string GetCPU()
        {
            string name = string.Empty;
            string speed = string.Empty;
            searcher = new ManagementObjectSearcher("select Name, MaxClockSpeed from Win32_Processor");
            foreach (ManagementObject share in searcher.Get())
            {
                foreach (PropertyData PC in share.Properties)
                {
                    if (PC.Name == "Name")
                    {
                        name = PC.Value.ToString();
                        name = name.Trim();
                    }
                    else if (PC.Name == "MaxClockSpeed")
                    {
                        speed = PC.Value.ToString();
                        speed = speed.Trim();
                        speed = speed.Insert(1, ",");
                    }
                }
            }
            
            return string.Concat("CPU: ", name, " ", "@ ", speed, "GHz");
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
            return string.Concat("GPU: ", gpu);
        }

        private string GetRAM()
        {
            string maxCapacity= string.Empty;
            string currentUse= string.Empty;
            searcher = new ManagementObjectSearcher("select TotalVisibleMemorySize, FreePhysicalMemory from Win32_OperatingSystem");
            foreach (ManagementObject share in searcher.Get())
            {
                foreach (PropertyData PC in share.Properties)
                {
                    if (PC.Name == "TotalVisibleMemorySize")
                    {
                        maxCapacity = PC.Value.ToString();
                        maxCapacity = maxCapacity.Trim();
                        maxCapacity = main.FormatBytes(Convert.ToInt64(maxCapacity) * 1000);
                    }
                    else if (PC.Name == "FreePhysicalMemory")
                    {
                        currentUse = PC.Value.ToString();
                        currentUse = currentUse.Trim();
                        currentUse = main.FormatBytes(Convert.ToInt64(currentUse) * 1000);
                    }
                }
            }
            return string.Concat("RAM: ", currentUse, "/", maxCapacity);
        }

        private void DrawIcon()
        {
            string icon = 
                $@"          :    :@@@@:    :          
       :*@@%.  -@@@@=   #@@*-       
       .%@@@@@@@@@@@@@@@@@@@:       
   :.   *@@@@@@@@@@@@@@@@@@*.  .-   
  +@@%*@@@@@#=:.    .:=#@@@@@*%@@+  
 :%@@@@@@@*.            .*@@@@@@@%- 
   .%@@@@:                :@@@@@.   
...:@@@@-                  -@@@@-...
@@@@@@@@                    @@@@@@@@
@@@@@@@@                    @@@@@@@@
...-@@@@-                  -@@@@-...
    %@@@@:                :@@@@@.   
 :%@@@@@@@*.            .+@@@@@@@%- 
  +@@@#@@@@@#=:      :=#@@@@@#@@@*  
   -:  .*@@@@@@@@@@@@@@@@@@*.  :-   
       .%@@@@@@@@@@@@@@@@@@@:       
       -*@@%.  -@@@@=. .%@@*-       
          :    :@@@@:    :          " + "\n\n";

            MainController.controller.outputBox.AppendText(icon);
        }
    }
}
