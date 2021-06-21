using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FluentFTP;

namespace CustomShell
{
    class FTPController
    {
        FtpClient client;
        public FTPController(string ip)
        {
            client = new FtpClient(ip);
        }

        public void StartFTPSConnection(string user, string pass)
        {
            try
            {
                client.Credentials = new NetworkCredential(user, pass);
                client.Connect();
                MainController.controller.AddTextToConsole("Successfully connected to the server...");
                MainController.controller.inputBox.Text = MainController.controller.InputPrefix();
                MainController.controller.inputBox.SelectionStart = MainController.controller.inputBox.Text.Length;
            }
            catch (Exception)
            {
                MainController.controller.AddTextToConsole("An error occured, please try again...");
                return;
            }
        }
        public void StartFTPConnection()
        {
            try
            {
                client.Connect();
                MainController.controller.AddTextToConsole("Successfully connected to the server...");
                MainController.controller.inputBox.Text = MainController.controller.InputPrefix();
                MainController.controller.inputBox.SelectionStart = MainController.controller.inputBox.Text.Length;

            }
            catch (Exception)
            {
                MainController.controller.AddTextToConsole("An error occured, did you enter the right login credentials?");
                return;
            }
        }
        public void GetDir(string dir)
        {
            foreach (FtpListItem item in client.GetListing(dir))
            {

                // if this is a file
                if (item.Type == FtpFileSystemObjectType.File)
                {
                    // get the file size
                    long size = client.GetFileSize(item.FullName);

                    // calculate a hash for the file on the server side (default algorithm)
                    FtpHash hash = client.GetChecksum(item.FullName);
                }

                // get modified date/time of the file or folder
                DateTime time = client.GetModifiedTime(item.FullName);
                MainController.controller.AddFTPItemToConsole(item);
            }

            MainController.controller.inputBox.Text = MainController.controller.InputPrefix();
            MainController.controller.inputBox.SelectionStart = MainController.controller.inputBox.Text.Length;
        }

        public void UploadFile(string from, string to)
        {
            try
            {
                client.UploadFile(from, to);
                MainController.controller.AddTextToConsole("Successfully uploaded file...");
                MainController.controller.inputBox.Text = MainController.controller.InputPrefix();
                MainController.controller.inputBox.SelectionStart = MainController.controller.inputBox.Text.Length;
            }
            catch (Exception)
            {
                MainController.controller.AddTextToConsole("Could not upload file...");
                return;
            }
        }

        public void DownloadFile(string to, string from)
        {
            try
            {
                client.DownloadFile(to, from);
                MainController.controller.AddTextToConsole("Successfully downloaded file...");
                MainController.controller.inputBox.Text = MainController.controller.InputPrefix();
                MainController.controller.inputBox.SelectionStart = MainController.controller.inputBox.Text.Length;
            }
            catch (Exception)
            {
                MainController.controller.AddTextToConsole("Could not download file...");
                return;
            }
        }

        public void UploadDirectory(string from, string to)
        {
            try
            {
                client.UploadDirectory(from, to);
                MainController.controller.AddTextToConsole("Successfully uploaded directory...");
                MainController.controller.inputBox.Text = MainController.controller.InputPrefix();
                MainController.controller.inputBox.SelectionStart = MainController.controller.inputBox.Text.Length;
            }
            catch (Exception)
            {
                MainController.controller.AddTextToConsole("Could not upload directory...");
                return;
            }
        }

        public void DownloadDirectory(string to, string from)
        {
            try
            {
                client.DownloadDirectory(to, from);
                MainController.controller.AddTextToConsole("Successfully downloaded directory...");
                MainController.controller.inputBox.Text = MainController.controller.InputPrefix();
                MainController.controller.inputBox.SelectionStart = MainController.controller.inputBox.Text.Length;
            }
            catch (Exception)
            {
                MainController.controller.AddTextToConsole("Could not download directory...");
                return;
            }
        }

        public void Terminate()
        {
            try
            {
                client.Disconnect();
                MainController.controller.AddTextToConsole("Successfully terminated connection to the server...");
                MainController.controller.inputBox.Text = MainController.controller.InputPrefix();
                MainController.controller.inputBox.SelectionStart = MainController.controller.inputBox.Text.Length;
            }
            catch (Exception)
            {
                MainController.controller.AddTextToConsole("Could not terminate connection, please try again...");
                return;
            }

        }
    }
}
