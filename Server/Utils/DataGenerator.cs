using System.Reflection;
using System.Text.Json;
using System.Text;
using System.Text.Json.Serialization;
using ConfTool.Server.Database;
using Microsoft.EntityFrameworkCore;

namespace ConfTool.Server.Utils
{
    public record Name(
        [property: JsonPropertyName("title")] string Title,
        [property: JsonPropertyName("first")] string First,
        [property: JsonPropertyName("last")] string Last
    );

    public record Picture(
        [property: JsonPropertyName("large")] string Large,
        [property: JsonPropertyName("medium")] string Medium,
        [property: JsonPropertyName("thumbnail")]
        string Thumbnail
    );

    public record RandomUser(
        [property: JsonPropertyName("gender")] string Gender,
        [property: JsonPropertyName("name")] Name Name,
        [property: JsonPropertyName("picture")]
        Picture Picture
    );

    public record Root(
        [property: JsonPropertyName("results")]
        IReadOnlyList<RandomUser> Results
    );

    public class DataGenerator
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvIder)
        {
            var data = await LoadDataAsync();
            if (data == null )
            {
                return;
            }

            using (var context = new ConfToolDbContext(
                       serviceProvIder.GetRequiredService<DbContextOptions<ConfToolDbContext>>()))
            using (var httpClient = serviceProvIder.GetRequiredService<HttpClient>())
            {
                foreach (var item in data.Conferences) 
                {
                    var date = item.CfpStart.HasValue ? item.CfpStart.Value.ToString("d") : DateTime.Now.ToString("d");
                    context.Conferences.Add(new Conference
                    {
                        Id = Guid.NewGuid(),
                        Title = item.Title,
                        Description = $"{item.City} {item.Country} | {date}",
                        City = item.City,
                        Country = item.Country,
                        StartDate = item.CfpStart ?? DateTime.Now,
                        EndDate = item.CfpDeadline ?? DateTime.Now,
                    });
                }

                var speakers = new List<Speaker>();
                if (context.Speakers.Any())
                {
                    speakers = context.Speakers.ToList();
                }
                else
                {
                    Root? randomUsers = null;
                    try
                    {
                        //randomUsers = await httpClient.GetFromJsonAsync<Root>(
                        //    "https://randomuser.me/api/?inc=gender,name,picture&nat=de&results=150&noinfo");
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Random user currently not abailable");
                    }

                    if (randomUsers == null)
                    {
                        foreach(var speaker in data.Speaker)
                        {
                            speakers.Add(new Speaker
                            {
                                Id = Guid.NewGuid(),
                                FirstName = speaker.FirstName,
                                LastName = speaker.LastName,
                                PictureUrl = speaker.PictureUrl ?? string.Empty
                            });
                        }
                    }
                    else
                    {
                        speakers = randomUsers.Results.Select(user => new Speaker
                        {
                            Id = Guid.NewGuid(),
                            FirstName = user.Name.First,
                            LastName = user.Name.Last,
                            PictureUrl = user.Picture.Large
                        }).ToList();
                    }

                    context.Speakers.AddRange(speakers);
                    context.SaveChanges();
                }

                if (!context.Contributions.Any())
                {
                    foreach(var contribution in data.Contributions)
                    {
                        var random = new Random();
                        var id = Guid.NewGuid();
                        DateTime.TryParse(contribution.Date, out var startDate);
                        var endDate = startDate.AddDays(1);
                        context.Contributions.Add(new Contribution
                        {
                            Id = id,
                            Title = contribution.Title,
                            Abstract = contribution.Abstract,
                            StartDate = startDate,
                            EndDate = endDate,
                        });

                        var count = random.Next(1, 3);
                        var max = speakers.Count - 1;
                        var currentSpeakerId = random.Next(0, max);
                        for (int j = 0; j < count; j++)
                        {
                            var newId = random.Next(0, max);
                            while (currentSpeakerId == newId)
                            {
                                newId = random.Next(0, max);
                            }

                            currentSpeakerId = newId;
                            try
                            {
                                context.ContributionSpeakers.Add(new ContributionSpeaker
                                {
                                    ContributionId = id,
                                    SpeakerId = speakers[currentSpeakerId].Id
                                });
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(
                                    $"Failed to add {id} , {speakers[currentSpeakerId].Id}. Error: {e.Message}");
                            }
                        }

                        context.SaveChanges();
                    }
                }
            }
        }

        private static async Task<SampleDataRoot?> LoadDataAsync()
        {
            var assembly = Assembly.GetEntryAssembly();
            var resourceStream = assembly?.GetManifestResourceStream("ConfTool.Server.SampleData.contributions.json");
            if (resourceStream == null)
            {
                return null;
            }

            using var reader = new StreamReader(resourceStream, Encoding.UTF8);
            var jsonString = await reader.ReadToEndAsync();
            return JsonSerializer.Deserialize<SampleDataRoot>(jsonString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }
}