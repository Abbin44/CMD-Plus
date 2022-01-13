using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomShell
{
    internal class ProgressBar
    {
        MainController main = MainController.controller;
        public ProgressBar()
        {

        }

        public async Task LoadingBar()
        {
            //Chars for loading bar
            //. o O @ *
            string[] chars = new string[] { ".", "o", "O", "@", "*" };

            for (int i = 0; i < chars.Length; ++i)
            {
                if (main.outputBox.Lines[main.outputBox.Lines.Length].StartsWith("Loading: "))
                    main.ReplaceLastLineConsole("Loading: " + chars[i]);
                else
                    main.AddTextToConsole("Loading: " + chars[0]);
            }
        }

    }
}
