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
            var url = _baseUrl + subUrl;
            var er = NameValues.GetEnumerator();

            StringBuilder sb = new StringBuilder();
            while (er.MoveNext())
            {
                if (sb.Length > 0)
                    sb.Append("&");
                sb.Append(er.Current.Key + "=" + er.Current.Value);
            }
            using (var client = new HttpClient())
            {
                client.Timeout = new TimeSpan(0, 0, 0, 0, WaitTimeout);
                if (sb.Length > 0)
                {
                    url += "?" + sb;
                }
                return await client.GetAsync(url).Result.Content.ReadAsStringAsync();
            }
        }

        public override string Post(string subUrl)
        {
            var r = PostAsync(subUrl);
            r.Wait(WaitTimeout);
            return r.Result;
        }
    }
}
