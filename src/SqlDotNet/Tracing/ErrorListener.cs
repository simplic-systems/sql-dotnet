// <copyright file="Token.cs" company="Benedikt Eggers">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Benedikt Eggers</author>
// <date>03/16/2015</date>
// <summary>Error-Listener, provides methods for Error-reporting, must be implemented for every special case</summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet
{
    /// <summary>
    /// Error listener interface, tracks all errors occured during the compiling process
    /// </summary>
    public interface IErrorListener
    {
        /// <summary>
        /// Will be called, if an error occured during compiling
        /// </summary>
        /// <param name="errorCode">Error-Code (T: Tokenizer, ST: SyntaxTree, SA: Semantic Analysis, C: Compiler</param>
        /// <param name="errorMessage">Detailed error message</param>
        /// <param name="index">Startindex of the error code</param>
        /// <param name="length">Length of the error code</param>
        /// <param name="token">Token which occurese the error</param>
        void Report(string errorCode, string errorMessage, int index, int length, Compiler.RawToken token);
    }
}
