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
        Coloring coloring = new Coloring();
        MainController main = MainController.controller;
        private JDF.Object[] settings;
        public JDFController jdf;

        public SettingsManager()
        {
            if (defaultSettings == null)
                defaultSettings = new DefaultSettings();

            jdf = new JDFController(filePath);

            LoadSettings();
        }

        void LoadSettings()
        {
            settings = jdf.ReadOptions();
            try
            {
                for (int i = 0; i < settings.Length; ++i)
                {
                    string currentObj = settings[i].name;
                    if (currentObj == "coloring")
                    {
                        settings[i].parameters.TryGetValue("fcolor", out string fColor);
                        settings[i].parameters.TryGetValue("bcolor", out string bColor);
                        coloring.ColorForeground(fColor);
                        coloring.ColorBackground(bColor);
                    }
                }
            }
            catch (Exception)
            {
                //Could potentially be added to a log file
                return;
            }
        }

        public void SetSetting(string name, string param, string value)
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            keyValuePairs.Add(param, value);
            if (!jdf.ObjNameExists(name)) //If the parent object doesn't exist
                jdf.AddOption(name, keyValuePairs);
            else if (jdf.ObjNameExists(name) && jdf.ParamExistsInObj(name, param)) //If the parent exists and the parameter already exists
                jdf.UpdateParameter(name, param, value);
            else //If the parent object exists but not the parameter
                jdf.AddParameter(name, param, value);
        }

        public void AddSetting(string name, string param, string value)
        {
            if(jdf.ObjNameExists(name))
               jdf.AddParameter(name, param, value);
        }
    }
}
