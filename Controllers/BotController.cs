using JobHubBot.Interfaces.IDbInterfaces;
using JobHubBot.Resources;
using JobHubBot.Services.HandleServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Telegram.Bot.Types;

namespace JobHubBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly IAppDbContext _context;
        public BotController(IAppDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Update update, [FromServices] UpdateHandlers handleUpdateService, CancellationToken cancellationToken)
        {
            try
            {
                Console.WriteLine("Received Message");
                await handleUpdateService.HandleUpdateAsync(update, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                await handleUpdateService.HandleErrorAsync(ex);
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("test")]
        public async Task<IActionResult> Get()
        {
            return Ok(await _context.Users.ToListAsync());
        }
    }
}
