using System;
using System.Diagnostics;

namespace CustomShell
{
    class Processes
    {
        MainController main = MainController.controller;
        public Processes()
        {

        }

        public void ListProcesses()
        {
            Process[] processes = Process.GetProcesses();
            for (int i = 0; i < processes.Length; i++)
                main.AddTextToConsole(processes[i].ProcessName + " : " + processes[i].Id);

            main.SetInputPrefix();
        }

        public void KillProcess(string[] tokens)
        {
            if(tokens.Length != 2)
            {
                main.AddTextToConsole("Invalid command format");
                return;
            }

            bool isNumeric = int.TryParse(tokens[1], out int n);
            if (isNumeric == true)
            {
                int id = Convert.ToInt32(tokens[1]);
                Process.GetProcessById(id).Kill();
                main.AddCommandToConsole(tokens);
            }
            else
                main.AddTextToConsole("Please enter a process id...");
        }
    }
}
