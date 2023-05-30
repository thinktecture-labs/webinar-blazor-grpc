using System.ServiceModel;
using ConfTool.Shared.Models;

namespace ConfTool.Shared.Services;

[ServiceContract]
public interface IConferencesService
{
    [OperationContract]
    Task<IList<ConferenceDto>> GetConferencesAsync(CollectionRequest request);
    
    [OperationContract]
    Task<ConferenceDto?> GetConferenceAsync(IdRequest request);
    
    [OperationContract]
    Task AddOrUpdateConferenceAsync(AddOrUpdateRequest<ConferenceDto> request);
    
    [OperationContract]
    Task DeleteConferenceAsync(IdRequest request);
}