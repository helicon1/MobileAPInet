using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System.Configuration;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Net;
using System.Collections;
using MobileAPInet.Models;
using System.IO;

namespace MobileAPInet.Models
{
    public static class DocumentDBRepository
    {
        private static readonly string DatabaseId = ConfigurationManager.AppSettings["database"];
        private static readonly string CollectionId = ConfigurationManager.AppSettings["collection"];
        private static DocumentClient client;

        public static void Initialize()
        {
            client = new DocumentClient(new Uri(ConfigurationManager.AppSettings["endpoint"]), ConfigurationManager.AppSettings["authKey"]);
            //CreateDatabaseIfNotExistsAsync().Wait();
           // CreateCollectionIfNotExistsAsync().Wait();
        }

        private static async Task CreateDatabaseIfNotExistsAsync()
        {
            try
            {
                await client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(DatabaseId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await client.CreateDatabaseAsync(new Database { Id = DatabaseId });
                }
                else
                {
                    throw;
                }
            }
        }

        private static async Task CreateCollectionIfNotExistsAsync()
        {
            try
            {
                await client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(DatabaseId),
                        new DocumentCollection { Id = CollectionId },
                        new RequestOptions { OfferThroughput = 1000 });
                }
                else
                {
                    throw;
                }
            }
        }
        public static async Task<List<Document>> GetAllItems()
        {
            FeedResponse<dynamic> docs = await client.ReadDocumentFeedAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), new FeedOptions { MaxItemCount = 10 });

            List<Document> results = new List<Document>();
            foreach (var d in docs)
            {
                results.Add((Document)d);
            }

            return results;
        }
        public static async Task<Document> CreateItemAsync(object item)
        {
            return await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), item);
        }
        private static MemoryStream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
        public static async Task<Document> CreateItemAsync(string js)
        {
            //Stream s = GenerateStreamFromString(js);
            //JsonSerializable.LoadFrom<MemoryStream>(s);
            return await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), js );
        }
        public static async Task<Attachment> CreateAttachmentAsync(string id, object attachment)
        {
            Document d = await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id));
            Attachment a = await client.CreateAttachmentAsync(d.SelfLink, attachment);
            return a;

        }
        public static async Task<Attachment> GetAttachment(string formId, string attachId)
        {
            Document d = await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, formId));
            Attachment a = await client.ReadAttachmentAsync(UriFactory.CreateAttachmentUri(DatabaseId, CollectionId, formId, attachId));
            
            return a;

        }
        //Return FeedResponse as list of attachments
        public static async Task<List<AttachmentItem>> GetAttachmentList(string id)
        {
            Document d = await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id));
            FeedResponse<Attachment> fr = await client.ReadAttachmentFeedAsync(d.AttachmentsLink);
            List<AttachmentItem> attachmentList = new List<AttachmentItem>();
            AttachmentItem item;
            foreach (Attachment att in fr)
            {
                item = (AttachmentItem)att;
                
                attachmentList.Add(item);
            }
            return attachmentList;
        }
        public static async Task<Attachment> CreateAttachmentAsyncInDocDB(string id, Stream attStream, MediaOptions mediaOpts )
        {
            Document d = await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id));
            Attachment a = await client.CreateAttachmentAsync(d.AttachmentsLink, attStream, mediaOpts );
            return a;

        }
        public static async Task<Document> UpdateItemAsync(string id, string item)
        {
            return await client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id), item);
        }

        public static async Task<Document> GetItem(string id)
        {
            try
            {
                Document document = await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id));
                return document;
                //return (T)(dynamic)document;
                //return (String)(dynamic)document.ToString() ;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }
    }

}