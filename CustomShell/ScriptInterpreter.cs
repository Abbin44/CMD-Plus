using System;
using System.Collections.Generic;
using System.IO;

namespace CustomShell
{
    class ScriptInterpreter
    {
        MainController main = MainController.controller;

        string[] lines;
        string[] file;
        List<NUM> nums = new List<NUM>();
        List<LABEL> labels = new List<LABEL>();
        List<GOTO> jumps = new List<GOTO>();
        List<IF> ifs = new List<IF>();
        List<ENDIF> endifs = new List<ENDIF>();

        /*  TINY DOCUMENTATION
         * 
         *  The variables value is assigned in the script file and is converted into a NUM token with it's value.
         *  If the interpreter finds a call to change it's value, the script file will remain the same, but the
         *  Tokens value will be changed. The file is only ever read to create the initial string array of lines.
         *  The "File" array is a constant list that always contain the original script lines while "Lines" contain
         *  Modified text, such as variable names getting replaced by numbers.
         */

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
            ReadScriptFile(filePath);
        }

        private void ReadScriptFile(string filePath)
        {
            filePath = main.GetPathType(filePath);

            if (!filePath.EndsWith(".srp"))
                return;

            lines = File.ReadAllLines(filePath);
            file = File.ReadAllLines(filePath);
            CreateTokens();
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

                lines[i] = lines[i].Trim();
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
                else if (lines[i].StartsWith("if"))
                {
                    IF iF;
                    iF.line = i;
                    iF.statement = string.Concat(tokens[1], " ",tokens[2], " ", tokens[3]);
                    iF.index = ifIndex;
                    iF.endLine = null;
                    for (int x = 0; x < endifs.Count; ++x)//Check if an if statement can be paired to an end line
                    {
                        if (iF.index == endifs[x].index)
                            iF.endLine = endifs[x].line;
                    }


                    ifs.Add(iF);
                }
                else if (lines[i].Contains("endif"))
                {
                    ENDIF endif;
                    endif.line = i;
                    endif.index = endifIndex;
                    endif.startLine = null;
                    for (int x = 0; x < ifs.Count; ++x)
                    {
                        if (endif.index == ifs[x].index)
                        {
                            endif.startLine = ifs[x].line;
                            ifs[x] = new IF { endLine = endif.line, index = ifs[x].index, line = ifs[x].line, statement = ifs[x].statement }; //Create a new copy of the if struct with a modified end line
                        }
                    }
                    endifs.Add(endif);
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

                lines[i] = lines[i].Trim();
                string[] tokens = lines[i].Split();

                for (int j = 0; j < tokens.Length; ++j)
                {
                    for (int l = 0; l < nums.Count; ++l)
                    {
                        if (tokens[j].Equals(nums[l].name) && lines[i].Contains("if")) //If line contains both "variable name" and "if"
                        {
                            tokens[j] = nums[l].value.ToString(); //Replace variable names with the appropriate number
                            break;
                        }
                    }
                }

                lines[i] = string.Join(" ", tokens); //Insert the modified string back into the main file array

                if (lines[i].Contains("goto"))
                {
                    for (int x = 0; x < labels.Count; ++x)
                    {
                        if (labels[x].name == tokens[1]) //Set i to the line number of the label referenced by the goto statement
                            i = labels[x].lineNum;
                    }
                }
                else if (lines[i].StartsWith("if"))
                {
                    if (!isValidStatement(tokens)) //If the statement is not valid, jump to endif
                    {
                        for (int z = 0; z < ifs.Count; ++z)
                        {
                            if (ifs[z].line == i)
                            {
                                i = (int)ifs[z].endLine - 1;
                                continue;
                            }
                        }
                    }
                }
                else if (lines[i].Contains("[END]"))
                    return;
                else if (!lines[i].Contains("NUM") && !lines[i].Contains("label") && !lines[i].Contains("[SCRIPT]") && !lines[i].Contains("endif") && LineContainsVariable(lines[i]) == false)//If line doesn't contain a keyword, run it as a command
                    main.RunCommand(lines[i], true);

                CheckForVariableChange(tokens);
            }
        }

        private bool LineContainsVariable(string line)
        {
            bool containsVar = false;
            for (int i = 0; i < nums.Count; ++i)
            {
                if (line.Contains(nums[i].name)) //Line contains a variable
                    containsVar = true;
                else
                    containsVar = false;
            }
            return containsVar;
        }

        private bool isValidStatement(string[] statement)
        {
            bool valid = false;

            switch (statement[2])
            {
                case "==":
                    if (statement[1] == statement[3])
                        valid = true;
                    break;
                case "<":
                    if (Convert.ToSingle(statement[1]) < Convert.ToSingle(statement[3]))
                        valid = true;
                    break;
                case "<=":
                    if (Convert.ToSingle(statement[1]) <= Convert.ToSingle(statement[3]))
                        valid = true;
                    break;
                case ">":
                    if (Convert.ToSingle(statement[1]) > Convert.ToSingle(statement[3]))
                        valid = true;
                    break;
                case ">=":
                    if (Convert.ToSingle(statement[1]) >= Convert.ToSingle(statement[3]))
                        valid = true;
                    break;
                case "!=":
                    if (statement[1] != statement[3])
                        valid = true;
                    break;
                default:
                    valid = false;
                    break;
            }
            return valid;
        }

        private void CheckForVariableChange(string[] tokens)
        {
            for (int l = 0; l < tokens.Length; ++l) //Look for lines where a NUM variable changes in value, only +, -, ++, --
            {
                for (int j = 0; j < nums.Count; ++j)
                {
                    if (tokens[j].StartsWith(nums[j].name))
                    {
                        if (tokens.Length >= 2)
                        {
                            if (tokens[j + 1] == "+")
                            {
                                ChangeNUMValue(nums[j], j, true, Convert.ToSingle(tokens[2]));
                            }
                            else if (tokens[j + 1] == "-")
                            {
                                ChangeNUMValue(nums[j], j, false, Convert.ToSingle(tokens[2]));
                            }
                            else if (tokens[j + 1] == "=")
                            {
                                ChangeNUMValue(nums[j], j, false, Convert.ToSingle(tokens[2]));
                            }
                            return;
                        }
                        else if (tokens.Length == 1) 
                        {
                            if (tokens[j].EndsWith("++"))
                            {
                                ChangeNUMValue(nums[j], j, true, 1.0f);
                            }
                            else if (tokens[j].EndsWith("--"))
                            {
                                ChangeNUMValue(nums[j], j, false, 1.0f);
                            }
                            return;
                        }
                    }
                }
            }
        }

        private bool IsNumVar(string name)
        {
            bool isVar = false;
            for (int i = 0; i < nums.Count; ++i)
            {
                if (name == nums[i].name)
                    isVar = true;
            }
            return isVar;
        }

        private float GetNumValue(string name)
        {
            for (int i = 0; i < nums.Count; ++i)
            {
                if (name == nums[i].name)
                {
                    return nums[i].value;
                }
            }
            return 0.0f; //This should never be reached since a check for IsNumVar should always have happened before calling this
        }

        private void ChangeNUMValue(NUM number, int index, bool add, float change)
        {
            float newValue = 0.0f;
            if(add == true)
                newValue = number.value + change;
            else
                newValue = number.value - change;

            nums[index] = new NUM { name = number.name, value = newValue };
            file.CopyTo(lines, 0); //Reset the file each iteration
        }
    }
}