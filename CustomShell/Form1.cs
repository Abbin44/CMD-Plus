using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace CustomShell
{
    public partial class Form1 : Form
    {
        string currentDir = @"C:/";
        List<string> history = new List<string>();
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
            outputBox.ScrollToCaret();
            inputBox.Text = string.Concat(Environment.UserName + "@" + currentDir);
            inputBox.SelectionStart = inputBox.Text.Length;
            this.ActiveControl = inputBox;
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
            history.Add(command);

            inputBox.Text = inputPrefix(); //Clear input area
            inputBox.SelectionStart = inputBox.Text.Length;//Set cursor to right position
        }

        public void AddTextToConsole(string text)
        {
            outputBox.AppendText("\n" + text);
        }

        #region Commands
        public void ChangeDirectory(string[] tokens)
        {
            if (tokens.Length == 1) //Go to root
            {
                currentDir = @"C:\";
                AddCommandToConsole(tokens);
            }
            else if (tokens[1] == "..")//Go back one folder
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

        public void MakeDirectory(string[] tokens)
        {
            string dir = string.Empty;

            if (!tokens[1].Contains(@":\"))
                dir = string.Concat(currentDir, @"\", tokens[1]);
            else
                dir = tokens[1];

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
                AddCommandToConsole(tokens);
            }
            else
                AddTextToConsole("Directory already exists.");
        }

        public void MakeFile(string[] tokens)
        {
            string path = string.Empty;

            if (!tokens[1].Contains(@":\"))
                path = string.Concat(currentDir, @"\", tokens[1]);
            else 
                path = tokens[1];

            if (!File.Exists(path))
            {
                File.Create(path);
                AddCommandToConsole(tokens);
            }
            else
                AddTextToConsole("File already exists.");
        }
        #endregion
        //When command is entered
        private void inputBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            int historyIndex = 0;
            string input = string.Empty;
            string[] tokens;

            if (e.KeyCode == Keys.Enter)
            {
                input = inputBox.Text;
                string command = input.Remove(0, (Environment.UserName + "@" + currentDir).Length);
                tokens = command.Split(' ');

                if (tokens[0] == "cd")
                    ChangeDirectory(tokens);
                else if (tokens[0] == "mkdir")
                    MakeDirectory(tokens);
                else if (tokens[0] == "mkfile")
                    MakeFile(tokens);
                else
                    return;
            }

            if(e.KeyCode == Keys.Up)
            {
                if (historyIndex >= 0 && historyIndex < history.Count)
                {
                    inputBox.Text = string.Concat(inputPrefix(), " ", history[historyIndex]); //Clear input area
                    inputBox.SelectionStart = inputBox.Text.Length;//Set cursor to right position
                    ++historyIndex;
                }
            }

            if (e.KeyCode == Keys.Down)
            {
                if(historyIndex > 0 && historyIndex <= history.Count)
                {
                    inputBox.Text = string.Concat(inputPrefix(), " ", history[historyIndex]); //Clear input area
                    inputBox.SelectionStart = inputBox.Text.Length;//Set cursor to right position
                    --historyIndex;
                }
            }
        }
    }
}
