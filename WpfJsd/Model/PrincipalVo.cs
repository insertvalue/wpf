using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfJsd.Model
{
    class PrincipalVo
    {
        [JsonProperty(PropertyName = "user")]
        public LoginUser User { get; set; }

        [JsonProperty(PropertyName = "sessionId")]
        public string SessionId { get; set; }
    }
}
