using Newtonsoft.Json;
using Questao2;

public class Program
{
    public static void Main()
    {
        // A partir da versão 6 do C#, para "using" de classes que implementam IDisposable
        // não é mais necessário usar o bloco "using", apenas o "using" simples
        using GoalsClient goalsClient = new GoalsClient();
        string teamName = "Paris Saint-Germain";
        int year = 2013;
        int totalGoals = getTotalScoredGoals(teamName, year, goalsClient);

        Console.WriteLine(
            "Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year
        );

        teamName = "Chelsea";
        year = 2014;
        totalGoals = getTotalScoredGoals(teamName, year, goalsClient);

        Console.WriteLine(
            "Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year
        );

        // Output expected:
        // Team Paris Saint - Germain scored 109 goals in 2013
        // Team Chelsea scored 92 goals in 2014
    }

    public static int getTotalScoredGoals(string team, int year, GoalsClient goalsClient)
    {
        return goalsClient.GetGoalsByTeamInYear(team, year).Result;
    }
}
