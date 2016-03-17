using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Compiler
{
    public class CompiledQuery
    {
        public EntryPointNode EntryPoint
        {
            get;
            set;
        }

        public Stream ILCode
        {
            get;
            internal set;
        }
    }
}
