using SqlDotNet.Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Debugger
{
    public class ErrorObject
    {
        public string ErrorCode
        {
            get;
            set;
        }

        public string ErrorMessage
        {
            get;
            set;
        }

        public int Index
        {
            get;
            set;
        }

        public int Length
        {
            get;
            set;
        }

        public RawToken Token
        {
            get;
            set;
        }
    }
}
