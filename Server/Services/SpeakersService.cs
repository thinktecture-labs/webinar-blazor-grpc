using ConfTool.Server.Database;
using ConfTool.Shared.Models;
using ConfTool.Shared.Services;
using Microsoft.EntityFrameworkCore;

namespace ConfTool.Server.Services
{
    public class SpeakersService : ISpeakersService
    {
        private readonly ConfToolDbContext _dbContext;

        public SpeakersService(ConfToolDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<int> GetSpeakersCountAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Speakers.CountAsync(cancellationToken);
        }

        public async Task<IList<SpeakerDto>> GetSpeakersAsync(CollectionRequest request)
        {
            var result = await _dbContext.Speakers
                .OrderBy(c => c.FirstName).ThenBy(c => c.LastName)
                .Skip(request.Skip)
                .Take(request.Take)
                .Select(speaker => new SpeakerDto
                {
                    Id = speaker.Id,
                    FirstName = speaker.FirstName,
                    LastName = speaker.LastName,
                    ImageUrl = speaker.PictureUrl,
                    Abstract = speaker.Abstract
                }
                ).ToListAsync();
            return result?.ToList() ?? new List<SpeakerDto>();
        }

        public async Task<SpeakerDto?> GetSpeakerAsync(IdRequest request)
        {
            var speaker = await _dbContext.Speakers
                .FirstOrDefaultAsync(c => c.Id == request.Id);
            return speaker is not null
                ? new SpeakerDto
                {
                    Id = speaker.Id,
                    FirstName = speaker.FirstName,
                    LastName = speaker.LastName,
                    ImageUrl = speaker.PictureUrl,
                    Abstract = speaker.Abstract
                }
                : null;
        }

        public async Task AddOrUpdateSpeakerAsync(AddOrUpdateRequest<SpeakerDto> request)
        {
            if (request.Dto is null)
            {
                return;
            }
            
            var speaker = await _dbContext.Speakers.FirstOrDefaultAsync(c => c.Id == request.Dto.Id);
            if (speaker is not null)
            {
                speaker.FirstName = request.Dto.FirstName;
                speaker.LastName = request.Dto.LastName;
                speaker.PictureUrl = request.Dto.ImageUrl;
                speaker.Abstract = request.Dto.Abstract;
            }
            else
            {
                var id = Guid.NewGuid();
                speaker = new Speaker
                {
                    Id = id,
                    FirstName = request.Dto.FirstName,
                    LastName = request.Dto.LastName,
                    PictureUrl = request.Dto.ImageUrl,
                    Abstract = request.Dto.Abstract
                };
                await _dbContext.Speakers.AddAsync(speaker);
            }

            await _dbContext.SaveChangesAsync();

        }

        public async Task DeleteSpeakerAsync(IdRequest request)
        {
            var speaker = await _dbContext.Speakers
                .FirstOrDefaultAsync(c => c.Id == request.Id);

            if (speaker is not null)
            {
                _dbContext.Speakers.Remove(speaker);
            }
        }
    }
}
