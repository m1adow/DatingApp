using DatingApp.Api.Data;
using DatingApp.Api.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Api.Controllers
{
	public class BuggyController : BaseApiController
	{
        private readonly DataContext context;

        public BuggyController(DataContext context)
		{
            this.context = context;
        }

        [Authorize]
        [HttpGet("auth")]
        public ActionResult<string> GetSecret()
        {
            return "secret text";
        }

        [HttpGet("not-found")]
        public ActionResult<AppUser> GetNotFound()
        {
            var thing = this.context.Users.Find(-1);

            if (thing is null)
                return NotFound();

            return thing;
        }

        [HttpGet("server-error")]
        public ActionResult<string> GetServerError()
        {
            var thing = this.context.Users.Find(-1);

            var thingToReturn = thing.ToString();

            return thingToReturn;
        }

        [HttpGet("bad-request")]
        public ActionResult<string> GetBadRequest()
        {
            return BadRequest("This was not a good request");
        }
    }
}

