using ConfTool.Server.Services;
using ConfTool.Shared.Models;
using ConfTool.Shared.Services;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;

namespace ConfTool.Server.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class SpeakersController : ControllerBase
    {
        private readonly SpeakersService _speakersService;

        public SpeakersController(SpeakersService contributionsService)
        {
            _speakersService = contributionsService ?? throw new ArgumentNullException(nameof(contributionsService));
        }

        [HttpGet]
        public async Task<IActionResult> GetSpeakersAsync([FromQuery] int skip = 0, [FromQuery] int take = 1000, CancellationToken cancellation = default)
        {
            var result = await _speakersService.GetSpeakersAsync(skip, take, cancellation);
            if (result.Any())
            {
                return Ok(result);
            }

            return NotFound();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSpeakerAsync([FromRoute] Guid id, CancellationToken cancellation = default)
        {
            var result = await _speakersService.GetSpeakerAsync(id, cancellation);
            if (result is not null)
            {
                return Ok(result);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> AddOrUpdateSpeakerAsync([FromBody] SpeakerDto speaker, CancellationToken cancellation = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var options = new CallOptions(cancellationToken: cancellation);
                await _speakersService.AddOrUpdateSpeakerAsync(speaker, cancellation);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            return Ok();
        }
    }
}
