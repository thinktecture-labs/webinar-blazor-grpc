using ConfTool.Server.Database;
using ConfTool.Shared.Models;
using ConfTool.Shared.Services;
using Microsoft.EntityFrameworkCore;
using ProtoBuf.Grpc;

namespace ConfTool.Server.Services
{
    public class ConferencesService : IConferencesService
    {
        private readonly ConfToolDbContext _dbContext;

        public ConferencesService(ConfToolDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }


        public async Task<IList<ConferenceDto>> GetConferencesAsync(int skip = 0, int take = 100, CancellationToken cancellationToken = default)
        {
            var result = await _dbContext.Conferences
                .OrderByDescending(c => c.StartDate)
                .Skip(skip)
                .Take(take)
                .Select(conference => new ConferenceDto
                    {
                        Id = conference.Id,
                        Title = conference.Title,
                        Description = conference.Description,
                        StartDate = conference.StartDate,
                        EndDate = conference.EndDate,
                    }
                ).ToListAsync(cancellationToken);
            return result?.ToList() ?? new List<ConferenceDto>();
        }

        public async Task<ConferenceDto?> GetConferenceAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var conference = await _dbContext.Conferences
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
            return conference is not null
                ? new ConferenceDto
                {
                    Id = conference.Id,
                    Title = conference.Title,
                    Description = conference.Description,
                    StartDate = conference.StartDate,
                    EndDate = conference.EndDate
                }
                : null;
        }

        public async Task AddOrUpdateConferenceAsync(ConferenceDto dto, CancellationToken cancellationToken = default)
        {
            if (dto is null)
            {
                return;
            }
            var conference = await _dbContext.Conferences.FirstOrDefaultAsync(c => c.Id == dto.Id);
            if (conference is not null)
            {
                conference.Id = conference.Id;
                conference.Title = conference.Title;
                conference.Description = conference.Description;
                conference.StartDate = conference.StartDate;
                conference.EndDate = conference.EndDate;
            }
            else
            {
                var id = Guid.NewGuid();
                conference = new Conference
                {
                    Id = dto.Id,
                    Title = dto.Title,
                    Description = dto.Description,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate
                };
                await _dbContext.Conferences.AddAsync(conference, cancellationToken);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
