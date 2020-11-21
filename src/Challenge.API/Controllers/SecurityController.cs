using System.Net;
using System.Threading.Tasks;
using Challenge.Core.DTO;
using Challenge.Data.Repository.Interfaces;
using Challenge.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Challenge.API.Controllers
{
    [EnableCors()]
    [Route("api/security")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private readonly ISecurityRepository repository;
        private readonly ITokenService tokenService;

        public SecurityController(ISecurityRepository repository, ITokenService tokenService)
        {
            this.repository = repository;
            this.tokenService = tokenService;
        }

        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("login")]
        public async Task<ActionResult> Authenticate([FromBody]LoginDTO user)
        {
            var merchant = repository.Authenticate(user.Login, user.Password);

            if (merchant == null)
                return NotFound();

            var token = tokenService.GenerateToken(merchant);

            return Ok(
             new
            {
                user = user.Login,
                token = token
            });
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("create")]
        public async Task<ActionResult> CreateLogin([FromBody]UserDTO user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            repository.CreateUser(user);

            return new JsonResult(HttpStatusCode.Created);
        }


    }
}