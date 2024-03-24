using gRPClient;
using gRPClient.Clients;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpcClient<TeamStats.TeamStatsClient>(o =>
{
    o.Address = new Uri("http://localhost:5163"); // gRPC Server address
});
builder.Services.AddSingleton<TeamStatsClient>();
var app = builder.Build();
var teamStatsClient = app.Services.GetRequiredService<TeamStatsClient>();
await teamStatsClient.GetTeamStatsAsync(1);
await teamStatsClient.GetTeamsStatsStreamAsync();
await teamStatsClient.GetTeamStatsClientStreamAsync(new List<int> { 1, 2, 4, 8, 12 });
app.Run();