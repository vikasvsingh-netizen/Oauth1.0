using Microsoft.AspNetCore.Mvc;
using Oauth_1a_Demo.Model;

namespace Oauth_1a_Demo.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        [HttpGet("get")]
        public IActionResult Get(string name, int age)
        {
            return Ok(new
            {
                message = "you have passed oauth test",
                name,
                age
            });
        }

        [HttpPost("post-json")]
        public IActionResult PostJson([FromBody] UserModel model)
        {
            return Ok(new
            {
                message = "you have passed oauth test",
                model
            });
        }

        [HttpPost("post-form")]
        public IActionResult PostForm([FromForm] UserModel model)
        {
            return Ok(new
            {
                message = "you have passed oauth test",
                model
            });
        }

        [HttpPut("put/{id}")]
        public IActionResult Put(int id, [FromBody] UserModel model)
        {
            return Ok(new
            {
                message = "you have passed oauth test",
                id,
                model
            });
        }

        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            return Ok(new
            {
                message = "you have passed oauth test",
                deletedId = id
            });
        }
    }
}
