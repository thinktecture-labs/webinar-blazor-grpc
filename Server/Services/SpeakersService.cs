using ConfTool.Server.Database;
using ConfTool.Shared.Models;
using ConfTool.Shared.Services;
using Microsoft.EntityFrameworkCore;
using ProtoBuf.Grpc;

namespace ConfTool.Server.Services
{
    public class SpeakersService : ISpeakersService
    {
        private readonly ConfToolDbContext _dbContext;

        public SpeakersService(ConfToolDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<IList<SpeakerDto>> GetSpeakersAsync(int skip = 0, int take = 100, CancellationToken cancellationToken = default)
        {
            var result = await _dbContext.Speakers
                .OrderBy(c => c.FirstName).ThenBy(c => c.LastName)
                .Skip(skip)
                .Take(take)
                .Select(speaker => new SpeakerDto
                {
                    Id = speaker.Id,
                    FirstName = speaker.FirstName,
                    LastName = speaker.LastName,
                    ImageUrl = speaker.PictureUrl,
                    Abstract = speaker.Abstract
                }
                ).ToListAsync(cancellationToken);
            return result?.ToList() ?? new List<SpeakerDto>();
        }

        public async Task<SpeakerDto?> GetSpeakerAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var speaker = await _dbContext.Speakers
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
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

        public async Task AddOrUpdateSpeakerAsync(SpeakerDto dto, CancellationToken cancellationToken = default)
        {
            if (dto is null)
            {
                return;
            }
            
            var speaker = await _dbContext.Speakers.FirstOrDefaultAsync(c => c.Id == dto.Id, cancellationToken);
            if (speaker is not null)
            {
                speaker.FirstName = dto.FirstName;
                speaker.LastName = dto.LastName;
                speaker.PictureUrl = dto.ImageUrl;
                speaker.Abstract = dto.Abstract;
            }
            else
            {
                var id = Guid.NewGuid();
                speaker = new Speaker
                {
                    Id = id,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    PictureUrl = dto.ImageUrl,
                    Abstract = dto.Abstract
                };
                await _dbContext.Speakers.AddAsync(speaker, cancellationToken);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

        }
    }
}
