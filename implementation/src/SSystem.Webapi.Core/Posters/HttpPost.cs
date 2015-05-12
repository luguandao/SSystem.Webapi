using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.WebRequestMethods;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace SSystem.Webapi.Core.Posters
{
    public class HttpPost : HttpPoster
    {
        public override Task<string> PostAsync(string subUrl)
        {
            using (var client = new HttpClient())
            {
                
                client.Timeout = new TimeSpan(0, 0, 0, 0, WaitTimeout);
                
                var content=new FormUrlEncodedContent(NameValues);

                //content.Headers.ContentType=new MediaTypeHeaderValue("application/json");
                return client.PostAsync(_baseUrl + subUrl, content).Result.Content.ReadAsStringAsync();
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
