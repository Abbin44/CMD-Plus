using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace CustomShell
{
    public partial class Form1 : Form
    {
        string currentDir = @"C:/";
        public string inputPrefix()
        {
            string text = string.Concat(Environment.UserName + "@" + currentDir);
            return text;
        }

        public Form1()
        {
            InitializeComponent();
            InitConsole();
        }

        public void InitConsole()
        {
            //consoleBox.Select(0, consoleBox.Text.Length);
            outputBox.ScrollToCaret();
            inputBox.Text = string.Concat(Environment.UserName + "@" + currentDir);
            inputBox.Focus();

        }

        public void AddCommandToConsole(string[] tokens)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < tokens.Length; ++i)
            {
                 sb.Append(tokens[i] + " ");
            }
            string command = sb.ToString();
            outputBox.AppendText("\n" + command);

            inputBox.Text = inputPrefix(); //Clear input area
        }

        public void ChangeDirectory(string[] tokens)
        {
            if (tokens.Length == 1) //Go to root
            {
                currentDir = @"C:\";
                AddCommandToConsole(tokens);
            }
            else if(tokens[1] == "..")//Go back one folder
            {
                string dir = currentDir;
                int index = dir.LastIndexOf(@"\");
                if (index > 0)
                    dir = dir.Substring(0, index);

                currentDir = dir;
                AddCommandToConsole(tokens);
            }
            else if (Directory.Exists(tokens[1]))
            {
                currentDir = tokens[1];
                AddCommandToConsole(tokens);
            }
        }

        //When command is entered
        private void inputBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            string input = string.Empty;
            string[] tokens;

            if (e.KeyCode == Keys.Enter)
            {
                input = inputBox.Text;
                string command = input.Remove(0, (Environment.UserName + "@" + currentDir).Length);
                tokens = command.Split(' ');

                if (tokens[0] == "cd")
                    ChangeDirectory(tokens);
                else
                    return;
            }
        }
    }
}
