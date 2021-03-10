using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Linq;
using static CustomShell.MainController;
namespace CustomShell
{
    class LocalDirectory
    {
        public LocalDirectory()
        {

        }

        private string ExtractFilePath(string input)
        {
            string output = string.Empty;
            char[] text = input.ToCharArray();
            for (int i = input.Length - 1; i > 0; --i)
            {
                if(text[i] == '\\' && text[i - 1] == ':') //Start of a filepath
                {
                    output = input.Substring(i - 2);//Get only the filepath in a substring
                    break;
                }
            }
            return output;
        }

        public void GetAllCurrentDir(string currentInput, string currentDir)
        {
            string command = currentInput.Remove(0, (Environment.UserName + "@" + currentDir + " ~").Length);//Remove the prefix from the input line
            command = command.Trim();

            string path = ExtractFilePath(command);
            if (!string.IsNullOrEmpty(path))
            {
                IEnumerable entries = Directory.EnumerateFileSystemEntries(path);
                foreach (string item in entries)
                {
                    FileAttributes attr = File.GetAttributes(item.ToString());
                    controller.AddTextToConsole(item.ToString());
                    controller.outputBox.Select(controller.outputBox.Text.Length - item.Length - 1, controller.outputBox.Text.Length);

                    if (attr.HasFlag(FileAttributes.Directory))
                        controller.outputBox.SelectionColor = Color.Green;
                    else
                        controller.outputBox.SelectionColor = Color.Red;
                }
            }
            controller.outputBox.SelectionStart = controller.outputBox.TextLength;
        }
    }
}
