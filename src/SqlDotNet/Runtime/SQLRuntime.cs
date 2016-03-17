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
        public SCLRuntime()
        {
            // create basic scope
            rootScope = new Scope(null);
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

        }
        #endregion

        #endregion

        #region Public Methods

        #region [Execute interface]
        /// <summary>
        /// Execute siql code
        /// </summary>
        public void Execute(Stream ilCode, IList<QueryParameter> parameter)
        {
            
        }

        /// <summary>
        /// Execute command chain node directly
        /// </summary>
        /// <param name="root">Root command chain item</param>
        /// <param name="parameter">List of available parmaeter</param>
        internal void Execute(CommandChainNode root, IList<QueryParameter> parameter)
        {

        }
        #endregion

        #endregion

        #region Public Member

        #endregion
    }
}
