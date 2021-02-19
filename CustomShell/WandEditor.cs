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

        string oldHistory;
        string path;
        public bool hasFileLoaded = false;
        
        public void PeekFile(string[] tokens)
        {
            if (tokens.Length != 2)
            {
                main.AddTextToConsole("Invalid command format");
                return;
            }
            main.AddCommandToConsole(tokens);
            path = main.CheckInputType(tokens);
            main.outputBox.ForeColor = Color.Aqua;

            string[] lines = File.ReadAllLines(path);
            for (int i = 0; i < lines.Length; i++)
                main.AddTextToConsole(lines[i]);
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
            oldHistory = main.outputBox.Text;
            main.outputBox.Clear();
            main.outputBox.ForeColor = Color.Aqua;
            try
            {
                string[] lines = File.ReadAllLines(path);
                for (int i = 0; i < lines.Length; i++)
                    main.AddTextToConsole(lines[i]);

                main.outputBox.ReadOnly = false;
                main.outputBox.Focus();
                main.outputBox.SelectionStart = main.outputBox.TextLength;
                main.inputBox.Text = main.InputPrefix(); //Clear input console
                hasFileLoaded = true;
            }
            catch (Exception e)
            {
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
                    string text = main.outputBox.Text;
                    File.WriteAllText(path, text);
                    hasFileLoaded = false;
                    Exit();
                }
                catch (Exception e)
                {
                    if(e.InnerException is UnauthorizedAccessException)
                    {
                        main.AddTextToConsole("Cannot access a directory.Please run shell as admin.");
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
            main.outputBox.Clear();
            main.outputBox.ForeColor = Color.Fuchsia;
            main.outputBox.Text = oldHistory;
            main.outputBox.ReadOnly = true;
            main.inputBox.Focus();
            main.inputBox.SelectionStart = main.inputBox.Text.Length;
        }
    }
}