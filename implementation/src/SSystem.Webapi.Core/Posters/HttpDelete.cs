using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SSystem.Webapi.Core.Posters
{
    public class HttpDelete : HttpPoster
    {
        public override Task<string> PostAsync(string subUrl)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = new TimeSpan(0, 0, 0, 0, WaitTimeout);
                client.DefaultRequestHeaders.Authorization = _BasicAuthorization;

                var request = new HttpRequestMessage(HttpMethod.Delete, BaseUrl + subUrl);
                if (!string.IsNullOrWhiteSpace(SessionId))
                {
                    request.Headers.Add("Cookie", string.Format("{0}={1}", SessionName, SessionId));
                }

                AddHeaders(request);
                request.Content = new FormUrlEncodedContent(ConvertToIEnumerable(_NameValues));
                return client.SendAsync(request).Result.Content.ReadAsStringAsync();
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
            using (var client = new HttpClient())
            {
                client.Timeout = new TimeSpan(0, 0, 0, 0, WaitTimeout);
                client.DefaultRequestHeaders.Authorization = _BasicAuthorization;

                var content = new FormUrlEncodedContent(ConvertToIEnumerable(_NameValues));

                var request = new HttpRequestMessage(HttpMethod.Delete, BaseUrl + subUrl);
                if (!string.IsNullOrWhiteSpace(SessionId))
                {
                    request.Headers.Add("Cookie", string.Format("{0}={1}", SessionName, SessionId));
                }
                request.Content = content;
                var task = client.SendAsync(request);

                // var task = client.PostAsync(BaseUrl + subUrl, content);
                task.Wait(WaitTimeout);
                return task.Result.Content.ReadAsByteArrayAsync().Result;
            }
        }
    }
}
