using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSystem.Webapi.Core
{
    public class WebGetVisitor :WebVisitor
    {
        

        public override string Visit(string subUrl, NameValueCollection parameters)
        {
            using (HttpClient client = new HttpClient())
            {
                if (method == "get")
                {
                    if (sb.Length > 0)
                    {
                        url += "?" + sb;
                    }
                    //result = await client.GetAsync(url);
                    var t = client.GetAsync(url);
                    t.Wait(ServiceHelper.WaitByMilliseconds);
                    return t.Result.Content.ReadAsStringAsync().Result;
                }
                else if (method == "post")
                {
                    HttpContent content = new StringContent(sb.ToString());
                    var t = client.PostAsync(url, content);
                    t.Wait(ServiceHelper.WaitByMilliseconds);
                    return t.Result.Content.ReadAsStringAsync().Result;

                }

                throw new Exception("unknown method:" + method);



            }
        }
    }
}
