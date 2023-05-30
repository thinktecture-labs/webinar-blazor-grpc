using ConfTool.Server.Database;
using ConfTool.Shared.Models;
using ConfTool.Shared.Services;
using Microsoft.EntityFrameworkCore;

namespace ConfTool.Server.Services
{
    public class ConferencesService : IConferencesService
    {
        private readonly ConfToolDbContext _dbContext;

        public ConferencesService(ConfToolDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<int> GetConferencesCountAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Conferences.CountAsync(cancellationToken);
        }

        public async Task<IList<ConferenceDto>> GetConferencesAsync(CollectionRequest request)
        {
            var result = await _dbContext.Conferences
                .OrderByDescending(c => c.StartDate)
                .Skip(request.Skip)
                .Take(request.Take)
                .Select(conference => new ConferenceDto
                    {
                        Id = conference.Id,
                        Title = conference.Title,
                        Description = conference.Description,
                        StartDate = conference.StartDate,
                        EndDate = conference.EndDate,
                    }
                ).ToListAsync();
            return result?.ToList() ?? new List<ConferenceDto>();
        }

        public async Task<ConferenceDto?> GetConferenceAsync(IdRequest request)
        {
            var conference = await _dbContext.Conferences
                .FirstOrDefaultAsync(c => c.Id == request.Id);
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

        public async Task AddOrUpdateConferenceAsync(AddOrUpdateRequest<ConferenceDto> request)
        {
            if (request.Dto is null)
            {
                return;
            }
            var conference = await _dbContext.Conferences.FirstOrDefaultAsync(c => c.Id == request.Dto.Id);
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
                    Id = request.Dto.Id,
                    Title = request.Dto.Title,
                    Description = request.Dto.Description,
                    StartDate = request.Dto.StartDate,
                    EndDate = request.Dto.EndDate
                };
                await _dbContext.Conferences.AddAsync(conference);
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteConferenceAsync(IdRequest request)
        {
            
            var conference = await _dbContext.Conferences
                .FirstOrDefaultAsync(c => c.Id == request.Id);

            if (conference is not null)
            {
                _dbContext.Conferences.Remove(conference);
            }
        }
    }
}
