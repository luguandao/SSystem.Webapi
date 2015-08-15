using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SSystem.Webapi.Core.Models;
using SSystem.Webapi.Core.Posters;
using System.Net.Http.Headers;

namespace SSystem.Webapi.Core
{
    public abstract class HttpPoster
    {
        public readonly string UniqueNumber = Guid.NewGuid().ToString();
        public static readonly int WaitTimeout = int.Parse(ConfigurationManager.AppSettings["waitTimeout"] ?? "10") * 1000;
        public MethodType Type { get; private set; }

        protected Dictionary<string, string> _headers = new Dictionary<string, string>();

        protected AuthenticationHeaderValue _BasicAuthorization;

        protected string BaseUrl;
        public abstract Task<string> PostAsync(string subUrl);

        public event EventHandler<EventArgs> Starting;
        public event EventHandler<EventArgs> Completed;
        public event EventHandler<PostExceptionEventArg> Error;

        public string SessionId { get; set; }

        public CookieContainer CookiesContainer { get; set; }

        public string SessionName { get; set; } = "ASP.NET_SessionId=";

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
                while (e.InnerException != null)
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

        public void AddCookies(HttpCookieCollection cookies)
        {
            for (int i = 0; i < cookies.Count; i++)
            {
                var hc = cookies[i];
                if (hc == null) continue;

                CookiesContainer.Add(new Cookie
                {
                    Domain = hc.Domain,
                    Expires = hc.Expires,
                    Name = hc.Name,
                    Path = hc.Path,
                    Secure = hc.Secure,
                    Value = hc.Value
                });

            }
        }

        public HttpPoster AddHeader(string name, string value)
        {
            if (!_headers.ContainsKey(name))
            {
                _headers.Add(name, value);
            }
            return this;
        }

        public HttpPoster AddAuthorization(string scheme, string parameter)
        {
            _BasicAuthorization = new AuthenticationHeaderValue(scheme, parameter);
            return this;
        }

        protected abstract string _Post(string subUrl);
        protected abstract byte[] _PostForResponse(string subUrl);

        public byte[] PostForResponseMessage(string subUrl)
        {
            try
            {
                OnStarting();
                _stopwatch = Stopwatch.StartNew();
                return _PostForResponse(subUrl);
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

        public string Post(string subUrl)
        {
            try
            {
                RequestUrl = BaseUrl + subUrl;
                OnStarting();
                _stopwatch = Stopwatch.StartNew();
                ResultContent = _Post(subUrl);
                return ResultContent;
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

        public T Post<T>(string subUrl)
        {
            return JsonConvert.DeserializeObject<T>(Post(subUrl));
        }

        public virtual HttpPoster AddAttachment(string fileFullPath)
        {
            return this;
        }

        protected NameValueCollection _NameValues = new NameValueCollection();

        public HttpPoster AddParameter(string name, string value)
        {
            _NameValues.Add(name, value);
            return this;
        }

        protected void AddHeaders(HttpRequestMessage request)
        {
            if (_headers.Count > 0)
            {
                var er = _headers.GetEnumerator();
                while (er.MoveNext())
                {
                    request.Headers.Add(er.Current.Key, er.Current.Value);
                }
            }
        }

        protected virtual string AttachParametersToSubUrl(string subUrl)
        {
            if (subUrl.IndexOf('{') > -1 && subUrl.IndexOf('}') > -1)
            {
                StringBuilder sb1 = new StringBuilder(subUrl);
                var er1 = _NameValues.GetEnumerator();
                foreach (var key in _NameValues.AllKeys)
                {
                    sb1.Replace(string.Format("{{{0}}}", key), _NameValues.Get(key));
                }
              
                subUrl = sb1.ToString(); sb1.Clear();
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                foreach (var key in _NameValues.AllKeys)
                {
                    if (sb.Length > 0)
                        sb.Append("&");
                    sb.Append(key + "=" + _NameValues.Get(key));
                }
                if (sb.Length > 0)
                {
                    subUrl += "?" + sb.ToString();
                }
            }
            return subUrl;
        }

        public static HttpPoster Create(string baseUrl, MethodType type, EventHandler<PostExceptionEventArg> exceptionAction)
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
                case MethodType.PostAttachment:
                    obj = new HttpPostAttachment();
                    break;
                case MethodType.Delete:
                    obj = new HttpDelete();
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

        #region 请求
        public string RequestUrl { get; protected set; }
        #endregion
        #region 返回结果
        public string ResultContent { get; protected set; }
        #endregion

        protected static IEnumerable<KeyValuePair<string, string>> ConvertToIEnumerable(NameValueCollection nv)
        {
            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();
            foreach (var key in nv.AllKeys)
            {
                result.Add(new KeyValuePair<string, string>(key, nv.Get(key)));
            }
            return result;
        }
    }
}
