using ConfTool.Server.Services;
using ConfTool.Shared.Models;
using ConfTool.Shared.Services;
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
        [HttpGet("count")]
        public async Task<IActionResult> GetSpeakersCountAsync(CancellationToken cancellation = default)
        {
            return Ok(await _speakersService.GetSpeakersCountAsync(cancellation));
        }

        [HttpGet]
        public async Task<IActionResult> GetSpeakersAsync([FromQuery] int skip = 0, [FromQuery] int take = 100, CancellationToken cancellation = default)
        {
            var result = await _speakersService.GetSpeakersAsync(new CollectionRequest
            {
                Skip = skip,
                Take = take,
                SearchTerm = string.Empty
            });
            if (result.Any())
            {
                return Ok(result);
            }

            return NotFound();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSpeakerAsync([FromRoute] Guid id, CancellationToken cancellation = default)
        {
            var result = await _speakersService.GetSpeakerAsync(new IdRequest {Id = id});
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
                await _speakersService.AddOrUpdateSpeakerAsync(new AddOrUpdateRequest<SpeakerDto>
                {
                    Id = speaker.Id,
                    Dto = speaker
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSpeakerAsync([FromRoute] Guid id, CancellationToken cancellation = default)
        {
            try
            {
                await _speakersService.DeleteSpeakerAsync(new IdRequest {Id = id});
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            return Ok();
        }
    }
}
