using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Questao2
{
    public class GoalsClient : IDisposable
    {
        private readonly HttpClient _httpClient;

        public GoalsClient() => _httpClient = new HttpClient();

        public async Task<int> GetGoalsByTeamInYear(string teamName, int year)
        {
            int currentPage = 1;
            int totalGoals = 0;
            bool hasRemainingPages = true;

            // Parte 1 - Filtra como team1, time mandante e soma os gols em todas as paginas
            while (hasRemainingPages)
            {
                var response = await _httpClient.GetAsync(
                    $"https://jsonmock.hackerrank.com/api/football_matches?year={year}&team1={teamName}&page={currentPage}"
                );
                var result = await response.Content.ReadFromJsonAsync<FootballResponse>();

                totalGoals += result.Data.Sum(m => m.Team1goals);

                if (currentPage >= result.TotalPages)
                {
                    hasRemainingPages = false;
                }
                currentPage++;
            }
            // Parte 2 - Filtra como team2, time visitante e complementa a soma dos gols como mandante
            // com a soma em todas as paginas como visitante
            currentPage = 1;
            hasRemainingPages = true;

            while (hasRemainingPages)
            {
                var response = await _httpClient.GetAsync(
                    $"https://jsonmock.hackerrank.com/api/football_matches?year={year}&team2={teamName}&page={currentPage}"
                );
                var result = await response.Content.ReadFromJsonAsync<FootballResponse>();

                totalGoals += result.Data.Sum(m => m.Team2goals);

                if (currentPage >= result.TotalPages)
                {
                    hasRemainingPages = false;
                }
                currentPage++;
            }

            return totalGoals;
        }

        private class FootballResponse
        {
            public List<Match> Data { get; set; }
            [JsonPropertyName("total_pages")]
            public int TotalPages { get; set; }
        }

        private class Match
        {
            // [Newtonsoft.Json.JsonConstructor]
            // private Match(int team1goals, int team2goals)
            // {
            //     this.Team1goals = team1goals;
            //     this.Team2goals = team2goals;
            // }

            [JsonPropertyName("team1goals")]
            public int Team1goals { get; set; }

            [JsonPropertyName("team2goals")]
            public int Team2goals { get; set; }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
