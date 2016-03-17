using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Runtime
{
    /// <summary>
    /// Variable
    /// </summary>
    internal class Argument
    {
        #region Private Member
        private string name;
        private object value;
        #endregion

        #region Constructor
        /// <summary>
        /// Crate variable and set name
        /// </summary>
        /// <param name="name">Name of the var</param>
        public Argument(string name)
        {
            this.name = name;
        }
        #endregion

        #region Private Methods

        #endregion

        #region Public Methods

        #endregion

        #region Public Member
        /// <summary>
        /// Get variable name
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Get set variable value
        /// </summary>
        public object Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        #endregion
    }
}
