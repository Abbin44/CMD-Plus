using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace CustomShell
{
    class BatchInterpreter
    {
        MainController main = MainController.controller;
        public BatchInterpreter()
        {

        }

        public void ExecuteCommand(string[] tokens)
        {
            if (tokens.Length < 2)
            {
                main.AddTextToConsole("Invalid command format");
                return;
            }
            
            StringBuilder sb = new StringBuilder();
            for (int i = 1; i < tokens.Length; i++)
            {
                sb.Append(tokens[i]);

                if(!(i == tokens.Length - 1))
                   sb.Append(" ");
            }
            string command = sb.ToString();

            if (command.EndsWith(".bat"))
                command = main.GetPathType(command);

            ProcessStartInfo procStartInfo = new ProcessStartInfo("cmd", "/c " + command)
            {
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process proc = new Process())
            {
                proc.StartInfo = procStartInfo;
                proc.Start(); 

                string output = proc.StandardOutput.ReadToEnd();

                if (string.IsNullOrEmpty(output))
                    output = proc.StandardError.ReadToEnd();

                main.AddCommandToConsole(tokens);
                main.AddTextToConsole(output);

                /*NOTE TO MYSELF IN THE FUTURE
                 * This über-garbage code is used to remove all escape characters from the CMD stdout text.
                 * After the text has been printed in the output console the escape chars will not increse the lenght of the string
                 * Therefor, you need to remove all escape chars and calculate the offset from that, but there is still small differances
                 * in the offsets, the output cannot be colored until this is solved.
                 * 
                output = output.Replace("\a", ""); // Warning
                output = output.Replace("\b", ""); // BACKSPACE
                output = output.Replace("\f", ""); // Form - feed
                output = output.Replace("\n", ""); // Line reverse
                output = output.Replace("\r", ""); // Carriage return
                output = output.Replace("\t", ""); // Horizontal tab
                output = output.Replace("\v", ""); // Vertical tab
                output = output.Replace("\'", ""); // Single quote
                output = output.Replace("\"", ""); // Double quote
                output = output.Replace("\\", ""); // Backslash
                int actualLen = output.Length;
                main.outputBox.Select(main.outputBox.Text.Length - actualLen, actualLen);//<< This is messed up bigtime
                main.outputBox.SelectionColor = Color.Blue;
                main.outputBox.SelectionStart = main.outputBox.Text.Length;
                */
            }
        }
    }
}
