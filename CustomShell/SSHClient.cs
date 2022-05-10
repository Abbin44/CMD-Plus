using System;
using System.Drawing;
using Renci.SshNet;

namespace CustomShell
{
    class SSHClient
    {
        public SshClient client;
        MainController main = MainController.controller;
        string currentwd;
        public string username;
        public string host;
        public SSHClient()
        {
            
        }
        public void SetSSHInputPrefix(string user, string clientIp)
        {
            GetCurrentWD();
            string text = string.Concat(user, "@", clientIp, " ~ ", @currentwd, " § ");
            main.inputBox.Text = text;
            LockInputPrefix(text);
        }
        public void LockInputPrefix(string text)
        {
            main.inputBox.Select(0, text.Length);
            main.inputBox.SelectionProtected = true;
            main.inputBox.SelectionStart = main.inputBox.Text.Length;
        }

        private void AddTextToConsole(string text)
        {
            main.sshTextBox.AppendText(text);
        }

        private void InitSSHInterface()
        {
            main.sshTextBox.Visible = true;
            main.outputBox.Visible = false;
        }

        private void CloseSSHInterface()
        {
            main.sshTextBox.Visible = false;
            main.outputBox.Visible = true;
        }

        private void GetCurrentWD()
        {
            SshCommand result = client.RunCommand("pwd");
            currentwd = result.Result.Trim('\n');
        }

        public void EstablishConnection(string ip, string usrname, string password)
        {
            try
            {
                ConnectionInfo connInfo = new ConnectionInfo(ip, usrname, new PasswordAuthenticationMethod(usrname, password), new PrivateKeyAuthenticationMethod("rsa.key"));
                client = new SshClient(connInfo);
                client.Connect();

                username = usrname;
                host = ip;
                InitSSHInterface();
                main.sshMode = true;
                AddTextToConsole("Successfully connected to the host...\n");
                main.Text = string.Concat("ssh ", usrname, "@", ip);
                SetSSHInputPrefix(usrname, ip);
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
                AddTextToConsole(result.Result);
                main.sshTextBox.Select(main.sshTextBox.Text.Length - result.Result.Length - 1, main.sshTextBox.Text.Length);
                main.sshTextBox.SelectionColor = Color.Red;
                main.sshTextBox.SelectionStart = main.sshTextBox.Text.Length;

                SetSSHInputPrefix(username, host);
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
                CloseSSHInterface();
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
