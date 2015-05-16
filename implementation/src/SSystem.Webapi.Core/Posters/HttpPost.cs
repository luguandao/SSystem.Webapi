using System;
using System.Net.Http;
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
                return client.PostAsync(BaseUrl + subUrl, content).Result.Content.ReadAsStringAsync();
            }

        }

        protected override string _Post(string subUrl)
        {
            var r = PostAsync(subUrl);
            r.Wait(WaitTimeout);
            return r.Result;
        }

      
    }
}
