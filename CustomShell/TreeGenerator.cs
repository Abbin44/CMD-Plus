using System.Collections.Generic;
using static CustomShell.Calculator;

namespace CustomShell
{
    class TreeGenerator
    {
        /*NOTES
         * 
         * Expression looks for + and -
         * Term is one of the two values that is getting operated 
         * 
         * */

        List<double> result = new List<double>();
        public TreeGenerator()
        {

        }

        #region NodeGetters
        public string GetNumber(Token number)
        {
            return number.value.ToString();
        }

        public double AddNode(double NodeA, double NodeB)
        {
            return (double)(NodeA + NodeB);
        }

        public double SubtractNode(double NodeA, double NodeB)
        {
            return (double)(NodeA - NodeB);
        }

        public double MultiplyNode(double NodeA, double NodeB)
        {
            return (double)(NodeA * NodeB);
        }

        public double DivideNode(double NodeA, double NodeB)
        {
            return (double)(NodeA / NodeB);
        }
        #endregion

        public void GenerateTree()
        {
            List<Token> tokenPipeline = pipeline;
            Token Ltoken;
            Token Ctoken;
            Token Rtoken;
            for (int i = 0; i < tokenPipeline.Count; i++)
            {
                if (!(i - 1 < 0))
                {
                    Ltoken.type = tokenPipeline[i - 1].type;
                    Ltoken.value = tokenPipeline[i - 1].value;
                }
                else
                    Ltoken.value = null;

                Ctoken.type = tokenPipeline[i].type;
                Ctoken.value = tokenPipeline[i].value;

                if (i + 1 != tokenPipeline.Count)
                {
                    Rtoken.type = tokenPipeline[i + 1].type;
                    Rtoken.value = tokenPipeline[i + 1].value;

                }
                else
                    Rtoken.value = null;

                while (Ctoken.type == TokenType.Multiply || Ctoken.type == TokenType.Divide)
                {
                    if (Ctoken.type == TokenType.Multiply)
                    {
                        result.Add(MultiplyNode((double)Ltoken.value, (double)Rtoken.value));
                    }
                    else if (Ctoken.type == TokenType.Divide)
                    {
                        result.Add(DivideNode((double)Ltoken.value, (double)Rtoken.value));
                    }
                }

                while (Ctoken.type == TokenType.Plus || Ctoken.type == TokenType.Minus)
                {
                    if (Ctoken.type == TokenType.Plus)
                    {
                        result.Add(AddNode((double)Ltoken.value, (double)Rtoken.value));
                    }
                    else if (Ctoken.type == TokenType.Minus)
                    {
                        result.Add(SubtractNode((double)Ltoken.value, (double)Rtoken.value));
                    }
                }
            }
        }
    }
}
