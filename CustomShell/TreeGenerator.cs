using System.Collections.Generic;
using static CustomShell.Calculator;

namespace CustomShell
{
    static class TreeGenerator
    {
        /*NOTES
         * 
         * Expression looks for + and -
         * Term is one of the two values that is getting operated 
         * 
         * */

        public static List<object> nodeTree = new List<object>();

        #region Nodes
        public struct NumberNode
        {
            public Token number;
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

        public struct LparenNode
        {
            public Token Lparen;
        }

        public struct RparnNode
        {
            public Token Rparen;
        }

        public struct PlusNode
        {
            public Token token;
        }

        public struct MinusNode
        {
            public Token token;
        }
        #endregion


        public static void GenerateTree()
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
                    nodeTree.Add(node);
                }
                else if (Ctoken.type == TokenType.Minus)
                {
                    SubtractNode node;
                    node.NodeA = Ltoken;
                    node.NodeB = Rtoken;
                    nodeTree.Add(node);
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
                    MultiplyNode node;
                    node.NodeA = Ltoken;
                    node.NodeB = Rtoken;
                    nodeTree.Add(node);
                }
                else if (Ctoken.type == TokenType.Divide)
                {
                    DivideNode node;
                    node.NodeA = Ltoken;
                    node.NodeB = Rtoken;
                    nodeTree.Add(node);
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
                    LparenNode node;
                    node.Lparen = Ctoken;
                    nodeTree.Add(node);
                }
                else if (Ctoken.type == TokenType.Rparen)
                {
                    RparnNode node;
                    node.Rparen = Ctoken;
                    nodeTree.Add(node);
                }
                else if(Ctoken.type == TokenType.Plus)
                {
                    PlusNode node;
                    node.token = Ctoken;
                    nodeTree.Add(node);
                }
                else if (Ctoken.type == TokenType.Minus)
                {
                    MinusNode node;
                    node.token = Ctoken;
                    nodeTree.Add(node);
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

                if (Ctoken.type == TokenType.Number)
                {
                    NumberNode node;
                    node.number = Ctoken;
                    nodeTree.Add(node);
                }
            }
            Interpreter interpret = new Interpreter();
        }
    }
}
