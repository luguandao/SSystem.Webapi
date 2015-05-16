using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SSystem.Webapi.Core.Models;
using SSystem.Webapi.Core.Posters;

namespace SSystem.Webapi.Core
{
    public abstract class HttpPoster
    {
        public static readonly int WaitTimeout = int.Parse(ConfigurationManager.AppSettings["waitTimeout"] ?? "10") * 1000;
        public MethodType Type { get; private set; }
        protected string BaseUrl;
        public abstract Task<string> PostAsync(string subUrl);

        public event EventHandler<EventArgs> Starting;
        public event EventHandler<EventArgs> Completed;
        public event EventHandler<PostExceptionEventArg> Error;

        private Stopwatch _stopwatch;

        public TimeSpan SpendTime { get; private set; }

        protected void OnStarting()
        {
            Starting?.Invoke(this, new EventArgs());
        }

        protected void OnCompleted()
        {
            Completed?.Invoke(this, new EventArgs());
        }

        protected void OnError(Exception e)
        {
            if (Error != null)
            {
                while (e.InnerException!=null)
                {
                    e = e.InnerException;
                }
                Error(this, new PostExceptionEventArg(e));
            }
            else
            {
                throw e;
            }
        }


        protected abstract string _Post(string subUrl);

        public string Post(string subUrl)
        {
            try
            {
                OnStarting();
                _stopwatch = Stopwatch.StartNew();
                return _Post(subUrl);
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
            finally
            {
                _stopwatch.Stop();
                SpendTime = _stopwatch.Elapsed;
                OnCompleted();
            }
            return null;
        }

        public StandardResult<T> Post<T>(string subUrl)
        {
            //return new StandardResult<T>(  JsonConvert.DeserializeObject<T>(Post(subUrl));
            return new StandardResult<T>
            {
                Data = JsonConvert.DeserializeObject<T>(Post(subUrl))
            };
        }

        protected Dictionary<string, string> NameValues = new Dictionary<string, string>();

        public void AddParameter(string name, string value)
        {
            NameValues.Add(name, value);
        }

        public static HttpPoster Create(string baseUrl, MethodType type,EventHandler<PostExceptionEventArg> exceptionAction)
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
            obj.BaseUrl = baseUrl;
            obj.Type = type;

            obj.Error += exceptionAction;
            return obj;
        }

        public static HttpPoster Create(string baseUrl, MethodType type)
        {
            return Create(baseUrl, type, null);
        }


    }
}
