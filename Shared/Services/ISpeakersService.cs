using System.ServiceModel;
using ConfTool.Shared.Models;

namespace ConfTool.Shared.Services;

[ServiceContract]
public interface ISpeakersService
{
    [OperationContract]
    Task<IList<SpeakerDto>> GetSpeakersAsync(CollectionRequest request);
    [OperationContract]
    Task<SpeakerDto?> GetSpeakerAsync(IdRequest request);
    [OperationContract]
    Task AddOrUpdateSpeakerAsync(AddOrUpdateRequest<SpeakerDto> request);
    [OperationContract]
    Task DeleteSpeakerAsync(IdRequest request);
}