using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomShell
{
    class Calculator
    {
        MainController main = MainController.controller;
        public Calculator()
        {

        }

        public void CalculateInput(string[] calculation)
        {
            string[] tokens = SplitCalculation(calculation);
            double result = 0f;
            //First loop to check for * and / to follow BEMDAS
            for (int i = 1; i < tokens.Length; i++)
            {
                if (tokens[i - 1] == "*")
                {
                    if (result == 0)
                        result = Convert.ToDouble(tokens[i]);
                    else
                        result *= Convert.ToDouble(tokens[i]);
                }
                else if (tokens[i - 1] == "/")
                {
                    if (result == 0)
                        result = Convert.ToDouble(tokens[i]);
                    else
                        result /= Convert.ToDouble(tokens[i]);
                }
            }

            //Second loop after checking for * and / to follow BEMDAS
            for (int i = 1; i < tokens.Length; i++)
            {
                if (tokens[i - 1] == "-")
                {
                    if (result == 0)
                        result = Convert.ToDouble(tokens[i]);
                    else
                        result -= Convert.ToDouble(tokens[i]);
                }
                else if (tokens[i - 1] == "+")
                {
                    if(result == 0)
                        result = Convert.ToDouble(tokens[i]);
                    else
                        result += Convert.ToDouble(tokens[i]);
                }
            }

            main.AddCommandToConsole(tokens);
            main.AddTextToConsole(result.ToString());
        }

        public string[] SplitCalculation(string[] tokens)
        {
            string[] output = null;
            if (tokens.Length == 2)
            {
                char[] ops = tokens[1].ToCharArray();
                output = new string[ops.Length];

                for (int i = 0; i < ops.Length; i++)
                    output[i] = ops[i].ToString();
            }
            else
                output = tokens;

            return output;
        }
    }
}
