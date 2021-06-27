using FluentFTP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using static CustomShell.Calculator;

namespace CustomShell
{
    public partial class MainController : Form
    {
        string currentDir = @"C:/";
        List<string> history = new List<string>();
        List<string> queue = new List<string>();
        WandEditor wand;
        Processes proc;
        Compression comp;
        BatchInterpreter batch;
        FTPController ftpController;
        LocalDirectory localDirectory;
        public static MainController controller { get; private set; }

        public MainController()
        {
            InitializeComponent();

            if (controller != null)
                throw new Exception("Only one instance of cMainForm may ever exist!");

            controller = this;
            InitConsole();
        }

        public string InputPrefix()
        {
            string text = string.Concat(Environment.UserName, "@", currentDir, " ~ ");
            return text;
        }

        //This is used to get a full file path if only a file or folder is entered 
        //and the user expects the command to understand that it's the file inside of the current directory 
        public string GetFullPathFromArray(string[] tokens)
        {
            return string.Concat(currentDir, @"\", tokens[tokens.Length - 1]);
        }

        public string GetFullPathFromName(string path)
        {
            if (!currentDir.EndsWith("\\"))
                return string.Concat(currentDir, @"\", path);
            else
                return string.Concat(currentDir, path);
        }

        //Check if the user has entered a complete file path or only a file or folder within the current directory
        public string CheckInputType(string[] tokens)
        {
            string path;
            if (!tokens[tokens.Length - 1].Contains(@":\"))
                path = GetFullPathFromArray(tokens);
            else
                path = tokens[1];

            return path;
        }

        public string GetPathType(string path)
        {
            string input;
            if (!path.Contains(@":\"))
                input = GetFullPathFromName(path);
            else
                input = path;

            return input;
        }

        public void InitConsole()
        {
            inputBox.Text = string.Concat(Environment.UserName, "@", currentDir, " ~ ");
            inputBox.SelectionStart = inputBox.Text.Length;
            this.ActiveControl = inputBox;
        }

        public void AddCommandToConsole(string[] tokens)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < tokens.Length; ++i)
                sb.Append(tokens[i] + " ");

            string command = sb.ToString();
            outputBox.AppendText(command + "\n");
            outputBox.SelectionStart = outputBox.TextLength;
            outputBox.ScrollToCaret();
            history.Add(command);

            inputBox.Text = InputPrefix(); //Clear input area
            inputBox.SelectionStart = inputBox.Text.Length;//Set cursor to right position
        }

        public void AddTextToConsole(string text)
        {
            outputBox.AppendText(text + "\n");
            outputBox.SelectionStart = outputBox.TextLength;
            outputBox.ScrollToCaret();
        }

        public void AddFTPItemToConsole(FtpListItem item)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(item.FullName);
            sb.Append("               ");
            sb.Append(item.Modified);
            sb.Append("               ");
            sb.Append(FormatBytes(item.Size));
            AddTextToConsole(sb.ToString());
        }

        public string FormatBytes(long bytes)
        {
            string[] suffix = { "B", "KB", "MB", "GB", "TB" };
            int i;
            double dblSByte = bytes;
            for (i = 0; i < suffix.Length && bytes >= 1024; i++, bytes /= 1024)
                dblSByte = bytes / 1024.0;

            return string.Format("{0:0.##} {1}", dblSByte, suffix[i]);
        }

        #region Commands
        public void ChangeDirectory(string[] tokens)
        {
            string path = CheckInputType(tokens);

            if (tokens.Length == 1) //Go to root
            {
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                    currentDir = @"C:\";
                else if (Environment.OSVersion.Platform == PlatformID.Unix)
                    currentDir = @"~";
            }
            else if (tokens[1] == "..")//Go back one folder
            {
                string dir = currentDir;
                int index = dir.LastIndexOf(@"\");
                if (index > 0)
                    dir = dir.Substring(0, index);

                currentDir = dir;
            }
            else if (Directory.Exists(path))
            {
                currentDir = path;
            }
            AddCommandToConsole(tokens);
        }

        public void ListFiles(string[] tokens)
        {
            try
            {
                if (tokens.Length == 1)
                {
                    AddCommandToConsole(tokens);
                    IEnumerable entries = Directory.EnumerateFileSystemEntries(currentDir);
                    foreach (string item in entries)
                    {
                        FileAttributes attr = File.GetAttributes(item.ToString());
                        AddTextToConsole(item.ToString());
                        outputBox.Select(outputBox.Text.Length - item.Length - 1, outputBox.Text.Length);

                        if (attr.HasFlag(FileAttributes.Directory))
                            outputBox.SelectionColor = Color.Green;
                        else
                            outputBox.SelectionColor = Color.Red;
                    }
                }
                else if (tokens.Length == 2)
                {
                    AddCommandToConsole(tokens);

                    string path = CheckInputType(tokens);
                    IEnumerable entries = Directory.EnumerateFileSystemEntries(path);
                    foreach (string item in entries)
                    {
                        FileAttributes attr = File.GetAttributes(item.ToString());
                        AddTextToConsole(item.ToString());
                        outputBox.Select(outputBox.Text.Length - item.Length - 1, outputBox.Text.Length);

                        if (attr.HasFlag(FileAttributes.Directory))
                            outputBox.SelectionColor = Color.Green;
                        else
                            outputBox.SelectionColor = Color.Red;
                    }
                }
                else
                    AddTextToConsole("Command is not valid");

                outputBox.SelectionStart = outputBox.TextLength;
            }
            catch (Exception e)
            {
                if (e.InnerException is UnauthorizedAccessException)
                {
                    AddTextToConsole("Cannot access directory. Please run shell as admin.");
                    return;
                }
            }
        }

        public void MakeDirectory(string[] tokens)
        {
            string dir = CheckInputType(tokens);

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
                AddCommandToConsole(tokens);
            }
            else
                AddTextToConsole("Directory already exists.");
        }

        public void MakeFile(string[] tokens)
        {
            string path = CheckInputType(tokens);

            if (!File.Exists(path))
            {
                try
                {
                    File.Create(path);
                    AddCommandToConsole(tokens);
                }
                catch (Exception e)
                {
                    if (e.InnerException is UnauthorizedAccessException)
                    {
                        AddTextToConsole("Cannot access directory. Please run shell as admin.");
                        return;
                    }
                    else
                        AddTextToConsole("Error: make sure the filepath is valid");

                }
            }
            else
                AddTextToConsole("File already exists.");
        }

        public void RemoveFolder(string[] tokens)
        {
            string path = CheckInputType(tokens);

            if (tokens[1].Contains("."))
            {
                if (File.Exists(path))
                    File.Delete(path);
                else
                    AddTextToConsole("File doesn't exists");
            }
            else
            {
                if (Directory.Exists(path))
                {
                    if (tokens[1] == "-r")
                        Directory.Delete(path, true);
                    else
                    {
                        try
                        {
                            Directory.Delete(path, false);
                        }
                        catch (Exception e)
                        {
                            if (e.InnerException is UnauthorizedAccessException)
                            {
                                AddTextToConsole("Cannot access directory. Please run shell as admin.");
                                return;
                            }
                            else if (e.InnerException is IOException)
                            {
                                AddTextToConsole("Directory is not empty. Use -r to remove recursivley");
                                return;
                            }
                            else
                                return; //Hopefully never gets to this so don't display anything
                        }
                    }

                }
                else
                    AddTextToConsole("Folder doesn't exists");
            }
            AddCommandToConsole(tokens);
        }

        public void CopyFile(string[] tokens)
        {
            if (tokens.Length == 3)
            {
                string input = GetPathType(tokens[1]);
                string output = GetPathType(tokens[2]);

                try
                {
                    FileAttributes attr = File.GetAttributes(input);
                    if (!attr.HasFlag(FileAttributes.Directory))
                    {
                        if (!File.Exists(output))
                            File.Copy(input, output);
                        else
                            AddTextToConsole("Output file already exists");
                    }
                    else
                    {
                        if (!Directory.Exists(output))
                        {
                            //UNFINISHED, TO BE ADDED
                        }
                        else
                            AddTextToConsole("Output directory already exists");
                    }
                    AddCommandToConsole(tokens);
                }
                catch (Exception e)
                {
                    if (e.InnerException is UnauthorizedAccessException)
                    {
                        AddTextToConsole("Cannot access directory. Please run shell as admin.");
                        return;
                    }
                }

            }
            else if (tokens.Length == 2)
            {
                try
                {
                    string inputPath = GetPathType(tokens[1]); //This is the path to the inputfile
                    string fileInput = GetPathType(tokens[1]); //This gets converted to ONLY THE FILENAME

                    int index = fileInput.LastIndexOf(@"\");//Remove the filename from the filepath so 
                    if (index > 0)                          //we can add a modified filename instead
                        fileInput = fileInput.Substring(index + 1, fileInput.Length - index - 1);

                    StringBuilder sb = new StringBuilder();
                    sb.Append("copy_");
                    sb.Append(fileInput);
                    string output = sb.ToString();
                    string filePath = GetPathType(output);

                    if (!File.Exists(output))
                        File.Copy(inputPath, filePath);
                    else
                        AddTextToConsole("Output file already exists");

                    AddCommandToConsole(tokens);
                }
                catch (Exception e)
                {
                    if (e.InnerException is UnauthorizedAccessException)
                    {
                        AddTextToConsole("Cannot access directory. Please run shell as admin.");
                        return;
                    }
                }
            }
            else
            {
                AddTextToConsole("Command not formatted correctly");
                return;
            }
        }

        public void DisplayHelp()
        {
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
            sb.Append("rm [Path]                                        | Removes a file or folder\n");
            sb.Append("exec [PathToExe]                                 | Executes an .EXE file\n");
            sb.Append("open [PathToFile]                                | Opens a file with the standard app\n");
            sb.Append("extr [PathToZip] (OutputFolder)                  | Extracts a zip file\n");
            sb.Append("compr [Path] (OutputArchive)                     | Compresses a foler or file into a zip\n");
            sb.Append("calc [Equation]                                  | Calculates the given equation\n");
            sb.Append("size [Path]                                      | Gets the size of a folder\n");
            sb.Append("peek [Path]                                      | Prints all the text in a file\n");
            sb.Append("wand [Path]                                      | Lets you edit a file. CTRL + S to save. CTRL + Q to quit\n");
            sb.Append("listproc                                         | Lists all running processes\n");
            sb.Append("killproc [ID]                                    | Lets you kill a process\n");
            sb.Append("batch [CommandOrBatFile]                         | Lets you run any batch command or script file\n");
            sb.Append("clear                                            | Clears the console\n");
            sb.Append("fcolor [Color]                                   | Changes the text color of the console\n");
            sb.Append("bcolor [Color]                                   | Changes the back color of the console\n");
            sb.Append("ftp [ip] [true/false] (username) (Password)      | Starts an FTP/FTPS Connection\n");
            sb.Append("ftp uploadFile [local path] [remote path]        | Uploads a file to the FTP server\n");
            sb.Append("ftp downloadFile [local path] [remote path]      | Downloads a file from the FTP server\n");
            sb.Append("ftp uploadDirectory [local path] [remote path]   | Uploads a directory to the FTP server\n");
            sb.Append("ftp downloadDirectory [local path] [remote path] | Downloads a directory from the FTP server\n");
            sb.Append("ftp close                                        | Terminates the connection to the FTP server\n");
            sb.Append("help                                             | Display help\n");
            sb.Append("shutdown                                         | Shuts down the computer\n");
            sb.Append("exit                                             | Exits the shell");

            AddTextToConsole(sb.ToString());
            inputBox.Text = InputPrefix();
            inputBox.SelectionStart = inputBox.Text.Length;
        }

        public void Execute(string[] tokens)
        {
            string path = CheckInputType(tokens);
            if (!tokens[1].EndsWith(".exe"))
            {
                AddTextToConsole("You have not entered a valid file to execute");
                return;
            }
            else
            {
                try
                {
                    Process.Start(path);
                }
                catch (Exception)
                {
                    AddTextToConsole("Something went wrong...");
                }
            }
            AddCommandToConsole(tokens);
        }

        public void OpenFile(string[] tokens)
        {
            string path = CheckInputType(tokens);
            if (!tokens[1].Contains("."))
            {
                AddTextToConsole("You have not entered a valid file to open");
                return;
            }
            else
            {
                try
                {
                    Process.Start(path);
                    AddCommandToConsole(tokens);
                }
                catch (Exception)
                {
                    AddTextToConsole("Something went wrong...");
                }
            }
        }

        public void ClearConsole()
        {
            outputBox.Text = string.Empty;
            inputBox.Text = InputPrefix();
            inputBox.SelectionStart = inputBox.Text.Length;
        }

        public void DirectorySize(string[] tokens)
        {
            string path = CheckInputType(tokens);
            long size = 0;
            DirectoryInfo di = new DirectoryInfo(path);

            try
            {
                foreach (FileInfo fi in di.GetFiles("*", SearchOption.AllDirectories))
                    size += fi.Length;
            }
            catch (Exception)
            {
                AddTextToConsole("Path is invalid");
                return;
            }

            string output = FormatBytes(size);

            AddCommandToConsole(tokens);
            AddTextToConsole(output);
        }

        public new void Move(string[] tokens)
        {
            if (tokens.Length == 3)
            {
                string input;
                if (!tokens[1].Contains(@":\"))
                    input = GetFullPathFromName(tokens[1]);
                else
                    input = tokens[1];

                string output;
                if (!tokens[2].Contains(@":\"))
                    output = GetFullPathFromName(tokens[2]);
                else
                    output = tokens[2];

                try
                {
                    if (!input.Contains("."))
                        Directory.Move(input, output);
                    else
                        File.Move(input, output);

                    AddCommandToConsole(tokens);
                }
                catch (Exception e)
                {
                    if (e.InnerException is UnauthorizedAccessException)
                    {
                        AddTextToConsole("Cannot access directory. Please run shell as admin.");
                        return;
                    }
                }
            }
            else
            {
                AddTextToConsole("Invalid command format...");
                return;
            }
        }

        public void ExitShell()
        {
            Application.Exit();
        }

        public void Shutdown()
        {
            Process.Start("shutdown", "/s /t 10");
        }
        #endregion

        int historyIndex = 0;
        private void inputBox_KeyDown(object sender, KeyEventArgs e)
        {
            string[] tokens;
            //When command is entered
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;

                int commands = 1; //Default is one but will be incresed if there are any && in the input line
                string[] cmds;
                string input = inputBox.Text;
                string command = input.Remove(0, (Environment.UserName + "@" + currentDir + " ~").Length);
                command = command.Trim();

                if (string.IsNullOrEmpty(command) || string.IsNullOrWhiteSpace(command))
                    return;

                tokens = command.Split(' ');

                if (command.Contains("&&"))
                {
                    string[] cmd = command.Split(new string[] { "&&" }, StringSplitOptions.None); //Split input line into seperate commands
                    cmds = new string[cmd.Length];
                    commands = cmd.Length;
                    for (int i = 0; i < cmd.Length; ++i)
                        cmds[i] = cmd[i].Trim();//Trim the white chars
                }
                else
                    cmds = tokens;

                for (int i = 0; i < commands; ++i)
                {
                    if (commands > 1)
                        tokens = cmds[i].Split(' ');//Split all the commands into tokens and run them

                    switch (true)
                    {
                        case true when cmds[i].StartsWith("cd"):
                            ChangeDirectory(tokens);
                            break;
                        case true when cmds[i].StartsWith("ls"):
                            ListFiles(tokens);
                            break;
                        case true when cmds[i].StartsWith("mkdir"):
                            MakeDirectory(tokens);
                            break;
                        case true when cmds[i].StartsWith("mkfile"):
                            MakeFile(tokens);
                            break;
                        case true when cmds[i].StartsWith("cp"):
                            CopyFile(tokens);
                            break;
                        case true when cmds[i].StartsWith("mv"):
                            Move(tokens);
                            break;
                        case true when cmds[i].StartsWith("rm"):
                            RemoveFolder(tokens);
                            break;
                        case true when cmds[i].StartsWith("help"):
                            DisplayHelp();
                            break;
                        case true when cmds[i].StartsWith("exec"):
                            Execute(tokens);
                            break;
                        case true when cmds[i].StartsWith("open"):
                            OpenFile(tokens);
                            break;
                        case true when cmds[i].StartsWith("extr"):
                            if (comp == null)
                                comp = new Compression();
                            comp.ExtractArchive(tokens);
                            break;
                        case true when cmds[i].StartsWith("compr"):
                            if (comp == null)
                                comp = new Compression();
                            comp.CompressFolder(tokens);
                            break;
                        case true when cmds[i].StartsWith("size"):
                            DirectorySize(tokens);
                            break;
                        case true when cmds[i].StartsWith("peek"):
                            if (wand == null)
                                wand = new WandEditor();
                            wand.PeekFile(tokens);
                            break;
                        case true when cmds[i].StartsWith("wand"):
                            if (wand == null)
                                wand = new WandEditor();
                            wand.LoadFile(tokens);
                            break;
                        case true when cmds[i].StartsWith("clear"):
                            ClearConsole();
                            break;
                        case true when cmds[i].StartsWith("exit"):
                            ExitShell();
                            break;
                        case true when cmds[i].StartsWith("shutdown"):
                            Shutdown();
                            break;
                        case true when cmds[i].StartsWith("listproc"):
                            if (proc == null)
                                proc = new Processes();
                            proc.ListProcesses();
                            break;
                        case true when cmds[i].StartsWith("killproc"):
                            if (proc == null)
                                proc = new Processes();
                            proc.KillProcess(tokens);
                            break;
                        case true when cmds[i].StartsWith("calc")://Broken fucking calculator, someone please fix it.
                            CreateTokens(tokens);
                            break;
                        case true when cmds[i].StartsWith("batch"):
                            if (batch == null)
                                batch = new BatchInterpreter();
                            batch.ExecuteCommand(tokens);
                            break;
                        case true when cmds[i].StartsWith("ftp"):
                            if (ftpController == null)
                            {
                                ftpController = new FTPController(tokens[1]);
                                if (tokens[2] == "true")
                                    ftpController.StartFTPSConnection(tokens[3], tokens[4]);
                                else
                                    ftpController.StartFTPConnection();
                            }

                            if (tokens[1] == "uploadFile")
                                ftpController.UploadFile(tokens[2], tokens[3]);
                            else if (tokens[1] == "downloadFile")
                                ftpController.DownloadFile(tokens[2], tokens[3]);
                            else if (tokens[1] == "uploadDirectory")
                                ftpController.UploadDirectory(tokens[2], tokens[3]);
                            else if (tokens[1] == "downloadDirectory")
                                ftpController.DownloadDirectory(tokens[2], tokens[3]);
                            else if (tokens[1] == "deleteFile")
                                ftpController.DeleteFile(tokens[2]);
                            else if (tokens[1] == "deleteDirectory")
                                ftpController.DeleteDirectory(tokens[2]);
                            else if (tokens[1] == "ls")
                            {
                                AddTextToConsole("Path               Last Modified               Size");
                                if (tokens.Length > 2)
                                    ftpController.GetDir(tokens[2]);
                                else
                                    ftpController.GetDir("\\");
                            }
                            else if (tokens[1] == "close")
                                ftpController.Terminate();
                            break;
                        case true when cmds[i].StartsWith("fcolor"):
                            string fcolor = tokens[1].ToUpper();

                            if (fcolor == "RED" || fcolor == "01" || fcolor == "1")
                            {
                                outputBox.ForeColor = Color.Red;
                                inputBox.ForeColor = Color.Red;
                            }
                            else if (fcolor == "GREEN" || fcolor == "02" || fcolor == "2")
                            {
                                outputBox.ForeColor = Color.Green;
                                inputBox.ForeColor = Color.Green;
                            }
                            else if (fcolor == "YELLOW" || fcolor == "03" || fcolor == "3")
                            {
                                outputBox.ForeColor = Color.Yellow;
                                inputBox.ForeColor = Color.Yellow;
                            }
                            else if (fcolor == "ORANGE" || fcolor == "04" || fcolor == "4")
                            {
                                outputBox.ForeColor = Color.Orange;
                                inputBox.ForeColor = Color.Orange;
                            }
                            else if (fcolor == "BLUE" || fcolor == "05" || fcolor == "5")
                            {
                                outputBox.ForeColor = Color.Blue;
                                inputBox.ForeColor = Color.Blue;
                            }
                            else if (fcolor == "WHITE" || fcolor == "06" || fcolor == "6")
                            {
                                outputBox.ForeColor = Color.White;
                                inputBox.ForeColor = Color.White;
                            }
                            else if (fcolor == "TURQUOISE" || fcolor == "07" || fcolor == "7")
                            {
                                outputBox.ForeColor = Color.Turquoise;
                                inputBox.ForeColor = Color.Turquoise;
                            }
                            else if (fcolor == "BLACK" || fcolor == "08" || fcolor == "8")
                            {
                                outputBox.ForeColor = Color.Black;
                                inputBox.ForeColor = Color.Black;
                            }

                            inputBox.Text = InputPrefix();
                            inputBox.SelectionStart = inputBox.Text.Length;
                            break;
                        case true when cmds[i].StartsWith("bcolor"):
                            string bcolor = tokens[1].ToUpper();

                            if (bcolor == "RED" || bcolor == "01" || bcolor == "1")
                            {
                                outputBox.BackColor = Color.Red;
                                inputBox.BackColor = Color.Red;
                            }
                            else if (bcolor == "GREEN" || bcolor == "02" || bcolor == "2")
                            {
                                outputBox.BackColor = Color.Green;
                                inputBox.BackColor = Color.Green;
                            }
                            else if (bcolor == "YELLOW" || bcolor == "03" || bcolor == "3")
                            {
                                outputBox.BackColor = Color.Yellow;
                                inputBox.BackColor = Color.Yellow;
                            }
                            else if (bcolor == "ORANGE" || bcolor == "04" || bcolor == "4")
                            {
                                outputBox.BackColor = Color.Orange;
                                inputBox.BackColor = Color.Orange;
                            }
                            else if (bcolor == "BLUE" || bcolor == "05" || bcolor == "5")
                            {
                                outputBox.BackColor = Color.Blue;
                                inputBox.BackColor = Color.Blue;
                            }
                            else if (bcolor == "WHITE" || bcolor == "06" || bcolor == "6")
                            {
                                outputBox.BackColor = Color.White;
                                inputBox.BackColor = Color.White;
                            }
                            else if (bcolor == "TURQUOISE" || bcolor == "07" || bcolor == "7")
                            {
                                outputBox.BackColor = Color.Turquoise;
                                inputBox.BackColor = Color.Turquoise;
                            }
                            else if (bcolor == "BLACK" || bcolor == "08" || bcolor == "8")
                            {
                                outputBox.BackColor = Color.Black;
                                inputBox.BackColor = Color.Black;
                            }

                            inputBox.Text = InputPrefix();
                            inputBox.SelectionStart = inputBox.Text.Length;
                            break;
                        default:
                            AddTextToConsole("Command does not exist");
                            break;
                    }
                }
            }

            if (e.KeyCode == Keys.Tab)
            {
                e.Handled = true;
                if (localDirectory == null)
                    localDirectory = new LocalDirectory();
                localDirectory.GetAllCurrentDir(inputBox.Text, currentDir);
            }

            #region History
            if (e.KeyCode == Keys.Up)
            {
                e.Handled = true;
                if (historyIndex >= 0 && historyIndex < history.Count)
                {
                    inputBox.Text = string.Concat(InputPrefix(), " ", history[history.Count - historyIndex - 1]); //Clear input area
                    inputBox.SelectionStart = inputBox.Text.Length;//Set cursor to right position
                    ++historyIndex;
                }
            }

            if (e.KeyCode == Keys.Down)
            {
                e.Handled = true;
                if (historyIndex > 0 && historyIndex <= history.Count)
                {
                    inputBox.Text = string.Concat(InputPrefix(), " ", history[history.Count - historyIndex]); //Clear input area
                    inputBox.SelectionStart = inputBox.Text.Length;//Set cursor to right position
                    --historyIndex;
                }
            }

            #endregion
        }

        private void inputBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
                inputBox.Text.TrimEnd();
        }

        bool syntaxHighlight = true;
        private void wandTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)//Save and quit from Wand
            {
                wand.SaveAndExit();
            }
            else if (e.Control && e.KeyCode == Keys.H)//Toggle syntax highlighting
            {
                if (syntaxHighlight == true)
                {
                    syntaxHighlight = false;
                    wand.RemoveSyntaxHighlight();
                }
                else
                {
                    syntaxHighlight = true;
                    wand.ApplySyntaxHighlight();
                }
            }
            else if (e.Control && e.KeyCode == Keys.Q)//Quit without save Wand
            {
                wand.Exit();
            }
        }

        private void MainController_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ftpController != null)
                ftpController = null;
        }
    }
}
