using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfJsd.Model
{
    class ResponseModel
    {
        [JsonProperty(PropertyName = "data")]
        public dynamic Data { get; set; }

        [JsonProperty(PropertyName = "rtnStatus")]
        public string RtnStatus { get; set; }

        [JsonProperty(PropertyName = "rtnMsg")]
        public string RtnMsg { get; set; }
    }
}
