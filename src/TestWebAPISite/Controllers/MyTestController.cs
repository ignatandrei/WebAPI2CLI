using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TestWebAPISite.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MyTestController : ControllerBase
    {
       
        // POST: api/MyTest
        [HttpPost]
        public int AddValues([FromBody] SumArgs sumargs)
        {
            return sumargs.x + sumargs.y;
        }

        [HttpPost]
        public SumArgs GetTestValue()
        {
            return new SumArgs()
            {
                x = 10,
                y = 20
            };
        }


    }
}
