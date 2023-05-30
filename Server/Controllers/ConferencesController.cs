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
        public async Task<IActionResult> GetConferencesAsync([FromQuery] int skip = 0, [FromQuery] int take = 100,
            CancellationToken cancellation = default)
        {
            var options = new CallOptions(cancellationToken: cancellation);
            var result = await _conferencesService.GetConferencesAsync(new CollectionRequest
            {
                Skip = skip,
                Take = take,
                SearchTerm = string.Empty
            }, new ProtoBuf.Grpc.CallContext(options));
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
            var options = new CallOptions(cancellationToken: cancellation);
            var result = await _conferencesService.GetConferenceAsync(new IdRequest { Id = id }, new ProtoBuf.Grpc.CallContext(options));
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
                await _conferencesService.AddOrUpdateConferenceAsync(new AddOrUpdateRequest<ConferenceDto>
                {
                    Id = conference.Id,
                    Dto = conference
                }, new ProtoBuf.Grpc.CallContext(options));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConferenceAsync([FromRoute] Guid id,
            CancellationToken cancellation = default)
        {
            try
            {
                var options = new CallOptions(cancellationToken: cancellation);
                await _conferencesService.DeleteConferenceAsync(new IdRequest { Id = id }, new ProtoBuf.Grpc.CallContext(options));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            return Ok();
        }
    }
}