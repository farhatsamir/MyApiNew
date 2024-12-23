using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace samirApp.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class AuthController(SignInManager<IdentityUser> signInManager) : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager = signInManager;

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] object empty)
        {
            if (empty is not null)
            {
                await _signInManager.SignOutAsync();
                return Ok();
            }
            return Unauthorized();
        }
    }
}
