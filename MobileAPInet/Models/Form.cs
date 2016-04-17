using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using System;

namespace MobileAPInet.Models
{
    public class FormDocument: Resource
    {
        [JsonProperty(PropertyName = "id")]
        public string formId { get; set; }
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
    public class AttachmentItem : Resource
    {
        [JsonProperty(PropertyName = "id")]
        public string attachmentId { get; set; }
        [JsonProperty(PropertyName = "blobId")]
        public string blobId { get; set; }
        [JsonProperty(PropertyName = "description")]
        public string description { get; set; }
        [JsonProperty(PropertyName = "type")]
        public string attachmentType { get; set; }
        [JsonProperty(PropertyName = "created")]
        public string created { get; set; }
        [JsonProperty(PropertyName = "timestamp")]
        public DateTime timeStamp { get; set; }
        
        public static explicit operator AttachmentItem(Attachment v)
        {
            AttachmentItem item = new AttachmentItem();
            item.attachmentId = v.Id;
            item.description = v.GetPropertyValue<string>("description");
            item.created = v.Timestamp.ToString();
            item.attachmentType = v.ContentType.ToString();
            item.blobId = v.GetPropertyValue<string>("blobId");
            return item;

        }
    }
}