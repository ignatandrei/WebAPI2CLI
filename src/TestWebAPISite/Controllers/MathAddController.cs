using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TestWebAPISite.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class MathAddController : ControllerBase
    {
        // GET: api/MathAdd
        [HttpGet]
        public IEnumerable<int> Get()
        {
            return new int[] { 1, 2 };
        }

        // GET: api/MathAdd/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value"+id;
        }

        // POST: api/MathAdd
        [HttpPost]
        public void Post([FromBody] string value)
        {
            //Console.WriteLine("I am from POST MATH")
        }

        // PUT: api/MathAdd/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {

        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {

        }
    }
}
