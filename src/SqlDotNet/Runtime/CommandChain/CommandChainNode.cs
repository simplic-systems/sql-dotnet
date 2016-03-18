using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Runtime
{
    /// <summary>
    /// Command-Chain node, every CommandNode must inherit from this node
    /// </summary>
    public abstract class CommandChainNode
    {
        #region Private Member
        private CommandChainNode parent;
        private IList<CommandChainNode> children;
        #endregion

        #region Constructor
        /// <summary>
        /// Create the parser
        /// </summary>
        /// <param name="parent">Parent node</param>
        public CommandChainNode(CommandChainNode parent)
        {
            this.parent = parent;
            children = new List<CommandChainNode>();
        }
        #endregion

        #region Private Methods

        #endregion

        #region Public Methods
        /// <summary>
        /// Create new child node
        /// </summary>
        /// <typeparam name="T">Type of the node</typeparam>
        /// <returns>Instance of a CommandCainNode</returns>
        public T CreateNode<T>() where T : CommandChainNode
        {
            T newNode = (T)Activator.CreateInstance(typeof(T), this);
            children.Add(newNode);
            return newNode;
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
        public IList<CommandChainNode> FindChildrenBesidesOfType<T>()
        {
            return children.Where(Item => (Item is T) == false).ToList();
        }
        #endregion

        #region Public Member
        /// <summary>
        /// Child nodes
        /// </summary>
        internal IList<CommandChainNode> Children
        {
            get { return children; }
        }

        /// <summary>
        /// Parent node
        /// </summary>
        internal CommandChainNode Parent
        {
            get { return parent; }
        }
        #endregion
    }
}
