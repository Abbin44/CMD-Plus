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
        List<Token> pipeline = new List<Token>();
        public Calculator()
        {

        }

        public enum TokenType
        {
            Number   = 0,
            Plus     = 1,
            Minus    = 2,
            Multiply = 3,
            Divide   = 4,
            Lparen   = 5,
            Rparen   = 6
        }

        struct Token
        {
            public TokenType type;
            public double value;
        }

        public void CreateTokens(string[] tokens)
        {
            StringBuilder sb = new StringBuilder();
            int decimalCount = 0;
            for (int i = 1; i < tokens.Length; ++i)//Merge the shells input tokens into one long string
                sb.Append(tokens[i]);

            string input = sb.ToString();
            char[] calculation = input.ToCharArray();
            string number = string.Empty;
            char currentChar = calculation[0];

            for (int i = 0; i < calculation.Length; i++)//Loop through the input string and build up the needed tokens
            {
                currentChar = calculation[i];

                if (char.IsWhiteSpace(currentChar)) //If there's a whitespace, move on
                    continue;
                else if (char.IsDigit(currentChar) || currentChar == '.')//If we find a number check for trailing numbers and build it up
                {
                    while (char.IsDigit(currentChar) || currentChar == '.')
                    {
                        if (decimalCount > 1)
                            break;

                        if (number.StartsWith("."))             
                            number = string.Concat("0", number);//Adds a 0 to the beginning of a number if it starts with a .
                        else if (number.EndsWith("."))          
                            number = string.Concat(number, "0");//Adds a 0 to the end of the number if it ends with .

                        number += currentChar.ToString();

                        if (i + 1 == calculation.Length || !char.IsDigit(calculation[i + 1]))//Avoid out of bounds
                            break;
                        else
                            ++i;

                        if (currentChar == '.')
                            ++decimalCount;

                        currentChar = calculation[i];
                    }
                    Token token;
                    token.type = TokenType.Number;
                    token.value = Convert.ToDouble(number);
                    pipeline.Add(token);
                    number = string.Empty;
                }
                else if (currentChar == '+')
                {
                    Token token;
                    token.type = TokenType.Plus;
                    token.value = 0;
                    pipeline.Add(token);
                }
                else if (currentChar == '-')
                {
                    Token token;
                    token.type = TokenType.Minus;
                    token.value = 0;
                    pipeline.Add(token);
                }
                else if (currentChar == '*')
                {
                    Token token;
                    token.type = TokenType.Multiply;
                    token.value = 0;
                    pipeline.Add(token);
                }
                else if (currentChar == '/')
                {
                    Token token;
                    token.type = TokenType.Divide;
                    token.value = 0;
                    pipeline.Add(token);
                }
                else if (currentChar == '(')
                {
                    Token token;
                    token.type = TokenType.Lparen;
                    token.value = 0;
                    pipeline.Add(token);
                }
                else if (currentChar == ')')
                {
                    Token token;
                    token.type = TokenType.Rparen;
                    token.value = 0;
                    pipeline.Add(token);
                }
                else
                {
                    main.AddTextToConsole(string.Concat("Illegal character", currentChar));
                    return;
                }
            }
            PrintOutput();
        }

        public void PrintOutput()
        {
            for (int i = 0; i < pipeline.Count; i++)
                main.AddTextToConsole(pipeline[i].ToString());
        }
    }
}
