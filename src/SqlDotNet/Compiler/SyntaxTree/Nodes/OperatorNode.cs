using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Compiler
{
    /// <summary>
    /// Association of an token in the value-parser
    /// </summary>
    public enum OperatorAssociation
    {
        None = 0,
        Left = 1,
        Right = 2
    }

    /// <summary>
    /// Type of operators
    /// </summary>
    public enum OperatorType
    {
        Add = 0,
        Sub = 1,
        Mul = 2,
        Div = 3,

        Equal = 5,
        Unequal = 6,
        Greater = 7,
        Smaller = 8,
        GreaterEqual = 9,
        SmallerEqual = 10,
        In = 14,

        And = 11,
        Or = 12
    }

    public class OperatorNode : SyntaxTreeNode
    {
        #region Private Member
        private OperatorType type;
        private OperatorAssociation association;
        private int precedence;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Parent"></param>
        public OperatorNode(SyntaxTreeNode Parent, RawToken token)
            : base(Parent, SyntaxNodeType.Operator, token)
        {

        }
        #endregion

        public override void CheckSemantic()
        {

        }

        #region Public Member
        /// <summary>
        /// Precedence
        /// </summary>
        public int Precedence
        {
            get { return precedence; }
            set { precedence = value; }
        }

        /// <summary>
        /// Get and set the type of the operator, automatically set the precedence
        /// </summary>
        public OperatorType OperatorType
        {
            get
            {
                return type;
            }
            set
            {
                type = value;

                switch (value)
                {
                    case OperatorType.Mul:
                        precedence = 3;
                        break;
                    case OperatorType.Div:
                        precedence = 3;
                        break;
                    case OperatorType.Add:
                        precedence = 2;
                        break;
                    case OperatorType.Sub:
                        precedence = 2;
                        break;

                    case OperatorType.Equal:
                        precedence = 1;
                        break;
                    case OperatorType.Unequal:
                        precedence = 1;
                        break;
                    case OperatorType.Greater:
                        precedence = 1;
                        break;
                    case OperatorType.Smaller:
                        precedence = 1;
                        break;
                    case OperatorType.GreaterEqual:
                        precedence = 1;
                        break;
                    case OperatorType.SmallerEqual:
                        precedence = 1;
                        break;
                    case OperatorType.In:
                        precedence = 1;
                        break;
                    case OperatorType.And:
                        precedence = 0;
                        break;
                    case OperatorType.Or:
                        precedence = 0;
                        break;
                }
            }
        }

        /// <summary>
        /// Operator association
        /// </summary>
        public OperatorAssociation Association
        {
            get { return association; }
            set { association = value; }
        }

        /// <summary>
        /// Get debug text
        /// </summary>
        public override string DebugText
        {
            get
            {
                return "Operator (" + this.Token.Content + ")";
            }
        }
        #endregion
    }
}
