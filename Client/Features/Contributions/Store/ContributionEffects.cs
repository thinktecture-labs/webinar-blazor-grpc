using Fluxor;
using ConfTool.Shared.Models;
using ConfTool.Shared.Services;
using static ConfTool.Client.Features.Contributions.Store.ContributionActions;
using System.Net.Http.Json;
using System.Net.Http;
using Microsoft.VisualBasic;

namespace ConfTool.Client.Features.Contributions.Store
{
    public class ContributionsEffect : Effect<LoadContributionsAction>
    {
        private readonly HttpClient _httpClient;

        public ContributionsEffect(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public override async Task HandleAsync(LoadContributionsAction action, IDispatcher dispatcher)
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<IEnumerable<ContributionDto>>($"/api/v1/contributions?searchTerm={action.SearchTerm}");
                dispatcher.Dispatch(new LoadContributionsActionSuccess(result?.ToList() ?? new List<ContributionDto>()));;
            }
            catch (Exception ex)
            {
                dispatcher.Dispatch(new LoadContributionsActionFailed($"Load Contributions failed. Error: {ex.Message}"));
            }
        }
    }

    public class SaveContributionEffect : Effect<SaveContributionAction>
    {
        private readonly HttpClient _httpClient;

        public SaveContributionEffect(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public override async Task HandleAsync(SaveContributionAction action, IDispatcher dispatcher)
        {
            try
            {
                Console.WriteLine($"Speakers: {action.Contribution.Speakers.Count}");

                var result = await _httpClient.PostAsJsonAsync("/api/v1/contributions", action.Contribution);
                dispatcher.Dispatch(new SaveContributionSuccessAction());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Load Contributions failed. Error: {ex.Message}");
            }
        }
    }
}
