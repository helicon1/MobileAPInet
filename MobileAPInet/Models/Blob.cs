using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Microsoft.Azure; // Namespace for CloudConfigurationManager 
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Blob storage types
using System.Configuration;
using System.Net.Http.Headers;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using MobileAPInet.Models;
using Microsoft.Azure.Documents;

namespace MobileAPInet.Models
{
        public class BlobHelper
        {
            
        public CloudBlockBlob GetBlockBlobReference(String blobID)
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                ConfigurationManager.AppSettings["StorageConnectionString"]);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference(ConfigurationManager.AppSettings["Container"]);

            // Retrieve reference to a blob named "myblob".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobID);

            return blockBlob;
        }
    }


    public class FileDownloadResult : IHttpActionResult
    {
        //public BlobHelper Container { get; set; }
        public string ImageId { get; set; }

        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            BlobHelper Container = new BlobHelper();
            var response = new HttpResponseMessage();
            response.Content = new PushStreamContent(async (outputStream, _, __) =>
            {
                try
                {
                    CloudBlockBlob blockBlob = Container.GetBlockBlobReference(String.Format("image{0}.jpg", ImageId));
                    await blockBlob.DownloadToStreamAsync(outputStream);
                }
                finally
                {
                    outputStream.Close();
                }
            });

            response.StatusCode = HttpStatusCode.OK;
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            return response;
        }
    }
    public class FileUploadResult : IHttpActionResult
    {
        //public CloudBlobContainer Container { get; set; }
        public string ImageId { get; set; }
        public HttpRequestMessage Request { get; set; }

        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            BlobHelper Container = new BlobHelper();
            var response = new HttpResponseMessage();
            //TODO CHange refernce to reflect media type by using the prefix fo content type header
            CloudBlockBlob blockBlob = Container.GetBlockBlobReference(String.Format("image{0}.jpg", ImageId));
            await blockBlob.UploadFromStreamAsync(await Request.Content.ReadAsStreamAsync());
            var baseUri = string.Format("{0}://{1}:{2}", Request.RequestUri.Scheme, Request.RequestUri.Host, Request.RequestUri.Port);
            response.Headers.Location = new Uri(string.Format("{0}/images/{1}", baseUri, ImageId));
            response.StatusCode = HttpStatusCode.OK;
            return response;
        }
    }
    public class FileDownload : IHttpActionResult
    {
        //public BlobHelper Container { get; set; }
        public string blobId { get; set; }
        public string contentType { get; set; }
        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            BlobHelper Container = new BlobHelper();
            var response = new HttpResponseMessage();
            CloudBlockBlob blockBlob = Container.GetBlockBlobReference(blobId);
            response.Content = new PushStreamContent(async (outputStream, _, __) =>
            {
                try
                {
                    
                    await blockBlob.DownloadToStreamAsync(outputStream);
                }
                finally
                {
                    outputStream.Close();
                }
            });

            response.StatusCode = HttpStatusCode.OK;
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            return response;
        }
    }
    public class FileUpload : IHttpActionResult
    {
        //public CloudBlobContainer Container { get; set; }
        public string formID { get; set; }
        public HttpRequestMessage Request { get; set; }

        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {

            BlobHelper Container = new BlobHelper();
            DocumentDBRepository.Initialize();
            string contentType = Request.Content.Headers.ContentType.MediaType; //ContentDisposition.DispositionType;
            string extension = Apache.getExtension(contentType);
            if (extension.Length == 0)
            {
                return Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);
            }
            string fname = System.Guid.NewGuid().ToString();
            String blobId = String.Format("{0}/{1}.{2}", formID, fname, extension);
            CloudBlockBlob blockBlob = Container.GetBlockBlobReference(blobId);
            await blockBlob.UploadFromStreamAsync(await Request.Content.ReadAsStreamAsync());

            Attachment att = new Attachment { MediaLink = blockBlob.Uri.ToString(), ContentType = contentType, Id = fname, };
            att.SetPropertyValue("blobId", blobId);
            await DocumentDBRepository.CreateAttachmentAsync(formID, att);
            //set contetn type to return json
            var response = new HttpResponseMessage();
            response = Request.CreateResponse(HttpStatusCode.OK, att);
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            return response;

            //BlobHelper Container = new BlobHelper();
            //DocumentDBRepository.Initialize();
            //var response = new HttpResponseMessage();
            ////TODO CHange refernce to reflect media type by using the prefix fo content type header

            //var streamProvider = new MultipartMemoryStreamProvider();

            //await Request.Content.ReadAsMultipartAsync(streamProvider);
            //List<Attachment> attList= new List<Attachment>();
            //foreach (HttpContent fileContent in streamProvider.Contents) {

            //    //var fname = fileContent.Headers.ContentDisposition.FileName;
            //    //if (fileCon.IsFaulted || t.IsCanceled)
            //    //{
            //    //    throw new HttpResponseException(HttpStatusCode.InternalServerError);
            //    //}
            //    string fname = System.Guid.NewGuid().ToString();
            //    string contentType = fileContent.Headers.ContentDisposition.DispositionType;
            //    string extension = Apache.getExtension(contentType);
            //    CloudBlockBlob blockBlob = Container.GetBlockBlobReference(String.Format("{0}/{1}.{2}", formID, fname, extension));
            //    await blockBlob.UploadFromStreamAsync(await fileContent.ReadAsStreamAsync());

            //    Attachment att = new Attachment { MediaLink = blockBlob.Uri.ToString(), ContentType = contentType };
            //    attList.Add(att);
            //    await DocumentDBRepository.CreateAttachmentAsync(formID, att);
            //              //int imageId = DatabaseCode(stream);
            //              //return imageId;
            //    };
            ////var baseUri = string.Format("{0}://{1}:{2}", Request.RequestUri.Scheme, Request.RequestUri.Host, Request.RequestUri.Port);
            ////response.Headers.Location = new Uri(string.Format("{0}/images/{1}", baseUri, ImageId));

            ////set contetn type to return json
            //response = Request.CreateResponse(HttpStatusCode.OK, attList);
            //response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            //return response;
        }
    }
}