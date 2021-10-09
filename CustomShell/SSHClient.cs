using System;
using System.Drawing;
using Renci.SshNet;

namespace CustomShell
{
    class SSHClient
    {
        public SshClient client;
        MainController main = MainController.controller;

        public SSHClient()
        {
            
        }

        public void EstablishConnection(string host, string username, string password)
        {
            try
            {
                ConnectionInfo connInfo = new ConnectionInfo(host, username, new PasswordAuthenticationMethod(username, password), new PrivateKeyAuthenticationMethod("rsa.key"));
                client = new SshClient(connInfo);
                client.Connect();
                main.AddTextToConsole("Successfully connected to the host...");
                main.SetInputPrefix();
            }
            catch (Exception)
            {
                main.AddTextToConsole("Could not connect, did you enter the right login credentials?");
                return;            
            }
        }

        public void SendCommand(string command)
        {
            try
            {
                SshCommand result = client.RunCommand(command);
                main.AddCommandToConsole(MainController.controller.tokens);

                //Add a red color for the ssh output
                main.AddTextToConsole(result.Result);
                main.outputBox.Select(main.outputBox.Text.Length - result.Result.Length - 1, main.outputBox.Text.Length);
                main.outputBox.SelectionColor = Color.Red;
                main.outputBox.SelectionStart = main.outputBox.Text.Length;

                main.SetInputPrefix();
            }
            catch (Exception)
            {
                main.AddTextToConsole("Could not execute command...");
                return;
            }
        }

        public void TerminateConnection()
        {
            try
            {
                client.Disconnect();
                main.AddTextToConsole("Successfully terminated the connection...");
                main.SetInputPrefix();
            }
            catch (Exception)
            {
                main.AddTextToConsole("Could not terminate...");
                return;
            }
        }
    }
}
