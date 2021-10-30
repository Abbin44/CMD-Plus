using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomShell
{
    public class Helper
    {
        MainController main = MainController.controller;
        public Helper()
        {

        }

        public void DisplayHelp()
        {
            string[] com = { "help" };
            StringBuilder sb = new StringBuilder();
            sb.Append("Not everything can be displayed here, please refer to https://github.com/Abbin44/CMD-Plus/wiki for more help\n");
            sb.Append("() means optional parameter\n");
            sb.Append("------------------------------------------------------------------------------------------\n");
            sb.Append("cd [Path]                                        | Change directory\n");
            sb.Append("ls (Path)                                        | List all files and folders in a directory\n");
            sb.Append("mkdir [Path]                                     | Creates a folder\n");
            sb.Append("mkfile [Path]                                    | Creates a file\n");
            sb.Append("cp [InputPath] (OutputPath)                      | Copies a file\n");
            sb.Append("mv [InputPath] [OutputPath]                      | Moves a file or folder\n");
            sb.Append("rm (-r) [Path]                                   | Removes a file or folder, -r means recursive removal\n");
            sb.Append("system                                           | Displays system information in a nice way\n");
            sb.Append("script [ScriptPath]                              | Runs a script file with the .rls extention\n");
            sb.Append("exec [PathToExe]                                 | Executes an .EXE file\n");
            sb.Append("open [PathToFile]                                | Opens a file with the standard app\n");
            sb.Append("extr [PathToZip] (OutputFolder)                  | Extracts a zip file\n");
            sb.Append("compr [Path] (OutputArchive)                     | Compresses a foler or file into a zip\n");
            sb.Append("calc [Equation]                                  | Calculates the given equation\n");
            sb.Append("size [Path]                                      | Gets the size of a folder\n");
            sb.Append("peek [Path]                                      | Prints all the text in a file\n");
            sb.Append("wand [Path]                                      | Lets you edit a file. CTRL + S to save. CTRL + Q to quit. CTRL + H to toggle syntax highlight. CTRL + D to duplicate line. \n");
            sb.Append("listproc                                         | Lists all running processes\n");
            sb.Append("killproc [ID]                                    | Lets you kill a process\n");
            sb.Append("batch [CommandOrBatFile]                         | Lets you run any batch command or script file\n");
            sb.Append("clear                                            | Clears the console\n");
            sb.Append("clear history                                    | Clears the command history\n");
            sb.Append("history                                          | Displays command history with indexes\n");
            sb.Append("!!                                               | Runs the last command from the history file\n");
            sb.Append("![number]                                        | Runs the command from the history file with the corresponding index that is passed in number\n");
            sb.Append("fcolor [Color]                                   | Changes the text color of the console\n");
            sb.Append("bcolor [Color]                                   | Changes the back color of the console\n");
            sb.Append("ftp [ip] [true/false] (username) (Password)      | Starts an FTP/FTPS Connection\n");
            sb.Append("ftp uploadFile [local path] [remote path]        | Uploads a file to the FTP server\n");
            sb.Append("ftp downloadFile [local path] [remote path]      | Downloads a file from the FTP server\n");
            sb.Append("ftp uploadDirectory [local path] [remote path]   | Uploads a directory to the FTP server\n");
            sb.Append("ftp downloadDirectory [local path] [remote path] | Downloads a directory from the FTP server\n");
            sb.Append("ftp close                                        | Terminates the connection to the FTP server\n");
            sb.Append("ssh connect [host] [user] [password]             | Opens a connection to the SSH host\n");
            sb.Append("sshCom [SSH Command]                             | Executes an SSH command an returns the result\n");
            sb.Append("ssh close                                        | Terminates the connection to the SSH host\n");
            sb.Append("help                                             | Display help\n");
            sb.Append("shutdown                                         | Shuts down the computer\n");
            sb.Append("exit                                             | Exits the shell");

            main.AddCommandToConsole(com);
            main.AddTextToConsole(sb.ToString());
            main.SetInputPrefix();
        }
    }
}
