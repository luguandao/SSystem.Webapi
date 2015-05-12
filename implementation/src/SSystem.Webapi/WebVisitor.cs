using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSystem.Webapi.Core
{
    public abstract class WebVisitor
    {
        public MethodType Type { get; private set; }
        protected string _baseUrl;
        public abstract string Visit(string subUrl,NameValueCollection parameters);

        protected WebVisitor()
        {
        }

        public static WebVisitor Create(string baseUrl,MethodType type)
        {
            if(string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentException("baseUrl cannot be empty.");
            WebVisitor obj;
            switch (type)
            {
                    case MethodType.Get:
                    obj=new WebGetVisitor();
                    break;
                default:
                    throw new ArgumentException("cannot implement");
            }
            obj._baseUrl = baseUrl;
            obj.Type = type;
            return obj;
        }
    }
}
