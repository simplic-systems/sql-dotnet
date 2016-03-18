using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet
{
    /// <summary>
    /// Class for compiling and executing sql statements using Simplic Async SQL. This class automatically caches compiled scripts.
    /// </summary>
    public class Sql
    {
        #region Private Member
        private CLRInterface.IQueryExecutor executor;
        private IErrorListener errorListener;
        #endregion

        #region Constructor
        /// <summary>
        /// Create sql interface
        /// </summary>
        /// <param name="executor">If statements should be executed and not just compiled, object which definition
        /// inherit from IQieryExecuter must be passed. This implementation carries for all sql executions</param>
        public Sql(CLRInterface.IQueryExecutor executor, IErrorListener errorListener)
        {
            this.executor = executor;
            this.errorListener = errorListener;            
        }
        #endregion

        #region [CompileAndExecute]
        /// <summary>
        /// Compile and execute a query. Use caching
        /// </summary>
        /// <param name="sql">Sql query as string</param>
        /// <param name="parameter">Parameter as list</param>
        /// <param name="useCache">if set to true, cached queries will be used</param>
        /// <returns>Return value, amount for none query objects, else query result</returns>
        public object CompileAndExecute(string sql, IList<CLRInterface.QueryParameter> parameter, bool useCache = true)
        {
            Compiler.CompiledQuery query = null;

            if (useCache)
            {
                query = QueryCache.GetQuery(sql);
            }

            if (query == null)
            {
                query = Compile(sql);
            }

            if (query != null)
            {
                return Execute(query, parameter);
            }

            return null;
        }
        #endregion

        #region [Compile]
        /// <summary>
        /// Compile sql and return compiled result
        /// </summary>
        /// <param name="sql">Sql-Code to compile</param>
        /// <returns>Result of the compiling process ready for executing</returns>
        public Compiler.CompiledQuery Compile(string sql)
        {
            var parserConfig = new Compiler.ParserConfiguration();

            var tokenizer = GetTokenizer();
            tokenizer.ParseAsync(sql);

            var syntaxTreeBuilder = new Compiler.SyntaxTreeBuilder(parserConfig, tokenizer.Tokens, errorListener);
            var entryPoint = syntaxTreeBuilder.Build();

            var compiler = new Compiler.SIQLCompiler(errorListener, executor);
            var query = compiler.Compile(entryPoint);

            QueryCache.CacheQuery(sql, query);

            return query;
        }
        #endregion

        /// <summary>
        /// Execute a compiled sql query
        /// </summary>
        /// <param name="query">Compiled query instance</param>
        /// <param name="parameter">List of parameter</param>
        /// <returns>Return value, amount for none query objects, else query result</returns>
        public object Execute(Compiler.CompiledQuery query, IList<CLRInterface.QueryParameter> parameter)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }
            if (executor == null)
            {
                throw new Exception("No executor set");
            }

            Runtime.SCLRuntime runtime = new Runtime.SCLRuntime(parameter);
            runtime.Execute(query.CommandChainRoot);

            return null;
        }

        #region [Helper]
        /// <summary>
        /// Create a new tokenizer instance
        /// </summary>
        /// <returns></returns>
        public Compiler.Tokenizer GetTokenizer()
        {
            // Tokenize the code
            Compiler.ParserConfiguration parser = new Compiler.ParserConfiguration();
            return new Compiler.Tokenizer(parser, errorListener);
        }
        #endregion
    }
}
