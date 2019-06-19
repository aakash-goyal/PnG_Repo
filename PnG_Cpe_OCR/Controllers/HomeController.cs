﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PnG_Cpe_OCR.Controllers
{
    public class HomeController : ApiController
    {
        [Route("")]
        public HttpResponseMessage Get()
        {
            return Request.CreateResponse(HttpStatusCode.OK, "Hello World");
        }
    }
}