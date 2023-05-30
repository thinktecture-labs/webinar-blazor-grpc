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

        public async Task<ICollection<ContributionDto>> GetContributionsAsync(int skip = 0, int take = 100, string? searchTerm = null, CancellationToken cancellationToken = default)
        {
            var query = string.IsNullOrWhiteSpace(searchTerm)
                ? _dbContext.Contributions
                    .Include(c => c.ContributionSpeakers)
                    .ThenInclude(cs => cs.Speaker)
                : _dbContext.Contributions
                    .Include(c => c.ContributionSpeakers)
                    .ThenInclude(cs => cs.Speaker)
                    .Where(c => 
                        string.Join("",c.Title, c.Abstract)
                            .Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase)
                    );

            var result = await query
                .OrderBy(c => c.StartDate)
                .Skip(skip)
                .Take(take)
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
                }).ToListAsync(cancellationToken);
            return result;
        }

        public async Task<ContributionDto?> GetContributionAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var contribution = await _dbContext.Contributions
                .Include(c => c.ContributionSpeakers)
                .ThenInclude(cs => cs.Speaker)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
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

        public async Task AddOrUpdateContributionAsync(ContributionDto dto, CancellationToken cancellationToken = default)
        {
            if (dto is null)
            {
                return;
            }
            
            var contribution = await _dbContext.Contributions
                .Include(c => c.ContributionSpeakers)
                .FirstOrDefaultAsync(c => c.Id == dto.Id, cancellationToken);
            if (contribution is not null)
            {
                contribution.StartDate = dto.StartDate;
                contribution.EndDate = dto.EndDate;
                contribution.Title = dto.Title;
                contribution.Abstract = dto.Abstract;
                foreach (var speaker in dto.Speakers)
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
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    Title = dto.Title,
                    Abstract = dto.Abstract
                };
                foreach (var speaker in dto.Speakers)
                {
                    contribution.ContributionSpeakers.Add(new ContributionSpeaker
                    {
                        SpeakerId = speaker.Id,
                        ContributionId = id,
                    });
                }
                await _dbContext.Contributions.AddAsync(contribution, cancellationToken);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
