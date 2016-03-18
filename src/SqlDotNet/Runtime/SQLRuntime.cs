using SqlDotNet.CLRInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Runtime
{
    /// <summary>
    /// SQL-Runtime core
    /// </summary>
    public class SCLRuntime
    {
        #region Private Member
        private Scope rootScope;
        #endregion

        #region Constructor
        /// <summary>
        /// Create SCL-Runtime
        /// </summary>
        /// <param name="parameter">List of available parameter</param>
        public SCLRuntime(IList<QueryParameter> parameter)
        {
            // create basic scope
            rootScope = new Scope(null);

            int unnamedCounter = 0;
            foreach (var _var in parameter)
            {
                string _varName = _var.Name;

                if (string.IsNullOrWhiteSpace(_varName))
                {
                    _varName = string.Format("__unnamed{0}", unnamedCounter);
                    unnamedCounter++;
                }

                var newVar = rootScope.CreateVariable(_varName);
                newVar.Value = _var.Value;
            }
        }
        #endregion

        #region Private Methods

        #region [Execute Command]
        /// <summary>
        /// Execute a single command, cann be classed recursive
        /// </summary>
        /// <param name="node">ChainNode instance (command)</param>
        /// <param name="scope">Current scope, in which the command is executed</param>
        private void ExecuteCommand(CommandChainNode node, Scope scope)
        {
            if (node is OpenCursor)
            {
                var openCursorNode = (OpenCursor)node;

                
            }
        }
        #endregion

        #endregion

        #region Public Methods

        #region [Execute interface]
        /// <summary>
        /// Execute siql code
        /// </summary>
        public void Execute(Stream ilCode)
        {
            
        }

        /// <summary>
        /// Execute command chain node directly
        /// </summary>
        /// <param name="root">Root command chain item</param>
        internal void Execute(CommandChainNode root)
        {
            foreach (var node in root.Children)
            {
                ExecuteCommand(node, this.rootScope);
            }
        }
        #endregion

        #endregion

        #region Public Member

        #endregion
    }
}
