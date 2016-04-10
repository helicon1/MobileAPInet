using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace MobileAPInet.Models
{
    public class Item
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "ver")]
        public string Ver { get; set; }

        [JsonProperty(PropertyName = "ftype")]
        public string ftype { get; set; }

        [JsonProperty(PropertyName = "data")]
        public JsonArrayAttribute data { get; set; }
    }
}