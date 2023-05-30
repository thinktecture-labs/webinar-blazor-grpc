using System.ServiceModel;
using ConfTool.Shared.Models;
using ProtoBuf.Grpc;

namespace ConfTool.Shared.Services;

public interface ISpeakersService
{
    Task<IList<SpeakerDto>> GetSpeakersAsync(int skip = 0, int take = 1000, CancellationToken cancellationToken = default);
    Task<SpeakerDto?> GetSpeakerAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddOrUpdateSpeakerAsync(SpeakerDto dto, CancellationToken cancellationToken = default);
}