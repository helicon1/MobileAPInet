using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace MobileAPInet.App
{

    //public class FileDownloadResult : IHttpActionResult
    //{
    //    public CloudBlobContainer Container { get; set; }
    //    public int ImageId { get; set; }

    //    public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
    //    {
    //        var response = new HttpResponseMessage();
    //        response.Content = new PushStreamContent(async (outputStream, _, __) =>
    //        {
    //            try
    //            {
    //                CloudBlockBlob blockBlob = Container.GetBlockBlobReference(String.Format("image{0}.jpg", ImageId));
    //                await blockBlob.DownloadToStreamAsync(outputStream);
    //            }
    //            finally
    //            {
    //                outputStream.Close();
    //            }
    //        });

    //        response.StatusCode = HttpStatusCode.OK;
    //        response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
    //        return response;
    //    }
    //}
    //public class FileUploadResult : IHttpActionResult
    //{
    //    public CloudBlobContainer Container { get; set; }
    //    public int ImageId { get; set; }
    //    public HttpRequestMessage Request { get; set; }

    //    public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
    //    {
    //        var response = new HttpResponseMessage();
    //        CloudBlockBlob blockBlob = Container.GetBlockBlobReference(String.Format("image{0}.jpg", ImageId));
    //        await blockBlob.UploadFromStreamAsync(await Request.Content.ReadAsStreamAsync());
    //        var baseUri = string.Format("{0}://{1}:{2}", Request.RequestUri.Scheme, Request.RequestUri.Host, Request.RequestUri.Port);
    //        response.Headers.Location = new Uri(string.Format("{0}/productimages/{1}", baseUri, ImageId));
    //        response.StatusCode = HttpStatusCode.OK;
    //        return response;
    //    }
    //}
}