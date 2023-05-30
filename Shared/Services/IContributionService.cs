using System.Runtime.Serialization;
using System.ServiceModel;
using ConfTool.Shared.Models;
using ProtoBuf.Grpc;

namespace ConfTool.Shared.Services;


public interface IContributionService
{
    Task<ICollection<ContributionDto>> GetContributionsAsync(int skip = 0, int take = 1000, string? searchTerm = null, CancellationToken cancellationToken = default);

    Task<ContributionDto?> GetContributionAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task AddOrUpdateContributionAsync(ContributionDto dto, CancellationToken cancellationToken = default);
}