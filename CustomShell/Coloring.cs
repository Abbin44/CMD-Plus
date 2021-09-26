using System;
using System.Drawing;
using System.Windows.Forms;

namespace CustomShell
{
    class Coloring
    {
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
