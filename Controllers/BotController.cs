﻿using JobHubBot.Resources;
using JobHubBot.Services.HandleServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Telegram.Bot.Types;

namespace JobHubBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly IStringLocalizer<BotLocalizer> _localization;
        public BotController(IStringLocalizer<BotLocalizer> stringLocalizer)
        {
            _localization = stringLocalizer;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Update update, [FromServices] UpdateHandlers handleUpdateService, CancellationToken cancellationToken)
        {
            try
            {
                Console.WriteLine(_localization["greeting"]);

                await handleUpdateService.HandleUpdateAsync(update, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                await handleUpdateService.HandleErrorAsync(ex);
                return BadRequest(ex.Message);
            }
        }
    }
}
