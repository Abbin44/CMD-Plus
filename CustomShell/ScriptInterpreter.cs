using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace CustomShell
{
    class ScriptInterpreter
    {
        MainController main = MainController.controller;

        DataTable dt = new DataTable();

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
            public string name;
            public int line;
            public int? endLine;
        }

        private struct ENDIF
        {
            public string name;
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

            if (!filePath.EndsWith(".rls"))
            {
                main.AddTextToConsole("Not a .rls file...");
                return;
            }

            try
            {
                lines = File.ReadAllLines(filePath);
                file = File.ReadAllLines(filePath);
                CreateTokens();
            }
            catch (Exception)
            {
                main.AddTextToConsole("File could not be found...");
                return;
            }

        }

        private void CreateTokens()
        {
            if (!lines[0].Equals("[SCRIPT]"))
                return;

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
                    iF.name = tokens[1];
                    iF.statement = string.Concat(tokens[3], " ",tokens[4], " ", tokens[5]);
                    iF.endLine = null;
                    for (int x = 0; x < endifs.Count; ++x)//Check if an if statement can be paired to an end line
                    {
                        if (iF.name == endifs[x].name)
                            iF.endLine = endifs[x].line;
                    }


                    ifs.Add(iF);
                }
                else if (lines[i].Contains("endif"))
                {
                    ENDIF endif;
                    endif.name = tokens[1];
                    endif.line = i;
                    endif.startLine = null;
                    for (int x = 0; x < ifs.Count; ++x)
                    {
                        if (tokens[1] == ifs[x].name)
                        {
                            endif.startLine = ifs[x].line;
                            ifs[x] = new IF { endLine = endif.line, line = ifs[x].line, name = ifs[x].name, statement = ifs[x].statement }; //Create a new copy of the if struct with a modified end line
                        }
                    }
                    endifs.Add(endif);
                }
            }
            HandleComments();
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
                HandleComments();

                for (int j = 0; j < tokens.Length; ++j)
                {
                    for (int l = 0; l < nums.Count; ++l)
                    {
                        if (tokens[j].Equals(nums[l].name) && lines[i].Contains("if")) //If line contains both "variable name" and "if"
                        {
                            tokens[j] = nums[l].value.ToString(); //Replace variable names with the appropriate number
                            break;
                        }

                        if (tokens[j].Equals(nums[l].name + ".value") && lines[i].StartsWith("print"))
                        {
                            tokens[j] = nums[l].value.ToString();
                            break;
                        }
                    }
                }

                lines[i] = string.Join(" ", tokens); //Insert the modified string back into the main file array

                if (lines[i].StartsWith("goto"))
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
                            if (ifs[z].line == i && ifs[z].name == tokens[1])
                            {
                                int l = GetEndifIndex(tokens[1]);
                                i = endifs[l].line;
                                continue;
                            }
                        }
                    }
                }
                else if(tokens[0] == "print")
                {
                    string output = string.Join(" ", tokens, 1, tokens.Length - 1);
                    main.AddTextToConsole(output);
                }
                else if (lines[i].StartsWith("[END]"))
                    return;
                else if (!lines[i].StartsWith("NUM") && !lines[i].StartsWith("label") && !lines[i].StartsWith("[SCRIPT]") && !lines[i].Contains("endif") && !lines[i].StartsWith("print") && LineContainsVariable(lines[i]) == false)//If line doesn't contain a keyword, run it as a command
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

        private void HandleComments()
        {
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("#"))
                {
                    int commentIndex = 0;
                    commentIndex = lines[i].IndexOf("#");

                    if (commentIndex == 0)
                        file[i] = "";
                    else if(commentIndex > 0)
                        file[i] = file[i].Substring(0, commentIndex);

                    file.CopyTo(lines, 0);
                }
            }
        }

        private bool isValidStatement(string[] statement)
        {
            bool valid = false;

            switch (statement[4])
            {
                case "==":
                    if (statement[3] == statement[5])
                        valid = true;
                    break;
                case "<":
                    if (Convert.ToSingle(statement[3]) < Convert.ToSingle(statement[5]))
                        valid = true;
                    break;
                case "<=":
                    if (Convert.ToSingle(statement[3]) <= Convert.ToSingle(statement[5]))
                        valid = true;
                    break;
                case ">":
                    if (Convert.ToSingle(statement[3]) > Convert.ToSingle(statement[5]))
                        valid = true;
                    break;
                case ">=":
                    if (Convert.ToSingle(statement[3]) >= Convert.ToSingle(statement[5]))
                        valid = true;
                    break;
                case "!=":
                    if (statement[3] != statement[5])
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
            if (!IsNumVar(tokens[0]))
                return;

            int numIndex = GetNumIndex(tokens[0]);

            //Check for var++ and var--
            if (IsNumVar(tokens[0]) && tokens.Length == 1 && tokens[0].EndsWith("++"))
            {
                ChangeNUMValue(nums[numIndex], numIndex, true, 1.0f);
                return;
            }
            else if (IsNumVar(tokens[0]) && tokens.Length == 1 && tokens[0].EndsWith("--"))
            {
                ChangeNUMValue(nums[numIndex], numIndex, false, 1.0f);
                return;
            }

            for (int i = 0; i < tokens.Length; ++i) //Skip first two tokens since they are not part of the new value
            {
                if (IsNumVar(tokens[i]))
                    tokens[i] = nums[GetNumIndex(tokens[i])].value.ToString();
            }

            string equation = string.Empty;

            if(tokens[1] == "=")
                equation = string.Join(" ", tokens, 2, tokens.Length - 2);
            else
                equation = string.Join(" ", tokens);

            float answer = Convert.ToSingle(dt.Compute(equation, ""));
            nums[numIndex] = new NUM { name = nums[numIndex].name, value = answer };

            file.CopyTo(lines, 0); //Reset the lines array to default
        }

        private bool IsNumVar(string name)
        {
            bool isVar = false;
            for (int i = 0; i < nums.Count; ++i)
            {
                if (name == nums[i].name || name == nums[i].name + "++" || name == nums[i].name + "--")
                    isVar = true;
            }
            return isVar;
        }

        private float GetNumValue(string name)
        {
            for (int i = 0; i < nums.Count; ++i)
            {
                if (name == nums[i].name || name == nums[i].name + "++" || name == nums[i].name + "--")
                    return nums[i].value;
            }
            return 0.0f; //This should never be reached since a check for IsNumVar should always have happened before calling this
        }

        private int GetNumIndex(string name)
        {
            int index = 0;
            for (int i = 0; i < nums.Count; ++i)
            {
                if (name == nums[i].name)
                    index = i;
            }

            return index;
        }

        private int GetEndifIndex(string name)
        {
            int index = 0;
            for (int i = 0; i < endifs.Count; ++i)
            {
                if (endifs[i].name == name)
                    index = i;
            }
            return index;
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