using System;
using System.IO.Compression;

namespace CustomShell
{
    class Compression
    {
        MainController main = MainController.controller;
        ProgressBar progress = new ProgressBar();
        public Compression()
        {

        }

        public void ExtractArchive(string[] tokens)
        {
            try
            {
                main.AddTextToConsole("Extracting archive, please wait...");
                if (tokens.Length == 3)
                {
                    string inputPath = main.GetPathType(tokens[1]);
                    string outputPath = main.GetPathType(tokens[2]);
                    ZipFile.ExtractToDirectory(inputPath, outputPath);
                }
                else if (tokens.Length == 2)
                {
                    string input = main.GetPathType(tokens[1]);
                    string output;
                    if (!tokens[1].Contains(@":\"))
                    {
                        output = main.GetFullPathFromName(tokens[1]);
                        output = output.Substring(0, output.Length - 4);
                    }
                    else
                        output = tokens[1];

                    ZipFile.ExtractToDirectory(input, output);
                }
                main.AddCommandToConsole(tokens);
            }
            catch (Exception)
            {
                main.AddTextToConsole("Something went wrong...");
                return;
            }

        }

        public void CompressFolder(string[] tokens)
        {
            try
            {
                main.AddTextToConsole("Compressing folder into archive, please wait...");
                if (tokens.Length == 3)
                {
                    string inputPath = main.GetPathType(tokens[1]);
                    string outputPath = main.GetPathType(tokens[2]);
                    ZipFile.CreateFromDirectory(inputPath, outputPath + ".zip");
                }
                else if (tokens.Length == 2)
                {
                    string path = main.GetPathType(tokens[1]);
                    ZipFile.CreateFromDirectory(path, path + ".zip");
                }
                main.AddCommandToConsole(tokens);
            }
            catch (Exception)
            {
                main.AddTextToConsole("Something went wrong...");
                return;
            }
        }
    }
}
