using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CustomShell.TreeGenerator;

namespace CustomShell
{
    class Interpreter
    {
        public Interpreter()
        {
            InterpretNodeTree();
        }

        double result;

        struct Number
        {
            public double value;
        }

        #region NodeGetters
        public double GetNumber(NumberNode number)
        {
            return (double)number.number.value;
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

        public double CalcPlusNode(PlusNode node)
        {
            return (double)+node.token.value;
        }

        public double CalcMinusNode(MinusNode node)
        {
            return (double)-node.token.value;
        }
        #endregion

        public void InterpretNodeTree()
        {
            for (int i = nodeTree.Count - 1; i >= 0; --i)//Start from the back so order of operations are correct
            {
                //if (nodeTree[i].GetType() == typeof(NumberNode))
                //     result += GetNumber((NumberNode)nodeTree[i]);
                if (nodeTree[i].GetType() == typeof(AddNode))
                    result += CalcAddNode((AddNode)nodeTree[i]);
                else if (nodeTree[i].GetType() == typeof(SubtractNode))
                    result -= CalcSubtractNode((SubtractNode)nodeTree[i]);
                else if (nodeTree[i].GetType() == typeof(MultiplyNode))
                    result += CalcMultiplyNode((MultiplyNode)nodeTree[i]);
                else if (nodeTree[i].GetType() == typeof(DivideNode))
                    result += CalcDivideNode((DivideNode)nodeTree[i]);
                else if (nodeTree[i].GetType() == typeof(PlusNode))
                    result += CalcPlusNode((PlusNode)nodeTree[i]);
                else if (nodeTree[i].GetType() == typeof(MinusNode))
                    result += CalcMinusNode((MinusNode)nodeTree[i]);
            }
            MainController.controller.outputBox.AppendText(result.ToString());
            nodeTree.Clear(); //Clear nodes to prepare for next input
            Calculator.pipeline.Clear();
        }
    }
}
