using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JDF;
namespace CustomShell
{
    class SettingsManager
    {
        string filePath = $@"C:\Users\" + Environment.UserName + @"\AppData\Local\CMD++\settings.cfg"; //USE Environment.UserName for releases
        public static DefaultSettings defaultSettings { get; private set; }
        public SettingsManager()
        {
            if (defaultSettings == null)
                defaultSettings = new DefaultSettings();

            LoadSettings();
        }

        void LoadSettings()
        {
            JDFController jdf = new JDFController(filePath);
            JDF.Object[] settings = jdf.ReadOptions();
            /*
           for (int i = 0; i < settings.Count; ++i)
           {
               switch (settings[i][0])//Add new settings here (3 places total)
               {
                   case "fcolor":
                       defaultSettings.defaultFcolor = settings[i][1];
                       break;
                   case "bcolor":
                       defaultSettings.defaultBcolor = settings[i][1];
                       break;
                   default:
                       Console.WriteLine("Error loading settings, something is wrong...");
                       break;
               }
           }
           */
        }
    }
}
