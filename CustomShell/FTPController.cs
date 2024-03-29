﻿using System;
using System.Net;
using FluentFTP;

namespace CustomShell
{
    class FTPController
    {
        FtpClient client;
        MainController main = MainController.controller;

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
                main.AddTextToConsole("Successfully connected to the server...");
                main.SetInputPrefix();
            }
            catch (Exception)
            {
                main.AddTextToConsole("An error occured, please try again...");
                return;
            }
        }
        public void StartFTPConnection()
        {
            try
            {
                client.Connect();
                main.AddTextToConsole("Successfully connected to the server...");
                main.SetInputPrefix();
            }
            catch (Exception)
            {
                main.AddTextToConsole("An error occured, did you enter the right login credentials?");
                return;
            }
        }
        public void GetDir(string dir)
        {
            dir = main.GetFullPathFromName(dir);

            foreach (FtpListItem item in client.GetListing(dir))
                main.AddFTPItemToConsole(item);

            main.SetInputPrefix();
        }

        public void UploadFile(string from, string to)
        {
            try
            {
                client.UploadFile(from, to);
                main.AddTextToConsole("Successfully uploaded file...");
                main.SetInputPrefix();
            }
            catch (Exception)
            {
                main.AddTextToConsole("Could not upload file...");
                return;
            }
        }

        public void DownloadFile(string to, string from)
        {
            try
            {
                client.DownloadFile(to, from);
                main.AddTextToConsole("Successfully downloaded file...");
                main.SetInputPrefix();
            }
            catch (Exception)
            {
                main.AddTextToConsole("Could not download file...");
                return;
            }
        }

        public void UploadDirectory(string from, string to)
        {
            try
            {
                client.UploadDirectory(from, to);
                main.AddTextToConsole("Successfully uploaded directory...");
                main.SetInputPrefix();
            }
            catch (Exception)
            {
                main.AddTextToConsole("Could not upload directory...");
                return;
            }
        }

        public void DownloadDirectory(string to, string from)
        {
            try
            {
                client.DownloadDirectory(to, from);
                main.AddTextToConsole("Successfully downloaded directory...");
                main.SetInputPrefix();
            }
            catch (Exception)
            {
                main.AddTextToConsole("Could not download directory...");
                return;
            }
        }

        public void DeleteFile(string path)
        {
            try
            {
                client.DeleteFile(path);
                main.AddTextToConsole("Successfully deleted the file...");
                main.SetInputPrefix();
            }
            catch (Exception)
            {
                main.AddTextToConsole("Could not delete file...");
                return;
            }
        }

        public void DeleteDirectory(string path)
        {
            try
            {
                client.DeleteDirectory(path);
                main.AddTextToConsole("Successfully deleted the directory...");
                main.SetInputPrefix();
            }
            catch (Exception)
            {
                main.AddTextToConsole("Could not delete directory...");
                return;
            }
        }

        public void Terminate()
        {
            try
            {
                client.Disconnect();
                main.AddTextToConsole("Successfully terminated connection to the server...");
                main.SetInputPrefix();
            }
            catch (Exception)
            {
                main.AddTextToConsole("Could not terminate connection, please try again...");
                return;
            }
        }
    }
}
