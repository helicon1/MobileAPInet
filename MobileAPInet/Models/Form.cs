using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using System;

namespace MobileAPInet.Models
{
    public class FormDocument: Resource
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "ftype")]
        public string formType { get; set; }
        [JsonProperty(PropertyName = "fver")]
        public string formVersion { get; set; }
        [JsonProperty(PropertyName = "creator")]
        public string creator { get; set; }
        [JsonProperty(PropertyName = "timestamp")]
        public DateTime timeStamp { get; set; }
        [JsonProperty(PropertyName = "status")]
        public string status { get; set; }
        [JsonProperty(PropertyName = "data")]
        public string data { get; set; }
    }
    public class fdata
    {
        [JsonProperty(PropertyName = "data")]
        public object data { get; set; }
    }
}