using FluentFTP;
using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static CustomShell.Calculator;

namespace CustomShell
{
    public partial class MainController : Form
    {
        //TODO: Add support for linux filepaths as well, add an OS check in constructor of this class
        string currentDir = @"C:/";
        string historyFilePath = @"C:\Users\" + Environment.UserName + @"\AppData\Local\CMD++\cmdHistory.log";
        string[] cmdHistory;
        int historyLen;
        WandEditor wand;
        Processes proc;
        Compression comp;
        BatchInterpreter batch;
        FTPController ftpController;
        SSHClient sshClient;
        LocalDirectory localDirectory;
        SystemInformation systemInfo;
        public static MainController controller { get; private set; }

        public MainController()
        {
            InitializeComponent();

            if (!Directory.Exists(@"C:\Users\" + Environment.UserName + @"\AppData\Local\CMD++"))
                Directory.CreateDirectory(@"C:\Users\" + Environment.UserName + @"\AppData\Local\CMD++");

            if (!File.Exists(historyFilePath))
                File.Create(historyFilePath).Close();

            if (controller != null)
                throw new Exception("Only one instance of MainController may ever exist!");

            controller = this;
            LoadHistoryFile();
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
            UpdateHistoryFile(command);//Update history file
            inputBox.Text = InputPrefix(); //Clear input area
            inputBox.SelectionStart = inputBox.Text.Length;//Set cursor to right position
        }

        public void UpdateHistoryFile(string command)
        {
            if(cmdHistory.Length > 0)
                if(cmdHistory[cmdHistory.Length - 1] != command)//Check if the last command is the same as the current one so that there are no doubles in the history
                    File.AppendAllText(historyFilePath, command + "\n");

            LoadHistoryFile();//Update history array
        }

        public void LoadHistoryFile()
        {
            cmdHistory = File.ReadAllLines(historyFilePath);
            historyLen = cmdHistory.Length;
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
        public void Rename(string[] tokens)
        {
            string path = string.Empty;
            string name = string.Empty;

            if (!tokens[1].Contains(@":\"))
                path = GetFullPathFromName(tokens[1]);
            else
                path = tokens[1];

            if (!tokens[2].Contains(@":\"))
                name = GetFullPathFromName(tokens[2]);
            else
                name = tokens[2];

            try
            {
                if (path.Contains("."))
                    File.Move(path, name);
                else
                    Directory.Move(path, name);

                AddCommandToConsole(tokens);
            }
            catch (Exception e)
            {
                AddTextToConsole("Cannot find file path, did you enter the right name?");
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
            sb.Append("ssh connect [host] [user] [password]             | Opens a connection to the SSH host\n");
            sb.Append("sshCom [SSH Command]                             | Executes an SSH command an returns the result\n");
            sb.Append("ssh close                                        | Terminates the connection to the SSH host\n");
            sb.Append("help                                             | Display help\n");
            sb.Append("shutdown                                         | Shuts down the computer\n");
            sb.Append("exit                                             | Exits the shell");

            AddTextToConsole(sb.ToString());
            AddCommandToConsole(com);
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

        public string[] tokens;
        int historyIndex;
        bool firstClick = true;
        private void inputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(firstClick == true)
            {
                historyLen = cmdHistory.Length;
                historyIndex = historyLen - 1;
            }

            //When command is entered
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                historyIndex = cmdHistory.Length - 1;
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
                        case true when cmds[i].StartsWith("rename"):
                            Rename(tokens);
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
                            AddCommandToConsole(tokens);
                            break;
                        case true when cmds[i].StartsWith("killproc"):
                            if (proc == null)
                                proc = new Processes();
                            proc.KillProcess(tokens);
                            break;
                        case true when cmds[i].StartsWith("calc")://Broken fucking calculator, someone please fix it.
                            CreateTokens(tokens);
                            AddCommandToConsole(tokens);
                            break;
                        case true when cmds[i].StartsWith("batch"):
                            if (batch == null)
                                batch = new BatchInterpreter();
                            batch.ExecuteCommand(tokens);
                            break;
                        case true when cmds[i].StartsWith("system"):
                            if (systemInfo == null)
                                systemInfo = new SystemInformation();
                            AddCommandToConsole(tokens);

                            systemInfo = null;
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

                            AddCommandToConsole(tokens);
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
                        case true when cmds[i].StartsWith("ssh"):
                            if(sshClient == null)
                                sshClient = new SSHClient();

                            if (tokens[0] == "sshCom" || tokens[0] == "sshcom")
                            {
                                StringBuilder sb = new StringBuilder();
                                for (int x = 1; x < tokens.Length; ++x)
                                {
                                    sb.Append(tokens[x]);
                                    if(x + 1 < tokens.Length)
                                        sb.Append(" ");
                                }
                                sshClient.SendCommand(sb.ToString());
                            }
                            else if (tokens[0] == "ssh" && tokens[1] == "close")
                                sshClient.TerminateConnection();
                            else if(tokens[0] == "ssh" && tokens[1] == "connect")
                                sshClient.EstablishConnection(tokens[2], tokens[3], tokens[4]);

                            AddCommandToConsole(tokens);
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
            /*
            The Command History system is extremley janky with a lot of shenanigans
            Don't ever change this in the future because it is not understandable
            firstClick needs to be there so that we stay inside of bounds and
            only change the index by 1, if there is no bool it will either 
            go out of bounds or change index by 2
            */
            if (e.KeyCode == Keys.Up)
            {
                if (cmdHistory.Length == 0 || cmdHistory == null)
                    return;
                e.Handled = true;
                if (historyIndex > 0 && historyIndex <= cmdHistory.Length)
                {
                    if(firstClick == false)
                        --historyIndex;
                    inputBox.Text = string.Concat(InputPrefix(), " ", cmdHistory[historyIndex]);
                    inputBox.SelectionStart = inputBox.Text.Length;//Set cursor to right position

                    if(firstClick == true)
                        firstClick = false;
                }
            }

            if (e.KeyCode == Keys.Down)
            {
                if (cmdHistory.Length == 0 || cmdHistory == null)
                    return;
                e.Handled = true;
                if (historyIndex > 0 && historyIndex < cmdHistory.Length - 1)
                {
                    if (firstClick == false)
                        ++historyIndex;
                    inputBox.Text = string.Concat(InputPrefix(), " ", cmdHistory[historyIndex]); 
                    inputBox.SelectionStart = inputBox.Text.Length;//Set cursor to right position
                    if (firstClick == true)
                        firstClick = false;
                }
                else if(historyIndex == 0 && firstClick == false) //This else if must be here so that you can press down to clear the history if you are at the end
                {
                    ++historyIndex;
                    inputBox.Text = string.Concat(InputPrefix(), " ", cmdHistory[historyIndex]);
                    inputBox.SelectionStart = inputBox.Text.Length;
                }
                else if (historyIndex == cmdHistory.Length - 1)
                {
                    ++historyIndex;
                    inputBox.Text = InputPrefix();
                    inputBox.SelectionStart = inputBox.Text.Length;//Set cursor to right position
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
                wand.SaveAndExit();
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
                wand.Exit();
        }

        private void MainController_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ftpController != null)
                ftpController = null;

            if(sshClient != null)
            {
                if (sshClient.client.IsConnected)
                    sshClient.TerminateConnection();

                sshClient = null;
            }

            if (systemInfo != null)
                systemInfo = null;
        }
    }
}
