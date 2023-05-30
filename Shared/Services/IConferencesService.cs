using System.ServiceModel;
using ConfTool.Shared.Models;
using ProtoBuf.Grpc;

namespace ConfTool.Shared.Services;

[ServiceContract]
public interface IConferencesService
{
    [OperationContract]
    Task<IList<ConferenceDto>> GetConferencesAsync(CollectionRequest request, CallContext context = default);
    
    [OperationContract]
    Task<ConferenceDto?> GetConferenceAsync(IdRequest request, CallContext context = default);
    
    [OperationContract]
    Task AddOrUpdateConferenceAsync(AddOrUpdateRequest<ConferenceDto> request, CallContext context = default);
    
    [OperationContract]
    Task DeleteConferenceAsync(IdRequest request, CallContext context = default);
}