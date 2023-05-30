using System.Collections.Concurrent;
using ConfTool.Server.Database;
using ConfTool.Shared.Models;
using ConfTool.Shared.Services;
using Microsoft.EntityFrameworkCore;
using ProtoBuf.Grpc;

namespace ConfTool.Server.Services
{   
    public class ContributionsService : IContributionService
    {
        private readonly ConfToolDbContext _dbContext;
        private static readonly ConcurrentQueue<string> _queue = new();
        public ContributionsService(ConfToolDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<int> GetContributionsCountAsync()
        {
            return await _dbContext.Contributions.CountAsync();
        }

        public async Task<ICollection<ContributionDto>> GetContributionsAsync(CollectionRequest request)
        {
            var query = string.IsNullOrWhiteSpace(request.SearchTerm)
                ? _dbContext.Contributions
                    .Include(c => c.ContributionSpeakers)
                    .ThenInclude(cs => cs.Speaker)
                : _dbContext.Contributions
                    .Include(c => c.ContributionSpeakers)
                    .ThenInclude(cs => cs.Speaker)
                    .Where(c => 
                        string.Join("",c.Title, c.Abstract)
                            .Contains(request.SearchTerm, StringComparison.InvariantCultureIgnoreCase)
                    );

            var result = await query
                .OrderBy(c => c.StartDate)
                .Skip(request.Skip)
                .Take(request.Take)
                .Select(contribution => new ContributionDto
                {
                    Id = contribution.Id,
                    Title = contribution.Title,
                    Abstract = contribution.Abstract,
                    StartDate = contribution.StartDate,
                    EndDate = contribution.EndDate,
                    Speakers = contribution.ContributionSpeakers.Select(cs => new SpeakerDto
                    {
                        Id = cs.Speaker.Id,
                        FirstName = cs.Speaker.FirstName,
                        LastName = cs.Speaker.LastName,
                        ImageUrl = cs.Speaker.PictureUrl,
                        Abstract = cs.Speaker.Abstract
                    }).Distinct().ToList()
                }).ToListAsync();
            return result;
        }

        public async Task<ContributionDto?> GetContributionAsync(IdRequest request)
        {
            var contribution = await _dbContext.Contributions
                .Include(c => c.ContributionSpeakers)
                .ThenInclude(cs => cs.Speaker)
                .FirstOrDefaultAsync(c => c.Id == request.Id);
            return contribution is not null
                ? new ContributionDto
                {
                    Id = contribution.Id,
                    Title = contribution.Title,
                    Abstract = contribution.Abstract,
                    StartDate = contribution.StartDate,
                    EndDate = contribution.EndDate,
                    Speakers = contribution.ContributionSpeakers.Select(cs => new SpeakerDto
                    {
                        Id = cs.Speaker.Id,
                        FirstName = cs.Speaker.FirstName,
                        LastName = cs.Speaker.LastName,
                        ImageUrl = cs.Speaker.PictureUrl,
                        Abstract = cs.Speaker.Abstract
                    }).Distinct().ToList()
                }
                : null;
        }

        public async Task AddOrUpdateContributionAsync(AddOrUpdateRequest<ContributionDto> request)
        {
            if (request.Dto is null)
            {
                return;
            }
            
            var contribution = await _dbContext.Contributions.Include(c => c.ContributionSpeakers).FirstOrDefaultAsync(c => c.Id == request.Dto.Id);
            if (contribution is not null)
            {
                contribution.StartDate = request.Dto.StartDate;
                contribution.EndDate = request.Dto.EndDate;
                contribution.Title = request.Dto.Title;
                contribution.Abstract = request.Dto.Abstract;
                foreach (var speaker in request.Dto.Speakers)
                {
                    var relation = contribution.ContributionSpeakers.FirstOrDefault(cs => cs.SpeakerId == speaker.Id);
                    if (relation is null)
                    {
                        contribution.ContributionSpeakers.Add(new ContributionSpeaker
                        {
                            SpeakerId = speaker.Id,
                            ContributionId = contribution.Id,
                        });
                    }
                }
                _queue.Enqueue(contribution.Id.ToString());
                
                Console.WriteLine($"Update contribution: {contribution.Id}");
            }
            else
            {
                var id = Guid.NewGuid();
                contribution = new Contribution
                {
                    Id = id,
                    StartDate = request.Dto.StartDate,
                    EndDate = request.Dto.EndDate,
                    Title = request.Dto.Title,
                    Abstract = request.Dto.Abstract
                };
                foreach (var speaker in request.Dto.Speakers)
                {
                    contribution.ContributionSpeakers.Add(new ContributionSpeaker
                    {
                        SpeakerId = speaker.Id,
                        ContributionId = id,
                    });
                }
                await _dbContext.Contributions.AddAsync(contribution);
            }

            await _dbContext.SaveChangesAsync();
        }

        public async IAsyncEnumerable<string> UpdatedContributionAsync(CallContext context = default)
        {
            Console.WriteLine("UpdatedContributionAsync is called.");
            while (!context.CancellationToken.IsCancellationRequested)
            {
                _queue.TryDequeue(out string? item);
                if (!String.IsNullOrWhiteSpace(item))
                {
                    Console.WriteLine($"UpdatedContributionAsync throw new id {item}.");
                    yield return item;
                }
                await Task.Delay(100);
            }
            Console.WriteLine("UpdatedContributionAsync is finished.");
        }

        public async Task DeleteContributionAsync(IdRequest request)
        {
        
            var contribution = await _dbContext.Contributions
                .FirstOrDefaultAsync(c => c.Id == request.Id);

            if (contribution is not null)
            {
                _dbContext.Contributions.Remove(contribution);
            }
        }
    }
}
