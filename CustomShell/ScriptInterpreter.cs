using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomShell
{
    class ScriptInterpreter
    {
        MainController main = MainController.controller;

        string[] lines;
        List<string> statements = new List<string>();
        List<string> pipe = new List<string>(); //List of commands to run and in the correct order
        List<NUM> nums = new List<NUM>();
        List<LABEL> labels = new List<LABEL>();
        List<GOTO> jumps = new List<GOTO>();
        List<IF> ifs = new List<IF>();
        List<ENDIF> endifs = new List<ENDIF>();


        private struct NUM
        {
            public float value;
            public string name;
        }

        private struct LABEL
        {
            public string name;
            public int lineNum;
        }

        private struct GOTO
        {
            public string name;
        }

        private struct IF
        {
            public string statement;
            public int line;
            public int? endLine;
            public int index;
        }

        private struct ENDIF
        {
            public int index;
            public int line;
            public int? startLine;
        }

        public ScriptInterpreter(string filePath)
        {
            AddStatements();
            ReadScriptFile(filePath);
        }

        private void AddStatements()
        {
            statements.Add("NUM");
            statements.Add("label");
            statements.Add("if");
            statements.Add("endif");
            statements.Add("goto");
        }

        private void ReadScriptFile(string filePath)
        {
            try
            {
                if (!main.IsFilePath(filePath))
                    filePath = main.GetFullPathFromName(filePath);

                if (!filePath.EndsWith(".srp"))
                    return;

                lines = File.ReadAllLines(filePath);
                CreateTokens();
            }
            catch (Exception e)
            {
                main.AddTextToConsole("Could't read script file.");
            }
        }

        private void CreateTokens()
        {
            if (!lines[0].Equals("[SCRIPT]"))
                return;

            int ifIndex = 0;
            int endifIndex = 0;
            for (int i = 0; i < lines.Length; ++i)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))//Skip all empty lines
                    continue;

                string[] tokens = lines[i].Split();
                if (lines[i].Contains("NUM"))
                {
                    NUM num;
                    num.name = tokens[1]; //Name
                    num.value = Convert.ToSingle(tokens[3]);
                    nums.Add(num);
                }
                else if (lines[i].Contains("label"))
                {
                    LABEL label;
                    label.name = tokens[1];
                    label.lineNum = i;
                    labels.Add(label);
                }
                else if (lines[i].Contains("goto"))
                {
                    GOTO jump;
                    jump.name = tokens[1];
                    jumps.Add(jump);
                }
                else if (lines[i].Contains("if"))
                {
                    IF iF;
                    iF.line = i;
                    iF.statement = string.Concat(tokens[1], tokens[2], tokens[3]);
                    iF.index = ifIndex;
                    iF.endLine = null;
                    for (int x = 0; x < endifs.Count; ++x)//Check if an if statement can be paired to an end line
                        if (iF.index == endifs[x].index)
                            iF.endLine = endifs[x].line;

                    ifs.Add(iF);
                }
                else if (lines[i].Contains("endif"))
                {
                    ENDIF endif;
                    endif.line = i;
                    endif.index = endifIndex;
                    endif.startLine = null;
                    for (int x = 0; x < ifs.Count; ++x)
                        if (endif.index == ifs[x].index)
                            endif.startLine = ifs[x].line;

                    endifs.Add(endif);
                }
                else
                {
                    //TODO: Run line as a command
                }
            }
            InterpretTokens();
        }

        private void InterpretTokens()
        {
            for (int i = 0; i < lines.Length; ++i)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))//Skip all empty lines
                    continue;

                string[] tokens = lines[i].Split();

                for (int j = 0; j < nums.Count; ++j)
                    if (tokens[j].Equals(nums[j].name))
                        tokens[j] = nums[j].value.ToString(); //Replace variable names with the appropriate number

                lines[i] = tokens.ToString(); //Insert the modified string back into the main file array

                if (lines[i].Contains("goto"))
                {
                    for (int x = 0; x < labels.Count; ++x)
                        if (labels[x].name == tokens[1]) //Set i to the line number of the label referenced by the goto statement
                            i = labels[x].lineNum;
                }
                else if (lines[i].Contains("if"))
                {

                }
                else if (lines[i].Contains("endif"))
                {

                }
                else if (lines[i].Contains("[END]"))
                {
                    return;
                }
                else
                    main.RunCommand(lines[i], true);
            }
        }
    }
}