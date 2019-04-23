using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MVC_App.Models;
using MVC_App.Helpers;
using Microsoft.Extensions.Logging;

namespace MVC_App.Controllers
{
    [Route("api")]
    public class APIController : ControllerBase
    {
        private readonly ILogger _logger;

        public APIController(ILogger<APIController> logger)
        {
            _logger = logger;
        }

        // /api/unhash
        // expects just a plain number received in request body
        [HttpPost("unhash")]
        public ActionResult<string> Unhash([FromBody]long hash)
        {
            return Hash.Unhash(hash, _logger);
        }
    }
}