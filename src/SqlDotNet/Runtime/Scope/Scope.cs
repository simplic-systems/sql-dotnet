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
    public class Scope
    {
        #region Private Member
        private IDictionary<string, Variable> vars;
        private IDictionary<string, Cursor> cursors;
        private IDictionary<string, ResultSet> resultSets;
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
            cursors = new Dictionary<string, Cursor>();
            resultSets = new Dictionary<string, ResultSet>();
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
        /// Create and add a new cursor
        /// </summary>
        /// <param name="name">Name of the cursor</param>
        /// <returns>Cursor instance</returns>
        public Cursor CreateCursor(string name)
        {
            if (cursors.ContainsKey(name))
            {
                throw new Exception("Cursor already exists: " + name);
            }
            else
            {
                var _cursor = new Cursor(name);
                cursors.Add(name, _cursor);
                return _cursor;
            }
        }

        /// <summary>
        /// Create and add a new result set
        /// </summary>
        /// <param name="name">Name of the result set</param>
        /// <returns>Result set instance</returns>
        public ResultSet CreateResultSet(string name)
        {
            if (resultSets.ContainsKey(name))
            {
                throw new Exception("ResultSet already exists: " + name);
            }
            else
            {
                var _rs = new ResultSet(name);
                resultSets.Add(name, _rs);
                return _rs;
            }
        }

        /// <summary>
        /// Get last or default set, only for the current scope
        /// </summary>
        /// <returns>Current scope</returns>
        public ResultSet GetResultSet()
        {
            var res = resultSets.LastOrDefault().Value;

            if (res == null && parentScope != null)
            {
                return parentScope.GetResultSet();
            }

            return res;
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

        /// <summary>
        /// Get an already existing cursor
        /// </summary>
        /// <param name="name">Name of the cursor</param>
        /// <returns>Cursor instance</returns>
        public Cursor GetCursor(string name)
        {
            // Proof whether a cursor exists in the current scope
            if (!cursors.ContainsKey(name))
            {
                if (parentScope != null)
                {
                    // Look in the parent scope if the variable exists
                    return parentScope.GetCursor(name);
                }
                // Exit the script execution with throwing an exception
                throw new Exception("Cursor does not exists: " + name);
            }
            else
            {
                // Return the existing variable
                return cursors[name];
            }
        }

        /// <summary>
        /// Get an already existing result set
        /// </summary>
        /// <param name="name">Name of the resultset</param>
        /// <returns>Resultset instance</returns>
        public ResultSet GetResultSet(string name)
        {
            // Proof whether a cursor exists in the current scope
            if (!resultSets.ContainsKey(name))
            {
                if (parentScope != null)
                {
                    // Look in the parent scope if the variable exists
                    return parentScope.GetResultSet(name);
                }
                // Exit the script execution with throwing an exception
                throw new Exception("ResltSet does not exists: " + name);
            }
            else
            {
                // Return the existing variable
                return resultSets[name];
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
