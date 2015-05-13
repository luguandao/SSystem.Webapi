using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSystem.Webapi.Core
{
    public class PostExceptionEventArg :EventArgs
    {
        public Exception Error { get; private set; }

        public PostExceptionEventArg(Exception e)
        {
            Error = e;
        }
    }
}
