using Fluxor;
using ConfTool.Shared.Models;
using System.Net.Http.Json;

namespace ConfTool.Client.Features.Conferences.Store
{
    public class LoadConferencesEffect : Effect<LoadConferencesAction>
    {
        private readonly HttpClient _httpClient;

        public LoadConferencesEffect(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public override async Task HandleAsync(LoadConferencesAction action, IDispatcher dispatcher)
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<IEnumerable<ConferenceDto>>("/api/v1/conferences");
                dispatcher.Dispatch(new LoadConferencesActionSuccess(result?.ToList() ?? new List<ConferenceDto>()));
            }
            catch (Exception ex)
            {
                dispatcher.Dispatch(new LoadConferencesActionFailed($"Load Conferences failed. Error: {ex.Message}"));
            }
        }
    }
}
