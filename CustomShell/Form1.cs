using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.IO.Compression;
using System.Collections;
using System.Drawing;

namespace CustomShell
{
    public partial class Form1 : Form
    {
        string currentDir = @"C:/";
        List<string> history = new List<string>();

        public string inputPrefix()
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
            if(!currentDir.Contains("\\"))
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

        public Form1()
        {
            InitializeComponent();
            InitConsole();
        }

        public void InitConsole()
        {
            outputBox.ScrollToCaret();
            inputBox.Text = string.Concat(Environment.UserName, "@", currentDir, " ~ ");
            inputBox.SelectionStart = inputBox.Text.Length;
            this.ActiveControl = inputBox;
        }

        public void AddCommandToConsole(string[] tokens)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < tokens.Length; ++i)
            {
                 sb.Append(tokens[i] + " ");
            }
            string command = sb.ToString();
            outputBox.AppendText(command + "\n");
            history.Add(command);

            inputBox.Text = inputPrefix(); //Clear input area
            inputBox.SelectionStart = inputBox.Text.Length;//Set cursor to right position
        }

        public void AddTextToConsole(string text)
        {
            outputBox.AppendText(text + "\n");
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
                        outputBox.Find(item.ToString());
                        if(attr.HasFlag(FileAttributes.Directory))
                            outputBox.SelectionColor = Color.Green;
                        else
                            outputBox.SelectionColor = Color.Red;

                        outputBox.Select(0, 0);
                    }
                }
                else if(tokens.Length == 2)
                {
                    AddCommandToConsole(tokens);

                    string path = CheckInputType(tokens);
                    IEnumerable entries = Directory.EnumerateFileSystemEntries(path);
                    foreach (string item in entries)
                    {
                        FileAttributes attr = File.GetAttributes(item.ToString());
                        AddTextToConsole(item.ToString());
                        outputBox.Find(item.ToString());

                        if (attr.HasFlag(FileAttributes.Directory))
                            outputBox.SelectionColor = Color.Green;
                        else
                            outputBox.SelectionColor = Color.Red;

                        outputBox.Select(0, 0);
                    }
                }
                else
                {
                    AddTextToConsole("Command is not valid");
                }
            }
            catch (Exception e)
            {
                if(e.InnerException is UnauthorizedAccessException)
                {
                    AddTextToConsole("Cannot access a directory. Please run shell as admin.");
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
                        AddTextToConsole("Cannot access a directory. Please run shell as admin.");
                        return;
                    }
                    else
                    {
                        AddTextToConsole("Error: make sure the filepath is valid");
                    }
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
                    if(tokens[1] == "-r")
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
                                AddTextToConsole("Cannot access a directory. Please run shell as admin.");
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
            if (tokens.Length != 3)
            {
                AddTextToConsole("Command not formatted correctly");
                return;
            }

            string input = tokens[1];
            string output = tokens[2];

            try
            {
                if (!File.Exists(output))
                    File.Copy(input, output);
                else
                    AddTextToConsole("Output file already exists");

                AddCommandToConsole(tokens);
            }
            catch (Exception e)
            {
                if (e.InnerException is UnauthorizedAccessException)
                {
                    AddTextToConsole("Cannot access a directory. Please run shell as admin.");
                    return;
                }
            }
        }

        public void DisplayHelp()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("() means optional parameter\n");
            sb.Append("cd [Path]\n");
            sb.Append("ls (Path)\n");
            sb.Append("mkdir [Path]\n");
            sb.Append("mkfile [Path]\n");
            sb.Append("cp [InputPath] (OutputPath)\n");
            sb.Append("mv [InputPath] [OutputPath]\n");
            sb.Append("rm [Path]\n");
            sb.Append("exec [PathToExe]\n");
            sb.Append("open [PathToFile]\n");
            sb.Append("clear\n");
            sb.Append("extr [PathToZip] (OutputFolder)\n");
            sb.Append("compr [Path] (OutputArchive)\n");
            sb.Append("size [Path]\n");
            sb.Append("help");

            AddTextToConsole(sb.ToString());
            inputBox.Text = inputPrefix();
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
        }

        public void ExtractArchive(string[] tokens)
        {
            if(tokens.Length == 3)
            {
                string inputPath = GetPathType(tokens[1]);
                string outputPath = GetPathType(tokens[2]);
                ZipFile.ExtractToDirectory(inputPath, outputPath);
            }
            else if(tokens.Length == 2)
            {
                string input = GetPathType(tokens[1]);
                string output;
                if (!tokens[1].Contains(@":\"))
                {
                    output = GetFullPathFromName(tokens[1]);
                    output = output.Substring(0, output.Length - 4);
                }
                else
                    output = tokens[1];

                ZipFile.ExtractToDirectory(input, output);
            }
            AddCommandToConsole(tokens);
        }

        public void CompressFolder(string[] tokens)
        {

            if (tokens.Length == 3)
            {
                string inputPath = GetPathType(tokens[1]);
                string outputPath = GetPathType(tokens[2]);
                ZipFile.CreateFromDirectory(inputPath, outputPath + ".zip");
            }
            else if (tokens.Length == 2)
            {
                string path;

                if (!tokens[1].Contains(@":\"))
                    path = GetFullPathFromName(tokens[1]);
                else
                    path = tokens[1];

                ZipFile.CreateFromDirectory(path, path + ".zip");
            }
            AddCommandToConsole(tokens);
        }

        public void DirectorySize(string[] tokens)
        {
            string path = CheckInputType(tokens);
            long size = 0;
            DirectoryInfo di = new DirectoryInfo(path);

            foreach (FileInfo fi in di.GetFiles("*", SearchOption.AllDirectories))
                size += fi.Length;

            string output = FormatBytes(size);

            AddCommandToConsole(tokens);
            AddTextToConsole(output);
        }
        
        public new void Move(string[] tokens)
        {
            if(tokens.Length != 3)
            {
                AddTextToConsole("Invalid command format...");
                return;
            }

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
                if(e.InnerException is UnauthorizedAccessException)
                {
                    AddTextToConsole("Cannot access a directory. Please run shell as admin.");
                    return;
                }
            }
        }
        #endregion

        int historyIndex = 0;
        private void inputBox_KeyDown(object sender, KeyEventArgs e)
        {
            string input = string.Empty;
            string[] tokens;

            //When command is entered
            if (e.KeyCode == Keys.Enter)
            {
                input = inputBox.Text;
                string command = input.Remove(0, (Environment.UserName + "@" + currentDir + " ~ ").Length);
                command = command.Trim();
                tokens = command.Split(' ');

                switch (tokens[0])
                {
                    case "cd":
                        ChangeDirectory(tokens);
                        break;
                    case "ls":
                        ListFiles(tokens);
                        break;
                    case "mkdir":
                        MakeDirectory(tokens);
                        break;
                    case "mkfile":
                        MakeFile(tokens);
                        break;
                    case "cp":
                        CopyFile(tokens);
                        break;
                    case "mv":
                        Move(tokens);
                        break;
                    case "rm":
                        RemoveFolder(tokens);
                        break;
                    case "help":
                        DisplayHelp();
                        break;
                    case "exec":
                        Execute(tokens);
                        break;
                    case "open":
                        OpenFile(tokens);
                        break;
                    case "clear":
                        ClearConsole();
                        break;
                    case "extr":
                        ExtractArchive(tokens);
                        break;
                    case "compr":
                        CompressFolder(tokens);
                        break;
                    case "size":
                        DirectorySize(tokens);
                        break;
                    default:
                        AddTextToConsole("Command does not exist");
                        break;

                }
            }

            if(e.KeyCode == Keys.Up)
            {
                if (historyIndex >= 0 && historyIndex < history.Count)
                {
                    inputBox.Text = string.Concat(inputPrefix(), " ", history[history.Count - historyIndex - 1]); //Clear input area
                    inputBox.SelectionStart = inputBox.Text.Length;//Set cursor to right position
                    ++historyIndex;
                }
            }

            if (e.KeyCode == Keys.Down)
            {
                if(historyIndex > 0 && historyIndex <= history.Count)
                {
                    inputBox.Text = string.Concat(inputPrefix(), " ", history[history.Count - historyIndex]); //Clear input area
                    inputBox.SelectionStart = inputBox.Text.Length;//Set cursor to right position
                    --historyIndex;
                }
            }
        }
    }
}
