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

        List<Token> result = new List<Token>();
        public TreeGenerator()
        {

        }

        #region Nodes
        public struct NumberNode
        {
            public Token value;
        }

        public struct AddNode
        {
            public Token NodeA;
            public Token NodeB;
        }        
        
        public struct SubtractNode
        {
            public Token NodeA;
            public Token NodeB;
        }
        public struct MultiplyNode
        {
            public Token NodeA;
            public Token NodeB;
        }
        public struct DivideNode
        {
            public Token NodeA;
            public Token NodeB;
        }
        #endregion

        #region NodeGetters
        public string GetNumber(NumberNode number)
        {
            return number.value.ToString();
        }

        public double CalcAddNode(AddNode node)
        {
            return (double)(node.NodeA.value + node.NodeB.value);
        }

        public double CalcSubtractNode(SubtractNode node)
        {
            return (double)(node.NodeA.value - node.NodeB.value);
        }

        public double CalcMultiplyNode(MultiplyNode node)
        {
            return (double)(node.NodeA.value * node.NodeB.value);
        }

        public double CalcDivideNode(DivideNode node)
        {
            return (double)(node.NodeA.value / node.NodeB.value);
        }
        #endregion

        public void GenerateTree()
        {
            List<Token> tokenPipeline = pipeline;
            Token Ltoken;
            Token Ctoken;
            Token Rtoken;
            Ltoken.type = null;
            Ltoken.value = null;
            Ctoken.type = null;
            Ctoken.value = null;
            Rtoken.type = null;
            Rtoken.value = null;

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

                if (Ctoken.type == TokenType.Plus)
                {
                    AddNode node;
                    node.NodeA = Ltoken;
                    node.NodeB = Rtoken;
                    result.Add(CalcAddNode(node));
                }
                else if (Ctoken.type == TokenType.Minus)
                {
                    result.Add(SubtractNode((double)Ltoken.value, (double)Rtoken.value));
                }
            }

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


                if (Ctoken.type == TokenType.Multiply)
                {
                    result.Add(MultiplyNode((double)Ltoken.value, (double)Rtoken.value));
                }
                else if (Ctoken.type == TokenType.Divide)
                {
                    result.Add(DivideNode((double)Ltoken.value, (double)Rtoken.value));
                }
            }

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


                if (Ctoken.type == TokenType.Lparen)
                {
                    while (Ctoken.type != TokenType.Rparen)
                    {
                        continue;
                    }
                    result.Add(MultiplyNode((double)Ltoken.value, (double)Rtoken.value));
                }
                else if (Ctoken.type == TokenType.Divide)
                {
                    result.Add(DivideNode((double)Ltoken.value, (double)Rtoken.value));
                }
            }
        }
    }
}
