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
            main.wandTextBox.ForeColor = Color.Aqua;
            try
            {
                string[] lines = File.ReadAllLines(path);
                for (int i = 0; i < lines.Length; i++)
                    AddTextToConsole(lines[i]);

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