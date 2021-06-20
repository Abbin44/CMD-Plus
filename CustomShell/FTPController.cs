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
                MainController.controller.AddTextToConsole("An error occured, did you enter the right login credentials?");
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
        }

        public void UploadFile(string from, string to)
        {
            client.UploadFile(from, to);
        }

        public void DownloadFile(string from, string to)
        {
            client.DownloadFile(from, to);
        }

        public void UploadDirectory(string from, string to)
        {
            client.UploadDirectory(from, to);
        }

        public void DownloadDirectory(string from, string to)
        {
            client.DownloadDirectory(from, to);
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
