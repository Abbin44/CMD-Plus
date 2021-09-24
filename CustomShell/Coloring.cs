using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomShell
{
    class Coloring
    {
        MainController main = MainController.controller;
        public Coloring()
        {

        }

        public void FindAndColor(string[] strings, Color color)
        {
            string word;

            for (int i = 0; i < strings.Length; ++i)
            {
                int s_start = main.wandTextBox.SelectionStart, startIndex = 0, index;
                word = strings[i];
                while ((index = main.wandTextBox.Text.IndexOf(word, startIndex)) != -1)
                {
                    main.wandTextBox.Select(index, word.Length);
                    main.wandTextBox.SelectionColor = color;

                    startIndex = index + word.Length;
                }

                main.wandTextBox.SelectionStart = s_start;
                main.wandTextBox.SelectionLength = 0;
            }
        }
    }
}
