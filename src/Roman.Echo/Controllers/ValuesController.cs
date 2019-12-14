using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Roman.Echo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        public async Task<IActionResult> Get([FromServices] IConfiguration configuration)
        {
            
            var nextUrl = configuration["next_url"];
            bool? successfullCall = null;
            try
            {
                if (!string.IsNullOrWhiteSpace(nextUrl))
                {
                    var str = await new HttpClient().GetStringAsync(nextUrl);

                }
            }
            catch (Exception)
            {
                successfullCall = false;
            }

            var response = new
            {
                headers = Request.Headers.ToDictionary(e => e.Key, v => v.Value.ToArray()),
                calledChild = !string.IsNullOrWhiteSpace(nextUrl),
                successfullCall = successfullCall
            };

            return this.Ok(response);

        }
    }
}