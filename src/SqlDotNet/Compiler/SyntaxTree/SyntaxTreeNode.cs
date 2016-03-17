using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Compiler
{
    /// <summary>
    /// Baseic syntax tree node
    /// </summary>
    public abstract class SyntaxTreeNode
    {
        #region Private Member
        private SyntaxTreeNode parentNode;
        private Queue<SyntaxTreeNode> children;
        private SyntaxNodeType nodeType;
        private RawToken token;
        #endregion

        #region Constructor
        /// <summary>
        /// Create SyntaxTreeNode
        /// </summary>
        /// <param name="parentNode">Parent-Node</param>
        /// <param name="nodeType">SyntaxTreeNode-Type</param>
        /// <param name="token">Raw token</param>
        public SyntaxTreeNode(SyntaxTreeNode parentNode, SyntaxNodeType nodeType, RawToken token)
        {
            children = new Queue<SyntaxTreeNode>();
            this.parentNode = parentNode;
            this.nodeType = nodeType;
            this.token = token;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Create new child-node
        /// </summary>
        /// <typeparam name="T">Generic Node-Type</typeparam>
        /// <returns>Instance of an SyntaxTreeNode</returns>
        public T CreateChildNode<T>(RawToken token) where T : SyntaxTreeNode
        {
            T returnValue = (T)Activator.CreateInstance(typeof(T), this, token);

            // Add the new node to the current child-list
            this.Children.Enqueue(returnValue);

            return returnValue;
        }

        /// <summary>
        /// Create new child-node, and pass symbol table
        /// </summary>
        /// <typeparam name="T">Generic Node-Type</typeparam>
        /// <returns>Instance of an SyntaxTreeNode</returns>
        public T CreateChildNode<T>(RawToken token, SymbolTable table) where T : SyntaxTreeNode
        {
            T returnValue = (T)Activator.CreateInstance(typeof(T), this, token, table);

            // Add the new node to the current child-list
            this.Children.Enqueue(returnValue);

            return returnValue;
        }

        /// <summary>
        /// Get all children of a specific type
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>List with all children</returns>
        public IList<T> FindChildrenOfType<T>()
        {
            return children.Where(Item => Item is T).OfType<T>().ToList();
        }

        /// <summary>
        /// Get all childres which are NOT of a specifig type
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>List with all children</returns>
        public IList<SyntaxTreeNode> FindChildrenBesidesOfType<T>()
        {
            return children.Where(Item => (Item is T) == false).ToList();
        }

        /// <summary>
        /// Get al children of specific TreeNodeType
        /// </summary>
        /// <param name="nodeType">Node-Type</param>
        /// <returns>List with all children</returns>
        public IList<SyntaxTreeNode> FindChildrenOfType(SyntaxNodeType nodeType)
        {
            return children.Where(Item => Item.NodeType == nodeType).ToList();
        }

        /// <summary>
        /// Proof node semantic
        /// </summary>
        public abstract void CheckSemantic();
        #endregion

        #region Public Member
        /// <summary>
        /// Contains all syntax children of this node
        /// </summary>
        public Queue<SyntaxTreeNode> Children
        {
            get { return children; }
        }

        /// <summary>
        /// Parent of this Node
        /// </summary>
        public SyntaxTreeNode ParentNode
        {
            get { return parentNode; }
            set { parentNode = value; }
        }

        /// <summary>
        /// Type of the current tree-node
        /// </summary>
        public SyntaxNodeType NodeType
        {
            get { return nodeType; }
        }

        /// <summary>
        /// Get debug text
        /// </summary>
        public abstract string DebugText
        {
            get;
        }

        /// <summary>
        /// Token
        /// </summary>
        public RawToken Token
        {
            get { return token; }
        }
        #endregion
    }
}