using ConfTool.Server.Services;
using ConfTool.Shared.Models;
using ConfTool.Shared.Services;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;

namespace ConfTool.Server.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ContributionsController : ControllerBase
    {
        private readonly ContributionsService _contributionsService;

        public ContributionsController(ContributionsService contributionsService)
        {
            _contributionsService =
                contributionsService ?? throw new ArgumentNullException(nameof(contributionsService));
        }

        [HttpGet]
        public async Task<IActionResult> GetContributionsAsync([FromQuery] int skip = 0, [FromQuery] int take = 100,
            [FromQuery] string? searchTerm = null, CancellationToken cancellation = default)
        {
            var options = new CallOptions(cancellationToken: cancellation);
            var result = await _contributionsService.GetContributionsAsync(new CollectionRequest
            {
                SearchTerm = searchTerm ?? string.Empty,
                Skip = skip,
                Take = take
            }, new ProtoBuf.Grpc.CallContext(options));
            if (result.Any())
            {
                return Ok(result);
            }

            return NotFound();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetContributionAsync([FromRoute] Guid id,
            CancellationToken cancellation = default)
        {
            var options = new CallOptions(cancellationToken: cancellation);
            var result = await _contributionsService.GetContributionAsync(new IdRequest { Id = id }, new ProtoBuf.Grpc.CallContext(options));
            if (result is not null)
            {
                return Ok(result);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> AddOrUpdateContributionAsync([FromBody] ContributionDto contribution,
            CancellationToken cancellation = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var options = new CallOptions(cancellationToken: cancellation);
                await _contributionsService.AddOrUpdateContributionAsync(new AddOrUpdateRequest<ContributionDto>
                    { Id = contribution.Id, Dto = contribution }, new ProtoBuf.Grpc.CallContext(options));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContributionAsync([FromRoute] Guid id,
            CancellationToken cancellation = default)
        {
            try
            {
                var options = new CallOptions(cancellationToken: cancellation);
                await _contributionsService.DeleteContributionAsync(new IdRequest { Id = id }, new ProtoBuf.Grpc.CallContext(options));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            return Ok();
        }
    }
}