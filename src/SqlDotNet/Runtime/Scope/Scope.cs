using Simplic.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Runtime
{
    /// <summary>
    /// Main runtime scope
    /// </summary>
    internal class Scope
    {
        #region Private Member
        private IDictionary<string, Variable> vars;
        private Scope parentScope;
        private CommandStack stack;
        private Dequeue<Tuple<int, StackItem>> argStack;
        #endregion

        #region Constructor
        /// <summary>
        /// Create new scope
        /// </summary>
        /// <param name="parent">Parent scope, can be null</param>
        public Scope(Scope parent)
        {
            vars = new Dictionary<string, Variable>();
            this.parentScope = parent;

            this.stack = new CommandStack();
            this.argStack = new Dequeue<Tuple<int, StackItem>>();
        }
        #endregion

        #region Private Methods

        #endregion

        #region Public Methods
        /// <summary>
        /// Get a new scope and set the current scope as the new parent scope
        /// </summary>
        /// <returns>Scope instance</returns>
        public Scope GetNew()
        {
            Scope scope = new Scope(this);
            return scope;
        }

        /// <summary>
        /// Create a new variable in the current scope
        /// </summary>
        /// <param name="name">Name of the variable</param>
        /// <returns>New var</returns>
        public Variable CreateVariable(string name)
        {
            if (vars.ContainsKey(name))
            {
                throw new Exception("Column already exists: " + name);
            }
            else
            {
                var _var = new Variable(name);
                vars.Add(name, _var);
                return _var;
            }
        }

        /// <summary>
        /// Get an already existing variable
        /// </summary>
        /// <param name="name">Name of the var</param>
        /// <returns>Variable instance</returns>
        public Variable GetVariable(string name)
        {
            // Proof wether a variable exists in the current scope
            if (!vars.ContainsKey(name))
            {
                if (parentScope != null)
                {
                    // Look in the parent scope if the variable exists
                    return parentScope.GetVariable(name);
                }
                // Exit the script execution with throwing an exception
                throw new Exception("Variable does not exists: " + name);
            }
            else
            {
                // Return the existing variable
                return vars[name];
            }
        }
        #endregion

        #region Public Member
        /// <summary>
        /// Get the parent scope if exists
        /// </summary>
        internal Scope ParentScope
        {
            get { return parentScope; }
        }

        /// <summary>
        /// Stack
        /// </summary>
        public CommandStack Stack
        {
            get { return stack; }
        }

        /// <summary>
        /// Argument stack, here will be all parameters which were passed to a function/method stored
        /// </summary>
        public Dequeue<Tuple<int, StackItem>> ArgumentStack
        {
            get { return argStack; }
        }

        /// <summary>
        /// List with all vars
        /// </summary>
        internal IDictionary<string, Variable> Vars
        {
            get { return vars; }
        }
        #endregion
    }
}
