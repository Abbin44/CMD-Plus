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
        string prgDirPath = @"C:\Users\" + Environment.UserName + @"\AppData\Local\CMD++\";
        string settingsPath = @"C:\Users\" + Environment.UserName + @"\AppData\Local\CMD++\settings.cfg";
        string[] cmdHistory;
        int historyLen;
        WandEditor wand;
        Processes proc;
        Helper helper;
        Compression comp;
        BatchInterpreter batch;
        FTPController ftpController;
        SSHClient sshClient;
        LocalDirectory localDirectory;
        SystemInformation systemInfo;
        ScriptInterpreter script;
        Coloring coloring;
        SettingsManager settings;
        public static MainController controller { get; private set; }

        public MainController()
        {
            InitializeComponent();

            if (!Directory.Exists(prgDirPath))
                Directory.CreateDirectory(prgDirPath);

            if (!File.Exists(historyFilePath))
                File.Create(historyFilePath).Close();

            if (!File.Exists(settingsPath))
                File.Create(settingsPath).Close();

            if (controller != null)
                throw new Exception("Only one instance of MainController may ever exist!");

            controller = this;
            LoadHistoryFile();
            InitConsole();
            settings = new SettingsManager();
        }

        public string GetInputPrefix()
        {
            string text = string.Concat(Environment.UserName, "@", currentDir, " ~ ");
            return text;
        }

        public void SetInputPrefix()
        {
            inputBox.Text = GetInputPrefix();
            LockInputPrefix();
        }

        public void LockInputPrefix()
        {
            inputBox.Select(0, GetInputPrefix().Length);
            inputBox.SelectionProtected = true;
            inputBox.SelectionStart = inputBox.Text.Length;
        }

        public string GetFullPathFromName(string path)
        {
            if (!currentDir.EndsWith("\\"))
                return string.Concat(currentDir, @"\", path);
            else
                return string.Concat(currentDir, path);
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
            SetInputPrefix();
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
            SetInputPrefix();
        }

        public void UpdateHistoryFile(string command)
        {
            command = command.Trim();
            if(cmdHistory.Length > 0)
            {
                if(cmdHistory[cmdHistory.Length - 1] != command)//Check if the last command is the same as the current one so that there are no doubles in the history
                    File.AppendAllText(historyFilePath, command + "\n");
            }
            else
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

        public void ReplaceLastLineConsole(string line)
        {
            outputBox.Lines[outputBox.Lines.Length] = line;
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

        private string GetLastCommand()
        {
            return cmdHistory[cmdHistory.Length - 1];
        }
        
        private string GetCommandFromIndex(int index)
        {
            return cmdHistory[cmdHistory.Length - index];
        }

        #region Commands
        public void ChangeDirectory(string[] tokens)
        {
            string path = string.Empty;

            if (tokens.Length > 1)
                path = GetPathType(tokens[1]);
            else if(tokens.Length == 1 && tokens[0] == "cd")
            {
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                    currentDir = @"C:\";
                else if (Environment.OSVersion.Platform == PlatformID.Unix)
                    currentDir = @"~";

                SetInputPrefix();
                return;
            }

            if (tokens[1] == "..")//Go back one folder
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

                    string path = GetPathType(tokens[1]);
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
            string path = GetPathType(tokens[1]);
            string name = GetPathType(tokens[2]);

            try
            {
                if (path.Contains("."))
                    File.Move(path, name);
                else
                    Directory.Move(path, name);

                AddCommandToConsole(tokens);
            }
            catch (Exception)
            {
                AddTextToConsole("Cannot find file path, did you enter the right name?");
            }
        }

        public void MakeDirectory(string[] tokens)
        {
            string path = GetPathType(tokens[1]);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                AddCommandToConsole(tokens);
            }
            else
                AddTextToConsole("Directory already exists.");
        }

        public void MakeFile(string[] tokens)
        {
            string path = string.Empty;
            if (tokens.Length == 2)
                path = GetPathType(tokens[1]);
            else
            {
                AddTextToConsole("Wrong ammount of arguments...");
                return;
            }

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
            string path = string.Empty;
            if (!tokens[1].Equals("-r"))
                path = GetPathType(tokens[1]);
            else
                path = GetPathType(tokens[2]);

            if (path.Contains("."))
            {
                if (File.Exists(path))
                    File.Delete(path);
                else
                    AddTextToConsole("File doesn't exist...");
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
                            //TODO: COPY FOLDERS
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

        public void Execute(string[] tokens)
        {
            string path = GetPathType(tokens[1]);

            if (path.EndsWith(".exe"))
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
            string path = GetPathType(tokens[1]);

            if (!path.Contains("."))
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

        private void ClearConsole()
        {
            outputBox.Text = string.Empty;
            SetInputPrefix();
        }

        private void ClearHistory()
        {
            File.WriteAllText(historyFilePath, string.Empty);
            LoadHistoryFile();
            SetInputPrefix();
        }

        private void PrintHistory()
        {
            int counter = cmdHistory.Length - 1;
            string text = string.Empty;
            if (coloring == null)
                coloring = new Coloring();
            for (int i = 0; i < cmdHistory.Length; ++i)
            {
                text = counter.ToString() + ") " + cmdHistory[i];
                AddTextToConsole(text);
                coloring.FindAndColorString(text, Color.DarkOrange, outputBox);
                --counter;
            }
            //AddTextToConsole("\n");
        }

        private void DirectorySize(string[] tokens)
        {
            string path = GetPathType(tokens[1]);
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
                string input = GetPathType(tokens[1]);
                string output = GetPathType(tokens[2]);

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
        public void RunCommand(string command, bool fromScript) 
        {
            historyIndex = historyLen;
            int commands = 1; //Default is one but will be incresed if there are any && in the input line
            string[] cmds;
            string input = string.Empty;
            if (fromScript == false)//Check if command comes from input box of from a script file since the input prefix doesn't exist in a script file
            {
                input = inputBox.Text;
                command = input.Remove(0, (Environment.UserName + "@" + currentDir + " ~").Length);
                command = command.Trim();
            }
            else
                command = command.Trim();

            if (string.IsNullOrEmpty(command) || string.IsNullOrWhiteSpace(command))
                return;

            tokens = command.Split(' ');

            for (int i = 0; i < tokens.Length; ++i)
            {
                if (tokens[i] == "!!")
                {
                    string temp = tokens[i];
                    tokens[i] = GetLastCommand();
                    UpdateHistoryFile(temp);//Update history file
                }

                if (tokens[i].StartsWith("!") && tokens[i] != "!!")
                {
                    string temp = tokens[i];
                    int x = Convert.ToInt32(tokens[i].Substring(1));
                    tokens[i] = GetCommandFromIndex(x + 1);
                    UpdateHistoryFile(temp);//Update history file
                }
            }

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
                        if (helper == null)
                            helper = new Helper();
                        
                        helper.DisplayHelp();
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
                        AddCommandToConsole(tokens);
                        if (wand == null)
                            wand = new WandEditor();
                        wand.PeekFile(tokens);
                        break;
                    case true when cmds[i].StartsWith("wand"):
                        AddCommandToConsole(tokens);
                        if (wand == null)
                            wand = new WandEditor();

                        if (tokens.Length == 2)
                            wand.LoadFile(tokens);
                        else if (tokens.Length == 3 && tokens[1] == "append")
                            wand.AppendText(tokens);
                        break;
                    case true when cmds[i].StartsWith("clear"):
                        if (tokens.Length == 1)
                            ClearConsole();
                        else if (tokens.Length == 2 && tokens[1] == "history")
                            ClearHistory();
                        break;
                    case true when cmds[i].StartsWith("history"):
                        AddCommandToConsole(tokens);
                        if (tokens.Length == 1)
                            PrintHistory();
                        break;
                    case true when cmds[i].StartsWith("exit"):
                        ExitShell();
                        break;
                    case true when cmds[i].StartsWith("shutdown"):
                        Shutdown();
                        break;
                    case true when cmds[i].StartsWith("listproc"):
                        AddCommandToConsole(tokens);
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
                        AddCommandToConsole(tokens);
                        CreateTokens(tokens);
                        break;
                    case true when cmds[i].StartsWith("batch"):
                        if (batch == null)
                            batch = new BatchInterpreter();
                        batch.ExecuteCommand(tokens);
                        break;
                    case true when cmds[i].StartsWith("system"):
                        AddCommandToConsole(tokens);
                        if (systemInfo == null)
                            systemInfo = new SystemInformation();

                        systemInfo = null;
                        break;
                    case true when cmds[i].StartsWith("script"):
                        if (tokens.Length == 2)
                        {
                            string[] scriptInput = cmds;
                            if (script == null)
                                script = new ScriptInterpreter(tokens[1]);

                            script = null; //delete object when script has been run
                            AddCommandToConsole(scriptInput);
                        }
                        else
                        {
                            AddTextToConsole("Incorrect ammount of parameters in command...");
                            return;
                        }
                        break;
                    case true when cmds[i].StartsWith("ftp"):
                        AddCommandToConsole(tokens);

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
                        AddCommandToConsole(tokens);

                        string fcolor = tokens[1].ToUpper();
                        coloring = new Coloring();
                        coloring.ColorForeground(fcolor);
                        if (tokens.Length == 3)
                            if (tokens[2] == "-s")
                                settings.SetSetting("coloring", "fcolor", fcolor);

                        SetInputPrefix();
                        break;
                    case true when cmds[i].StartsWith("bcolor"):
                        AddCommandToConsole(tokens);

                        string bcolor = tokens[1].ToUpper();
                        coloring = new Coloring();
                        coloring.ColorBackground(bcolor);
                        if (tokens.Length == 3)
                            if (tokens[2] == "-s")
                                settings.SetSetting("coloring", "bcolor", bcolor);

                        SetInputPrefix();
                        break;
                    case true when cmds[i].StartsWith("ssh"):
                        AddCommandToConsole(tokens);

                        if (sshClient == null)
                            sshClient = new SSHClient();

                        if (tokens[0] == "sshCom" || tokens[0] == "sshcom")
                        {
                            StringBuilder sb = new StringBuilder();
                            for (int x = 1; x < tokens.Length; ++x)
                            {
                                sb.Append(tokens[x]);
                                if (x + 1 < tokens.Length)
                                    sb.Append(" ");
                            }
                            sshClient.SendCommand(sb.ToString());
                        }
                        else if (tokens[0] == "ssh" && tokens[1] == "close")
                            sshClient.TerminateConnection();
                        else if (tokens[0] == "ssh" && tokens[1] == "connect")
                            sshClient.EstablishConnection(tokens[2], tokens[3], tokens[4]);

                        break;
                    default:
                        AddTextToConsole("Command does not exist");
                        break;
                }
            }
        }

        bool firstClick = true;
        private void inputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(firstClick == true)
            {
                historyLen = cmdHistory.Length;
                historyIndex = historyLen - 1;
            }

            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                RunCommand(inputBox.Text, false);
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
                    inputBox.Text = string.Concat(GetInputPrefix(), " ", cmdHistory[historyIndex]);
                    inputBox.SelectionStart = inputBox.Text.Length;//Set cursor to right position

                    if(firstClick == true)
                        firstClick = false;
                }
                else if (historyIndex == 0) //This else if must be here so that you can press down to clear the history if you are at the end
                {
                    inputBox.Text = string.Concat(GetInputPrefix(), " ", cmdHistory[historyIndex]);
                    inputBox.SelectionStart = inputBox.Text.Length;
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
                    inputBox.Text = string.Concat(GetInputPrefix(), " ", cmdHistory[historyIndex]); 
                    inputBox.SelectionStart = inputBox.Text.Length;//Set cursor to right position
                    if (firstClick == true)
                        firstClick = false;
                }
                else if(historyIndex == 0 && firstClick == false) //This else if must be here so that you can press down to clear the history if you are at the end
                {
                    ++historyIndex;
                    inputBox.Text = string.Concat(GetInputPrefix(), " ", cmdHistory[historyIndex]);
                    inputBox.SelectionStart = inputBox.Text.Length;
                }
                else if (historyIndex == cmdHistory.Length - 1)
                {
                    ++historyIndex;
                    SetInputPrefix();
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
            else if (e.Control && e.KeyCode == Keys.D)
                wand.DuplicateLine();
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
            settings.jdf.WriteNewData(); //Save all the settings

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

            if (script != null)
                script = null;
        }
    }
}
