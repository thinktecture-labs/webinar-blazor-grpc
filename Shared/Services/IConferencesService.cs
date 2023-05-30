using System.ServiceModel;
using ConfTool.Shared.Models;
using ProtoBuf.Grpc;

namespace ConfTool.Shared.Services;

public interface IConferencesService
{
    Task<IList<ConferenceDto>> GetConferencesAsync(int skip = 0, int take = 1000, CancellationToken cancellationToken = default);
    Task<ConferenceDto?> GetConferenceAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddOrUpdateConferenceAsync(ConferenceDto dto, CancellationToken cancellationToken = default);
}