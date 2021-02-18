using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void LoadFile(string[] tokens)
        {
            if(tokens.Length != 2)
            {
                main.AddTextToConsole("Invalid command format");
                return;
            }

            path = main.CheckInputType(tokens);
            oldHistory = main.outputBox.Text;
            main.outputBox.Clear();
            main.outputBox.ForeColor = Color.Aqua;
            try
            {
                string[] lines = File.ReadAllLines(path);
                for (int i = 0; i < lines.Length; i++)
                {
                    main.AddTextToConsole(lines[i]);
                }

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
                string text = main.outputBox.Text;
                File.WriteAllText(path, text);
                hasFileLoaded = false;
                Exit();
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