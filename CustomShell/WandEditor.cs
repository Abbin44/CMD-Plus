using System;
using System.Drawing;
using System.IO;

namespace CustomShell
{
    class WandEditor
    {
        MainController main = MainController.controller;
        public WandEditor()
        {

        }

        string path;
        public bool hasFileLoaded = false;
        public void AddTextToConsole(string text)
        {
            main.wandTextBox.AppendText(text + "\n");
        }

        public void PeekFile(string[] tokens)
        {
            if (tokens.Length != 2)
            {
                main.AddTextToConsole("Invalid command format");
                return;
            }
            main.AddCommandToConsole(tokens);
            path = main.CheckInputType(tokens);

            string[] lines = File.ReadAllLines(path);
            for (int i = 0; i < lines.Length; ++i)
            {
                main.AddTextToConsole(lines[i]);
                main.outputBox.Select(main.outputBox.Text.Length - lines[i].Length - 1, main.outputBox.Text.Length);
                main.outputBox.SelectionColor = Color.Aqua;
                main.outputBox.SelectionStart = main.outputBox.Text.Length;
            }
        }

        public void LoadFile(string[] tokens)
        {
            if(tokens.Length != 2)
            {
                main.AddTextToConsole("Invalid command format");
                return;
            }

            path = main.CheckInputType(tokens);
            main.AddCommandToConsole(tokens);

            main.wandTextBox.Clear();
            main.wandTextBox.Visible = true;// Swap text box to be able to preseve coloring in previous commands
            main.outputBox.Visible = false;
            main.wandTextBox.ForeColor = Color.LightSteelBlue;
            try
            {
                string[] lines = File.ReadAllLines(path);
                for (int i = 0; i < lines.Length; ++i)
                    AddTextToConsole(lines[i]);

                ApplySyntaxHighlight();
                main.Text = path; //Display filepath in title text

                main.wandTextBox.Focus();
                main.wandTextBox.SelectionStart = main.wandTextBox.TextLength;
                main.inputBox.Text = main.InputPrefix(); //Clear input console
                hasFileLoaded = true;
            }
            catch (Exception)
            {
                main.wandTextBox.Visible = false;
                main.outputBox.Visible = true;
                main.AddTextToConsole("File not valid...");
                return;
            }
        }

        public void RemoveSyntaxHighlight()
        {
            main.wandTextBox.SelectAll();
            main.wandTextBox.SelectionColor = Color.LightSteelBlue;
            main.wandTextBox.SelectionStart = main.wandTextBox.Text.Length;
        }

        public void ApplySyntaxHighlight()
        {
            string[] types = new string[] {"int ", "integer ", "float ", "single ", "double ", "decimal ", "bool ", "boolean ", "string "};
            string[] operators = new string[] {"!", "=", ">", "<", "|", "@", "%", "+", "-", "*", "/", "\\", "?"};
            string[] statements = new string[] {"if", "else if", "elif", "if else", "else", "true", "false", "try", "catch", "finally", "public", "private", "protected", "static", "using", "import", "include", "define", "void", "while", "for", "return", "continue", "break"};
            string[] misc = new string[] {"#", "$", "\"", "'", "region", "endregion"};
            string[] numbers = new string[] {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9"};

            FindAndColor(types, Color.Blue);
            FindAndColor(operators, Color.Red);
            FindAndColor(statements, Color.Purple);
            FindAndColor(misc, Color.Khaki);
            FindAndColor(numbers, Color.DarkViolet);
        }

        private void FindAndColor(string[] strings, Color color)
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

        public void SaveAndExit()
        {
            if (hasFileLoaded == true)
            {
                try
                {
                    string text = main.wandTextBox.Text;
                    File.WriteAllText(path, text);
                    hasFileLoaded = false;
                    Exit();
                }
                catch (Exception e)
                {
                    if(e.InnerException is UnauthorizedAccessException)
                    {
                        main.AddTextToConsole("Cannot access a directory. Please run shell as admin.");
                        main.wandTextBox.Visible = false;
                        main.outputBox.Visible = true;
                        return;
                    }
                }
            }
            else
                return;
        }

        public void Exit()
        {
            hasFileLoaded = false;
            main.wandTextBox.Visible = false;
            main.outputBox.Visible = true;
            main.inputBox.Focus();
            main.inputBox.SelectionStart = main.inputBox.Text.Length;
            main.Text = "CMD++";
        }
    }
}