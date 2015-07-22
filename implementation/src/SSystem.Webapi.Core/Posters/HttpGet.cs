using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SSystem.Webapi.Core.Posters
{
    public class HttpGet : HttpPoster
    {
        public override async Task<string> PostAsync(string subUrl)
        {
            var url = BaseUrl + AttachParametersToSubUrl(subUrl);
            
            using (var handler = new HttpClientHandler {UseCookies = true})
            {
                if (CookiesContainer != null)
                {
                    handler.CookieContainer = CookiesContainer;
                }
                using (var client = new HttpClient(handler))
                {
                    client.Timeout = new TimeSpan(0, 0, 0, 0, WaitTimeout);
                              
                    var request = new HttpRequestMessage(HttpMethod.Get, url);
                    if (!string.IsNullOrWhiteSpace(SessionId))
                    {
                        request.Headers.Add("Cookie", string.Format("{0}={1}", SessionName, SessionId));
                    }
                    var res = client.SendAsync(request).Result.Content;

                    CookiesContainer = handler.CookieContainer;

                    var content = await res.ReadAsStringAsync();
                    return content;
                }
            }
        }

        protected override string _Post(string subUrl)
        {
            var r = PostAsync(subUrl);
            r.Wait(WaitTimeout);
            return r.Result;
        }

        protected override byte[] _PostForResponse(string subUrl)
        {
            var url = BaseUrl + subUrl;
            var er = NameValues.GetEnumerator();

            StringBuilder sb = new StringBuilder();
            while (er.MoveNext())
            {
                if (sb.Length > 0)
                    sb.Append("&");
                sb.Append(er.Current.Key + "=" + er.Current.Value);
            }
            using (var handler = new HttpClientHandler {UseCookies = true})
            {
                if (CookiesContainer != null)
                {
                    handler.CookieContainer = CookiesContainer;
                }
                using (var client = new HttpClient())
                {
                    client.Timeout = new TimeSpan(0, 0, 0, 0, WaitTimeout);
                    if (sb.Length > 0)
                    {
                        url += "?" + sb;
                    }
                    var request = new HttpRequestMessage(HttpMethod.Get, url);
                    if (!string.IsNullOrWhiteSpace(SessionId))
                    {
                        request.Headers.Add("Cookie", string.Format("{0}={1}", SessionName, SessionId));
                    }
                    var task = client.SendAsync(request).Result.Content;

                    CookiesContainer = handler.CookieContainer;

                    return task.ReadAsByteArrayAsync().Result;

                }
            }
        }

        protected virtual string AttachParametersToSubUrl(string subUrl)
        {
            if (subUrl.IndexOf('{') > -1 && subUrl.IndexOf('}') > -1)
            {
                StringBuilder sb1 = new StringBuilder(subUrl);
                var er1 = NameValues.GetEnumerator();
                while (er1.MoveNext())
                {
                    sb1.Replace(string.Format("{{{0}}}",er1.Current.Key), er1.Current.Value);
                }
                subUrl = sb1.ToString(); sb1.Clear();
            }
            else
            {
                var er = NameValues.GetEnumerator();

                StringBuilder sb = new StringBuilder();
                while (er.MoveNext())
                {
                    if (sb.Length > 0)
                        sb.Append("&");
                    sb.Append(er.Current.Key + "=" + er.Current.Value);
                }
                if (sb.Length > 0)
                {
                    subUrl += "?" + sb.ToString();
                }
            }
            return subUrl;
        }
    }
}
