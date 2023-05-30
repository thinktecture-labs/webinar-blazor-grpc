using System.ServiceModel;
using ConfTool.Shared.Models;
using ProtoBuf.Grpc;

namespace ConfTool.Shared.Services;

[ServiceContract]
public interface ISpeakersService
{
    [OperationContract]
    Task<IList<SpeakerDto>> GetSpeakersAsync(CollectionRequest request, CallContext context = default);
    [OperationContract]
    Task<SpeakerDto?> GetSpeakerAsync(IdRequest request, CallContext context = default);
    [OperationContract]
    Task AddOrUpdateSpeakerAsync(AddOrUpdateRequest<SpeakerDto> request, CallContext context = default);
    [OperationContract]
    Task DeleteSpeakerAsync(IdRequest request, CallContext context = default);
}