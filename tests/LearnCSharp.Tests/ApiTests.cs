using System.Net;
using System.Net.Http.Json;
using LearnCSharp.Models;
using Microsoft.AspNetCore.Mvc.Testing;

namespace LearnCSharp.Tests;

public class ApiTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Health_returns_ok()
    {
        HealthReport? report = await _client.GetFromJsonAsync<HealthReport>("/health");

        Assert.NotNull(report);
        Assert.Equal("ok", report.Status);
        Assert.StartsWith(".NET", report.Runtime);
    }

    [Fact]
    public async Task Runtime_reports_processor_count()
    {
        RuntimeInfo? info = await _client.GetFromJsonAsync<RuntimeInfo>("/api/runtime");

        Assert.NotNull(info);
        Assert.True(info.ProcessorCount >= 1);
    }

    [Fact]
    public async Task Roadmap_streams_seven_stages()
    {
        List<RoadmapStage>? stages = await _client.GetFromJsonAsync<List<RoadmapStage>>("/api/roadmap");

        Assert.NotNull(stages);
        Assert.Equal(7, stages.Count);
        Assert.Equal(StageLevel.Staff, stages[^1].Level);
    }

    [Fact]
    public async Task Unknown_stage_is_a_404_problem()
    {
        HttpResponseMessage response = await _client.GetAsync("/api/roadmap/42");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Stats_summarizes_numbers()
    {
        StatsSummary? stats = await _client.GetFromJsonAsync<StatsSummary>("/api/stats?values=2,4,4,4,5,5,7,9");

        Assert.Equal(new StatsSummary(8, 40, 5, 4, 4.5), stats);
    }

    [Fact]
    public async Task Stats_rejects_bad_input()
    {
        HttpResponseMessage response = await _client.GetAsync("/api/stats?values=1,x");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Endpoint_parser_is_exposed()
    {
        NetworkEndpoint parsed = await _client.GetFromJsonAsync<NetworkEndpoint>("/api/endpoint?uri=wss://chat.example.com");

        Assert.Equal(new NetworkEndpoint("wss", "chat.example.com", 443), parsed);
    }
}
