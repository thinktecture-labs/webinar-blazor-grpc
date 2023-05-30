using Fluxor;
using ConfTool.Shared.Models;
using static ConfTool.Client.Features.Speakers.Store.SpeakerActions;
using System.Net.Http.Json;

namespace ConfTool.Client.Features.Speakers.State
{
    public class SpeakersEffect : Effect<LoadSpeakersAction>
    {
        private readonly HttpClient _httpClient;

        public SpeakersEffect(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public override async Task HandleAsync(LoadSpeakersAction action, IDispatcher dispatcher)
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<IEnumerable<SpeakerDto>>("/api/v1/speakers");
                dispatcher.Dispatch(new LoadSpeakersActionSuccess(result?.ToList() ?? new List<SpeakerDto>()));
            }
            catch (Exception ex)
            {
                dispatcher.Dispatch(new LoadSpeakersActionFailed($"Load Speakers failed. Error: {ex.Message}"));
            }
        }
    }
}
