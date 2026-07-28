using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NutriMind.Application.Services;
using NutriMind.Application.Settings;
using NutriMind.Domain.Interfaces.Repositories;
using NutriMind.Tests.TestHelpers;
using Xunit;

namespace NutriMind.Tests.Services;

public class GeminiAiServiceTests
{
    private readonly IFoodRepository _foodRepository = Substitute.For<IFoodRepository>();
    private readonly ILogger<GeminiAiService> _logger = Substitute.For<ILogger<GeminiAiService>>();

    private static string BuildGeminiEnvelope(object innerPayload)
    {
        var innerJson = JsonSerializer.Serialize(innerPayload);

        return JsonSerializer.Serialize(new
        {
            candidates = new object[]
            {
                new
                {
                    content = new
                    {
                        parts = new object[] { new { text = innerJson } }
                    }
                }
            }
        });
    }

    private GeminiAiService CreateSut(Func<HttpRequestMessage, HttpResponseMessage> responder, string apiKey = "fake-api-key")
    {
        var handler = new FakeHttpMessageHandler(responder);
        var httpClient = new HttpClient(handler);
        var settings = Options.Create(new GeminiSettings { ApiKey = apiKey });

        return new GeminiAiService(httpClient, settings, _logger, _foodRepository);
    }

    [Fact]
    public async Task EstimateFoodNutritionAsync_SuccessfulResponse_ReturnsParsedEstimate()
    {
        var envelope = BuildGeminiEnvelope(new
        {
            CaloriesPer100g = 52,
            ProteinPer100g = 0.3,
            CarbsPer100g = 14,
            FatPer100g = 0.2
        });

        var sut = CreateSut(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(envelope, Encoding.UTF8, "application/json")
        });

        var result = await sut.EstimateFoodNutritionAsync("Manzana");

        Assert.True(result.IsSuccess);
        Assert.Equal(52, result.Data.CaloriesPer100g);
        Assert.Equal(14, result.Data.CarbsPer100g);
    }

    [Fact]
    public async Task EstimateFoodNutritionAsync_TooManyRequestsResponse_ReturnsFailureInsteadOfThrowing()
    {
        var sut = CreateSut(_ => new HttpResponseMessage(HttpStatusCode.TooManyRequests)
        {
            Content = new StringContent("{\"error\":\"rate limited\"}", Encoding.UTF8, "application/json")
        });

        var result = await sut.EstimateFoodNutritionAsync("Manzana");

        Assert.False(result.IsSuccess);
        Assert.Contains("Error de Google", result.ErrorMessage);
    }

    [Fact]
    public async Task EstimateFoodNutritionAsync_HttpClientThrowsTimeout_ReturnsFailureInsteadOfThrowing()
    {
        var sut = CreateSut(_ => throw new TaskCanceledException("Simulated Gemini timeout"));

        var result = await sut.EstimateFoodNutritionAsync("Manzana");

        Assert.False(result.IsSuccess);
        Assert.False(string.IsNullOrEmpty(result.ErrorMessage));
    }

    [Fact]
    public async Task EstimateFoodNutritionAsync_MissingApiKey_ReturnsFailureWithoutCallingHttp()
    {
        var sut = CreateSut(
            _ => throw new InvalidOperationException("No debería llamarse a HTTP sin API key."),
            apiKey: string.Empty);

        var result = await sut.EstimateFoodNutritionAsync("Manzana");

        Assert.False(result.IsSuccess);
        Assert.Contains("API Key", result.ErrorMessage);
    }
}
