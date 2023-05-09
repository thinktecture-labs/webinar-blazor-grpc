using System.Runtime.Serialization;
using System.ServiceModel;
using ConfTool.Shared.Models;
using ProtoBuf.Grpc;

namespace ConfTool.Shared.Services;

[DataContract]
public class CollectionRequest
{
    [DataMember(Order = 1)] public int Skip { get; set; }

    [DataMember(Order = 2)] public int Take { get; set; }

    [DataMember(Order = 3)] public string SearchTerm { get; set; } = string.Empty;
}

[DataContract]
public class IdRequest
{
    [DataMember(Order = 1)]
    public Guid Id { get; set; }
}

[DataContract]
public class AddOrUpdateRequest<T>
{
    [DataMember(Order = 1)]
    public Guid Id { get; set; }

    [DataMember(Order = 2)]
    public T? Dto { get; set; }
}

[ServiceContract]
public interface IContributionService
{
    [OperationContract]
    Task<ICollection<ContributionDto>> GetContributionsAsync(CollectionRequest request, CallContext context = default);
    
    [OperationContract]
    Task<ContributionDto?> GetContributionAsync(IdRequest request, CallContext context = default);
    
    [OperationContract]
    Task AddOrUpdateContributionAsync(AddOrUpdateRequest<ContributionDto> request, CallContext context = default);
    
    [OperationContract]
    IAsyncEnumerable<string> UpdatedContributionAsync(CallContext context = default);
    
    [OperationContract]
    Task DeleteContributionAsync(IdRequest request, CallContext context = default);
}