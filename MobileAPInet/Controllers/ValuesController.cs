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
            String item = await DocumentDBRepository.GetItem(id);
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
    }
        public class HelloWorldController : ApiController
    {
        [HttpGet]
        [Route("HelloWorld({id})")]
        public String Get(int id)
        {
            switch (id)
            {
                case 1:
                    return "Hello Azure";
                case 2:
                    return "ITS";
                default:
                    return "Mobile API Service";
            }
                

            //return new string[] { "XX", "XX" };
        }

        [HttpGet]
        [Route("HelloWorld")]
        public String Get()
        {
            return "xx";
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
