using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfJsd.Model
{
    public class LoginUser
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "defaultWarehouseId")]
        public int WhId { get; set; }

        [JsonProperty(PropertyName = "defaultWarehouseName")]
        public string WhName { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "companyId")]
        public string CompanyId { get; set; }
    }
}
