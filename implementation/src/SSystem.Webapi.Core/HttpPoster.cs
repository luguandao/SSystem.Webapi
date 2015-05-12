using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSystem.Webapi.Core.Posters;

namespace SSystem.Webapi.Core
{
    public abstract class HttpPoster
    {
        public static readonly int WaitTimeout = int.Parse(ConfigurationManager.AppSettings["waitTimeout"] ?? "10") * 1000;
        public MethodType Type { get; private set; }
        protected string _baseUrl;
        public abstract Task<string> PostAsync(string subUrl);

        public abstract string Post(string subUrl);

        protected HttpPoster()
        {
        }

        protected Dictionary<string,string> NameValues=new Dictionary<string, string>();

        public void AddParameter(string name, string value)
        {
            NameValues.Add(name,value);
        }

        public static HttpPoster Create(string baseUrl, MethodType type)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentException("baseUrl cannot be empty.");
            HttpPoster obj;
            switch (type)
            {
                case MethodType.Get:
                    obj = new HttpGet();
                    break;
                case MethodType.Post:
                    obj = new HttpPost();
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
