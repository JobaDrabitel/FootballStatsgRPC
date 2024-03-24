using Grpc.Core;

namespace gRPClient.Clients;

public class TeamStatsClient(TeamStats.TeamStatsClient client)
{ 
    public async Task GetTeamStatsAsync(int number = 0, string name = "")
    {
        var reply = await client.GetTeamStatsAsync(new TeamRequest { Number = number });
        Console.WriteLine($"Ответ сервера: {reply}");
    }
    
    public async Task GetTeamsStatsStreamAsync()
    {
        using var call = client.GetTeamsStatsStream(new TeamRequest());
        await foreach (var response in call.ResponseStream.ReadAllAsync())
        {
            Console.WriteLine($"Команда: {response.Name}, Матчи: {response.Matches}, Победы: {response.Wins}, Ничьи: {response.Draws}, Поражения: {response.Losses}, Забито голов: {response.GoalsFor}, Пропущено голов: {response.GoalsAgainst}, Очки: {response.Points}, xG: {response.XG}, xGA: {response.XGA}, xPTS: {response.XPTS}");
        }
    }

    public async Task GetTeamStatsClientStreamAsync(List<int>? numbers = null)
    {
        if(numbers is null)
            return;
        
        using var call = client.GetTeamsStatsClientStream();
        try
        {
            foreach (var number in numbers)
            {
                await call.RequestStream.WriteAsync(new TeamRequest { Number = number });
            }

            await call.RequestStream.CompleteAsync();
            var streamResponse = await call.ResponseAsync;
            foreach (var response in streamResponse.Responses)
            {
                Console.WriteLine(
                    $"Команда: {response.Name}, Матчи: {response.Matches}, Победы: {response.Wins}, Ничьи: {response.Draws}, Поражения: {response.Losses}, Забито голов: {response.GoalsFor}, Пропущено голов: {response.GoalsAgainst}, Очки: {response.Points}, xG: {response.XG}, xGA: {response.XGA}, xPTS: {response.XPTS}");
            }

        }
        catch (RpcException e) when (e.StatusCode == StatusCode.Cancelled)
        {
            Console.WriteLine("Stream cancelled.");
        }
    }
}