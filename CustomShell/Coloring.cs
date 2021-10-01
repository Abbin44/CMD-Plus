using System;
using System.Drawing;
using System.Windows.Forms;

namespace CustomShell
{
    class Coloring
    {
        MainController main = MainController.controller;
        public Coloring()
        {

        }

        public void FindAndColorString(string word, Color color, RichTextBox textBox)
        {
            int s_start = textBox.SelectionStart;
            int startIndex = 0;
            int index;
            while ((index = textBox.Text.IndexOf(word, startIndex)) != -1)
            {
                textBox.Select(index, word.Length);
                textBox.SelectionColor = color;
            
                startIndex = index + word.Length;
            }
            
            textBox.SelectionStart = s_start;
            textBox.SelectionLength = 0;
        }

        public void FindAndColorArray(string[] strings, Color color, RichTextBox textBox)
        {
            string word;

            for (int i = 0; i < strings.Length; i++)
            {
                int s_start = textBox.SelectionStart;
                int startIndex = 0;
                int index;
                word = strings[i];
                while ((index = textBox.Text.IndexOf(word, startIndex)) != -1)
                {
                    textBox.Select(index, word.Length);
                    textBox.SelectionColor = color;

                    startIndex = index + word.Length;
                }

                textBox.SelectionStart = s_start;
                textBox.SelectionLength = 0;
            }
        }

        public void ColorForeground(string code)
        {
            if (code == "RED" || code == "01" || code == "1")
            {
                main.outputBox.ForeColor = Color.Red;
                main.inputBox.ForeColor = Color.Red;
            }
            else if (code == "GREEN" || code == "02" || code == "2")
            {
                main.outputBox.ForeColor = Color.Green;
                main.inputBox.ForeColor = Color.Green;
            }
            else if (code == "YELLOW" || code == "03" || code == "3")
            {
                main.outputBox.ForeColor = Color.Yellow;
                main.inputBox.ForeColor = Color.Yellow;
            }
            else if (code == "ORANGE" || code == "04" || code == "4")
            {
                main.outputBox.ForeColor = Color.Orange;
                main.inputBox.ForeColor = Color.Orange;
            }
            else if (code == "BLUE" || code == "05" || code == "5")
            {
                main.outputBox.ForeColor = Color.Blue;
                main.inputBox.ForeColor = Color.Blue;
            }
            else if (code == "WHITE" || code == "06" || code == "6")
            {
                main.outputBox.ForeColor = Color.White;
                main.inputBox.ForeColor = Color.White;
            }
            else if (code == "TURQUOISE" || code == "07" || code == "7")
            {
                main.outputBox.ForeColor = Color.Turquoise;
                main.inputBox.ForeColor = Color.Turquoise;
            }
            else if (code == "BLACK" || code == "08" || code == "8")
            {
                main.outputBox.ForeColor = Color.Black;
                main.inputBox.ForeColor = Color.Black;
            }
        }

        public void ColorBackground(string code)
        {
            if (code == "RED" || code == "01" || code == "1")
            {
                main.outputBox.BackColor = Color.Red;
                main.inputBox.BackColor = Color.Red;
            }
            else if (code == "GREEN" || code == "02" || code == "2")
            {
                main.outputBox.BackColor = Color.Green;
                main.inputBox.BackColor = Color.Green;
            }
            else if (code == "YELLOW" || code == "03" || code == "3")
            {
                main.outputBox.BackColor = Color.Yellow;
                main.inputBox.BackColor = Color.Yellow;
            }
            else if (code == "ORANGE" || code == "04" || code == "4")
            {
                main.outputBox.BackColor = Color.Orange;
                main.inputBox.BackColor = Color.Orange;
            }
            else if (code == "BLUE" || code == "05" || code == "5")
            {
                main.outputBox.BackColor = Color.Blue;
                main.inputBox.BackColor = Color.Blue;
            }
            else if (code == "WHITE" || code == "06" || code == "6")
            {
                main.outputBox.BackColor = Color.White;
                main.inputBox.BackColor = Color.White;
            }
            else if (code == "TURQUOISE" || code == "07" || code == "7")
            {
                main.outputBox.BackColor = Color.Turquoise;
                main.inputBox.BackColor = Color.Turquoise;
            }
            else if (code == "BLACK" || code == "08" || code == "8")
            {
                main.outputBox.BackColor = Color.Black;
                main.inputBox.BackColor = Color.Black;
            }
        }

        public Color GetRandomColor()
        {
            Random randomGen = new Random();
            KnownColor[] names = (KnownColor[])Enum.GetValues(typeof(KnownColor));
            KnownColor randomColorName = names[randomGen.Next(names.Length)];
            Color randomColor = Color.FromKnownColor(randomColorName);
            return randomColor;
        }
    }
}
