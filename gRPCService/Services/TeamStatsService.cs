using System.Globalization;
using Grpc.Core;
using OfficeOpenXml;

namespace gRPCService.Services;

public class TeamStatsService : TeamStats.TeamStatsBase
{
    private const string File = @"E:\Coding\Projects\C#\TestgRPC\gRPCService\EPL.xlsx";

    public override Task<TeamResponse> GetTeamStats(TeamRequest teamNameOrNumber, ServerCallContext context)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using var package = new ExcelPackage(File);
        var worksheet = package.Workbook.Worksheets[0];
        for (var row = 2; row <= worksheet.Dimension.End.Row; row++)
        {
            var number = worksheet.Cells[row, 1].Value;
            var name = worksheet.Cells[row, 2].Value.ToString();

            if (name == teamNameOrNumber.Name || Convert.ToInt32(number) == teamNameOrNumber.Number)
                return Task.FromResult(new TeamResponse
                {
                    Name = name,
                    Matches = int.Parse(worksheet.Cells[row, 3].Value?.ToString() ?? "0"),
                    Wins = int.Parse(worksheet.Cells[row, 4].Value?.ToString() ?? "0"),
                    Draws = int.Parse(worksheet.Cells[row, 5].Value?.ToString() ?? "0"),
                    Losses = int.Parse(worksheet.Cells[row, 6].Value?.ToString() ?? "0"),
                    GoalsFor = int.Parse(worksheet.Cells[row, 7].Value?.ToString() ?? "0"),
                    GoalsAgainst = int.Parse(worksheet.Cells[row, 8].Value?.ToString() ?? "0"),
                    Points = int.Parse(worksheet.Cells[row, 9].Value?.ToString() ?? "0"),
                    XG = double.Parse(worksheet.Cells[row, 10].Value?.ToString() ?? "0", CultureInfo.InvariantCulture),
                    XGA = double.Parse(worksheet.Cells[row, 11].Value?.ToString() ?? "0", CultureInfo.InvariantCulture),
                    XPTS = double.Parse(worksheet.Cells[row, 12].Value?.ToString() ?? "0", CultureInfo.InvariantCulture)
                });
        }

        return Task.FromResult(new TeamResponse());
    }

    public override async Task GetTeamsStatsStream(TeamRequest request,
        IServerStreamWriter<TeamResponse> responseStream, ServerCallContext context)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using var package = new ExcelPackage(File);
        var worksheet = package.Workbook.Worksheets[0];

        for (var row = 2; row <= worksheet.Dimension.End.Row; row++)
        {
            var name = worksheet.Cells[row, 2].Value.ToString();

            var teamResponse = new TeamResponse
            {
                Name = name,
                Matches = int.Parse(worksheet.Cells[row, 3].Value?.ToString() ?? "0"),
                Wins = int.Parse(worksheet.Cells[row, 4].Value?.ToString() ?? "0"),
                Draws = int.Parse(worksheet.Cells[row, 5].Value?.ToString() ?? "0"),
                Losses = int.Parse(worksheet.Cells[row, 6].Value?.ToString() ?? "0"),
                GoalsFor = int.Parse(worksheet.Cells[row, 7].Value?.ToString() ?? "0"),
                GoalsAgainst = int.Parse(worksheet.Cells[row, 8].Value?.ToString() ?? "0"),
                Points = int.Parse(worksheet.Cells[row, 9].Value?.ToString() ?? "0"),
                XG = double.Parse(worksheet.Cells[row, 10].Value?.ToString() ?? "0", CultureInfo.InvariantCulture),
                XGA = double.Parse(worksheet.Cells[row, 11].Value?.ToString() ?? "0", CultureInfo.InvariantCulture),
                XPTS = double.Parse(worksheet.Cells[row, 12].Value?.ToString() ?? "0", CultureInfo.InvariantCulture)
            };

            await responseStream.WriteAsync(teamResponse);

            if (context.CancellationToken.IsCancellationRequested) break;
        }
    }

    public override async  Task<EplTeamsListResponse> GetTeamsStatsClientStream(IAsyncStreamReader<TeamRequest> requestStream,
        ServerCallContext serverCallContext)
    {
        var response = new EplTeamsListResponse();
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using var package = new ExcelPackage(File);
        var worksheet = package.Workbook.Worksheets[0];

        while (await requestStream.MoveNext(serverCallContext.CancellationToken))
        {
            var currentRequest = requestStream.Current;
            for (var row = 2; row <= worksheet.Dimension.End.Row; row++)
            {
                var number = worksheet.Cells[row, 1].Value;
                var name = worksheet.Cells[row, 2].Value.ToString();

                if (name == currentRequest.Name || Convert.ToInt32(number) == currentRequest.Number)
                {
                    response.Responses.Add(new TeamResponse
                    {
                        Name = name,
                        Matches = int.Parse(worksheet.Cells[row, 3].Value?.ToString() ?? "0"),
                        Wins = int.Parse(worksheet.Cells[row, 4].Value?.ToString() ?? "0"),
                        Draws = int.Parse(worksheet.Cells[row, 5].Value?.ToString() ?? "0"),
                        Losses = int.Parse(worksheet.Cells[row, 6].Value?.ToString() ?? "0"),
                        GoalsFor = int.Parse(worksheet.Cells[row, 7].Value?.ToString() ?? "0"),
                        GoalsAgainst = int.Parse(worksheet.Cells[row, 8].Value?.ToString() ?? "0"),
                        Points = int.Parse(worksheet.Cells[row, 9].Value?.ToString() ?? "0"),
                        XG = double.Parse(worksheet.Cells[row, 10].Value?.ToString() ?? "0", CultureInfo.InvariantCulture),
                        XGA = double.Parse(worksheet.Cells[row, 11].Value?.ToString() ?? "0", CultureInfo.InvariantCulture),
                        XPTS = double.Parse(worksheet.Cells[row, 12].Value?.ToString() ?? "0", CultureInfo.InvariantCulture)
                    });
                }
            }
        }

        return response;
    }
}