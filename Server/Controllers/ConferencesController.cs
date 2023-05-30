using ConfTool.Server.Services;
using ConfTool.Shared.Models;
using ConfTool.Shared.Services;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;

namespace ConfTool.Server.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ConferencesController : ControllerBase
    {
        private readonly ConferencesService _conferencesService;

        public ConferencesController(ConferencesService conferencesService)
        {
            _conferencesService = conferencesService ?? throw new ArgumentNullException(nameof(conferencesService));
        }

        [HttpGet]
        public async Task<IActionResult> GetConferencesAsync([FromQuery] int skip = 0, [FromQuery] int take = 1000,
            CancellationToken cancellation = default)
        {
            var result = await _conferencesService.GetConferencesAsync(skip, take, cancellation);
            if (result.Any())
            {
                return Ok(result);
            }

            return NotFound();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetConferenceAsync([FromRoute] Guid id,
            CancellationToken cancellation = default)
        {
            var result = await _conferencesService.GetConferenceAsync(id, cancellation);
            if (result is not null)
            {
                return Ok(result);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> AddOrUpdateConferenceAsync([FromBody] ConferenceDto conference,
            CancellationToken cancellation = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var options = new CallOptions(cancellationToken: cancellation);
                await _conferencesService.AddOrUpdateConferenceAsync(conference, cancellation);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            return Ok();
        }
    }
}