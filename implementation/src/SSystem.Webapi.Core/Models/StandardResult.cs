using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SSystem.Webapi.Core.Models
{
    [DataContract(Name = "standard_result", Namespace = "")]
    public class StandardResult<T>
    {
        [DataMember(Name = "result")]
        public bool Result { get; set; }

        private string _message;

        [DataMember(Name = "message")]
        public string Message
        {
            get
            {
                return _message ?? "";
            }
            set { _message = value; }
        }

        [DataMember(Name = "data")]
        public T Data { get; set; }

        public StandardResult()
        {
            Message = string.Empty;
            Data = default(T);
        }
    }

    [DataContract(Name = "standard_result", Namespace = "")]
    public class StandardResult : StandardResult<object>
    {

    }
}
