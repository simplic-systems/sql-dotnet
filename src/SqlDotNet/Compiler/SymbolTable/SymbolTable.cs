using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Compiler
{
    /// <summary>
    /// Symbol table containing all symbol information.
    /// </summary>
    public class SymbolTable
    {
        #region Private Member
        private SymbolTable parent;
        private List<Symbol> symbols;
        #endregion

        #region Constructor
        /// <summary>
        /// Create a new symbol table
        /// </summary>
        public SymbolTable(SymbolTable parent)
        {
            // Set parent symbol table
            this.parent = parent;
            symbols = new List<Symbol>();
        }
        #endregion

        #region Private Methods

        #endregion

        #region Public Methods
        /// <summary>
        /// Add variable symbol to the current SymboleTable
        /// </summary>
        /// <param name="node">Name of the symbol</param>
        public void AddSqlTableSymbol(TableNode node)
        {
            SqlTableSymbol symbol = new SqlTableSymbol();
            symbol.TreeNode = node;

            symbols.Add(symbol);
        }

        #region [FindSymbol]
        /// <summary>
        /// Find a symbol in the symbol table
        /// </summary>
        /// <param name="name">Name of the symbol</param>
        /// <returns>Return symbol instance</returns>
        public Symbol FindSymbol(string name)
        {
            var returnValue = symbols.Where(Item => Item.TreeNode.Token.Content == name).ToList();

            if (returnValue.Any())
            {
                return returnValue.First();
            }

            // Search in parent symbol table
            if (parent != null)
            {
                // Fins symbol in the table
                return parent.FindSymbol(name);
            }
            else
            {
                // Return not found
                return null;
            }
        }

        /// <summary>
        /// Return all symbols of a type
        /// </summary>
        /// <typeparam name="T">Type to select</typeparam>
        /// <returns>Return list of a specific type</returns>
        public IList<T> FindSymbols<T>() where T : Symbol
        {
            return symbols.OfType<T>().ToList();
        }
        #endregion

        #endregion

        #region Public Member
        /// <summary>
        /// Parent symbol table
        /// </summary>
        public SymbolTable Parent
        {
            get { return parent; }
        }
        #endregion
    }
}
