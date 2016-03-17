using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlDotNet.Compiler;

namespace SqlDotNet
{
    /// <summary>
    /// Contains the complete query cache
    /// </summary>
    public static class QueryCache
    {
        private static IDictionary<string, CompiledQuery> compiledQueries;

        /// <summary>
        /// Create query cache
        /// </summary>
        static QueryCache()
        {
            compiledQueries = new Dictionary<string, CompiledQuery>();
        }

        /// <summary>
        /// Cache a query
        /// </summary>
        /// <param name="notHashedQuery">Sql statement</param>
        /// <param name="query">Query instance</param>
        public static void CacheQuery(string notHashedQuery, CompiledQuery query)
        {
            string hash = Simplic.Security.Cryptography.CryptographyHelper.HashSHA256(notHashedQuery);

            lock (compiledQueries)
            {
                if (compiledQueries.ContainsKey(hash))
                {
                    compiledQueries[hash] = query;
                }
                else
                {
                    compiledQueries.Add(hash, query);
                }
            }
        }

        /// <summary>
        /// Try to get a query. If not cached return null
        /// </summary>
        /// <param name="notHashedQuery">Not hashed query</param>
        /// <returns>Compiled query if found, else null</returns>
        public static CompiledQuery GetQuery(string notHashedQuery)
        {
            string hash = Simplic.Security.Cryptography.CryptographyHelper.HashSHA256(notHashedQuery);

            lock (compiledQueries)
            {
                if (compiledQueries.ContainsKey(hash))
                {
                    return compiledQueries[hash];
                }
            }

            return null;
        }
    }
}
