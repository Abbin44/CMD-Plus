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
            for (int i = 0; i < lines.Length; i++)
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
            main.wandTextBox.Visible = true;// Swap text box to be able to 
            main.outputBox.Visible = false; // preseve coloring in previous commands
            main.wandTextBox.ForeColor = Color.LightSteelBlue;
            try
            {
                string[] lines = File.ReadAllLines(path);
                for (int i = 0; i < lines.Length; i++)
                    AddTextToConsole(lines[i]);

                ApplySyntaxHighlight();

                main.wandTextBox.Focus();
                main.wandTextBox.SelectionStart = main.wandTextBox.TextLength;
                main.inputBox.Text = main.InputPrefix(); //Clear input console
                hasFileLoaded = true;
            }
            catch (Exception e)
            {
                main.wandTextBox.Visible = false;
                main.outputBox.Visible = true;
                main.AddTextToConsole("Something went wrong, please check your input");
                return;
            }
        }

        public void ApplySyntaxHighlight()
        {
            string[] types = new string[] {"int ", "integer ", "float ", "single ", "double ", "decimal ", "bool ", "boolean ", "string "};
            string[] operators = new string[] {"!", "=", ">", "<", "|"};
            string[] statements = new string[] {"if", "else if", "elif", "if else", "else", "true", "false", "try", "catch", "finally", "public", "private", "protected", "static", "using", "import", "void", "while", "for", "return", "continue", "break"};

            string word;


            for (int i = 0; i < types.Length; i++)
            {
                int s_start = main.wandTextBox.SelectionStart, startIndex = 0, index;
                word = types[i];
                while ((index = main.wandTextBox.Text.IndexOf(word, startIndex)) != -1)
                {
                    main.wandTextBox.Select(index, word.Length);
                    main.wandTextBox.SelectionColor = Color.Blue;

                    startIndex = index + word.Length;
                }

                main.wandTextBox.SelectionStart = s_start;
                main.wandTextBox.SelectionLength = 0;
            }

            for (int i = 0; i < operators.Length; i++)
            {
                int s_start = main.wandTextBox.SelectionStart, startIndex = 0, index;
                word = operators[i];
                while ((index = main.wandTextBox.Text.IndexOf(word, startIndex)) != -1)
                {
                    main.wandTextBox.Select(index, word.Length);
                    main.wandTextBox.SelectionColor = Color.Red;

                    startIndex = index + word.Length;
                }

                main.wandTextBox.SelectionStart = s_start;
                main.wandTextBox.SelectionLength = 0;
            }

            for (int i = 0; i < statements.Length; i++)
            {
                int s_start = main.wandTextBox.SelectionStart, startIndex = 0, index;
                word = statements[i];
                while ((index = main.wandTextBox.Text.IndexOf(word, startIndex)) != -1)
                {
                    main.wandTextBox.Select(index, word.Length);
                    main.wandTextBox.SelectionColor = Color.Purple;

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
                        main.AddTextToConsole("Cannot access a directory.Please run shell as admin.");
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
        }
    }
}