using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;

namespace CustomShell
{
    class SSHClient
    {
        public SshClient client;
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
                MainController.controller.AddTextToConsole("Successfully connected to the host...");
                MainController.controller.inputBox.Text = MainController.controller.InputPrefix();
                MainController.controller.inputBox.SelectionStart = MainController.controller.inputBox.Text.Length;
            }
            catch (Exception)
            {
                MainController.controller.AddTextToConsole("Could not connect, did you enter the right login credentials?");
                return;            
            }
        }

        public void SendCommand(string command)
        {
            try
            {
                SshCommand result = client.RunCommand(command);
                MainController.controller.AddCommandToConsole(MainController.controller.tokens);

                //Add a red color for the ssh output
                MainController.controller.AddTextToConsole(result.Result);
                MainController.controller.outputBox.Select(MainController.controller.outputBox.Text.Length - result.Result.Length - 1, MainController.controller.outputBox.Text.Length);
                MainController.controller.outputBox.SelectionColor = Color.Red;
                MainController.controller.outputBox.SelectionStart = MainController.controller.outputBox.Text.Length;

                MainController.controller.inputBox.Text = MainController.controller.InputPrefix();
                MainController.controller.inputBox.SelectionStart = MainController.controller.inputBox.Text.Length;
            }
            catch (Exception)
            {
                MainController.controller.AddTextToConsole("Could not execute command...");
                return;
            }
        }

        public void TerminateConnection()
        {
            try
            {
                client.Disconnect();
                MainController.controller.AddTextToConsole("Successfully terminated the connection...");
                MainController.controller.inputBox.Text = MainController.controller.InputPrefix();
                MainController.controller.inputBox.SelectionStart = MainController.controller.inputBox.Text.Length;
            }
            catch (Exception)
            {
                MainController.controller.AddTextToConsole("Could not terminate...");
                return;
            }
        }
    }
}
