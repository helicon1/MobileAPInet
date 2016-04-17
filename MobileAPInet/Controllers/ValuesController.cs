using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MobileAPInet.App;
using System.Threading.Tasks;
using MobileAPInet.Models;
using System.Configuration;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Azure.Documents.Client;
using System.IO;

namespace MobileAPInet.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/valuesx
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
    public class DocDbController : ApiController
    {
        [HttpGet]
        [Route("form({id})")]
        public async Task<HttpResponseMessage> Get(String id)
        {
            DocumentDBRepository.Initialize();
            if (id == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            Document item = await DocumentDBRepository.GetItem(id);
            //String item = await DocumentDBRepository.GetItemAsync(id);
            if (item == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound,"My Not found" );
            }

            HttpResponseMessage r = Request.CreateResponse(HttpStatusCode.OK, item);
            r.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            return r;

        }
        [HttpGet]
        [Route("form")]
        public async Task<HttpResponseMessage> Get()
        {
            DocumentDBRepository.Initialize();

            var items = await DocumentDBRepository.GetAllItems();
            //String item = await DocumentDBRepository.GetItemAsync(id);
            

            if (items == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "My Not found");
            }

            HttpResponseMessage r = Request.CreateResponse(HttpStatusCode.OK, items);
            r.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            return r;
        }
        [HttpPost]
        [Route("form")]
        public async Task<HttpResponseMessage> Post([FromBody]JToken jsonbody)//[FromBody] FormDocument form) //FormDocument form)
        {
            //FormDocument form = new FormDocument();    
            DocumentDBRepository.Initialize();
            Document doc;
            //string c = await Request.Content.ReadAsStringAsync();
            //Console.Write(c);
            JTokenReader jtReader= new JTokenReader(jsonbody);
            dynamic form = JObject.Parse(jsonbody.ToString());
            //form.LoadFrom(jtReader);
            //form.formType = jsonbody.Value<string>("ftype");
            //form.formVersion = jsonbody.Value<string>("ver");
            form.timeStamp = DateTime.Now;
            //form.data = jsonbody.Value<string>("data");
            form.creator = Request.GetRequestContext().Principal.Identity.Name;
            form.status = ConfigurationManager.AppSettings["defaultStatus"];
            
            try {
                //doc = await DocumentDBRepository.CreateItemAsync(jsonbody);
                doc = await DocumentDBRepository.CreateItemAsync(form);
            }
            catch  {
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            }

            HttpResponseMessage r = Request.CreateResponse(HttpStatusCode.OK, doc.Id );
            r.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            return r;
        }
    }
        public class HelloWorldController : ApiController
    {
        [HttpGet]
        [Route("HelloWorld({id})")]
        public HttpResponseMessage Get(int id)
        {
            string outstr;
            switch (id)
            {
                case 1:
                    outstr =  "Hello from Azure API";
                    break;
                case 2:
                    outstr = "ITS";
                    break;
                default:
                    outstr = "Mobile API Service";
                    break;
            }

            HttpResponseMessage r = Request.CreateResponse(HttpStatusCode.OK, outstr  );
            r.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            return r;
        }

        [HttpGet]
        [Route("HelloWorld")]
        public HttpResponseMessage Get()
        {
            HttpResponseMessage r = Request.CreateResponse(HttpStatusCode.OK, "Hello World");
            r.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            return r;
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
    public class AttachmentController : ApiController
    {
        //    https://azure.microsoft.com/en-us/documentation/articles/best-practices-api-implementation/

        
        [HttpGet]
        [Route("form({formId})/attachment({attachId})")]
        public async Task<IHttpActionResult> GetAttachment(String formId, string attachId)
        {
            try
            {
                DocumentDBRepository.Initialize();
                //Retrieve the attachment
                Attachment a = await DocumentDBRepository.GetAttachment(formId, attachId);

                //Retrieve the blob
                return new FileDownload()
                {
                    blobId = a.GetPropertyValue<string>("blobId"),
                    contentType = a.ContentType
                };
            }
            catch
            {
                return InternalServerError();
            }
        }
        [HttpGet]
        [Route("form({formId})/attachment")]
        public async Task<HttpResponseMessage> GetAttachmentList(string formId)
        {
            try
            {
                DocumentDBRepository.Initialize();
                List<AttachmentItem> list = await DocumentDBRepository.GetAttachmentList(formId);
                HttpResponseMessage r =  Request.CreateResponse(HttpStatusCode.OK, list);
                r.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                return r;
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        //multipart with one or more files
        [HttpPost]
        [Route("form({formId})/attachment")]
        public async Task<IHttpActionResult> Post(string formId)
        {
            try
            {
                // to create in BLOB...

                return new FileUpload()
                {
                    //Container = container,
                    formID = formId,
                    Request = Request
                };

                //To create in DocDB 
                //DocumentDBRepository.Initialize();
                //string contentType = Request.Content.Headers.ContentType.MediaType; //ContentDisposition.DispositionType;
                //string extension = Apache.getExtension(contentType);
                //if (extension.Length == 0)
                //{
                //    return Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);
                //}
                ////string fname = System.Guid.NewGuid().ToString();
                ////CloudBlockBlob blockBlob = Container.GetBlockBlobReference(String.Format("{0}/{1}.{2}", formID, fname, extension));
                ////await blockBlob.UploadFromStreamAsync(await Request.Content.ReadAsStreamAsync());

                //Attachment att = new Attachment { MediaLink = blockBlob.Uri.ToString(), ContentType = contentType, Id = fname };
                //MediaOptions mOpts = new MediaOptions();
                //mOpts.ContentType = contentType;
                //await DocumentDBRepository.CreateAttachmentAsyncInDocDB(formID,Request.Content.ReadAsStreamAsync(),mOpts);
                ////set contetn type to return json
                //var response = new HttpResponseMessage();
                //response = Request.CreateResponse(HttpStatusCode.OK, att);
                //response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                //return response;
                //TODO post attachement to Document DB

                //}
            }
            catch
            {
                return InternalServerError();
            }
        }
    }
    public class ProductImagesController : ApiController
    {
        //    https://azure.microsoft.com/en-us/documentation/articles/best-practices-api-implementation/
        [HttpGet]
        [Route("images/{id}")]
        public IHttpActionResult Get(String id)
        {
            try
            {
                //var container = ConnectToBlobContainer(ConfigurationManager.AppSettings["Container"]);

                //if (!BlobExists(container, string.Format("image{0}.jpg", id)))
                //{
                //    return NotFound();
                //}
                //else
                //{
                    return new FileDownloadResult()
                    {
                        //Container = container,
                        ImageId = id
                    };
                //}
            }
            catch
            {
                return InternalServerError();
            }
        }

        [HttpPost]
        [Route("images")]
        public async Task<IHttpActionResult> Post()
        {
            try
            {
                //TODO Enable all media but ensure its a file of some type
                if (!Request.Content.Headers.ContentType.MediaType.Equals("image/jpeg"))
                {
                    return StatusCode(HttpStatusCode.UnsupportedMediaType);
                }
                else
                {
                    var id = new Random().Next().ToString(); // Use a random int as the key for the new resource. Should probably check that this key has not already been used
                    //var container = ConnectToBlobContainer(Constants.PRODUCTIMAGESCONTAINERNAME);
                    return new FileUploadResult()
                    {
                        //Container = container,
                        ImageId = id,
                        Request = Request
                    };
                }
            }
            catch
            {
                return InternalServerError();
            }
        }
    }
}
