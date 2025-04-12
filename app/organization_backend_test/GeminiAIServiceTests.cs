using Moq.Protected;
using Moq;
using organization_back_end.AIHelpers;
using organization_back_end.Services;
using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace organization_back_end.Tests.Services
{
    public class GeminiAIServiceTests
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "test-api-key";
        private readonly string _apiUrl = "https://api.gemini.ai/test";

        public GeminiAIServiceTests()
        {
            // Setup configuration
            _configurationMock = new Mock<IConfiguration>();
            _configurationMock.SetupGet(c => c["GeminiAI:ApiKey"]).Returns(_apiKey);
            _configurationMock.SetupGet(c => c["GeminiAI:ApiUrl"]).Returns(_apiUrl);

            // Setup HTTP handler
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        }

        [Fact]
        public async Task GetResponseAsync_SuccessfulApiCall_ReturnsGeminiResponse()
        {
            // Arrange
            var promptText = "Test prompt";
            var expectedResponse = new GeminiResponse
            {
                Success = true,
                Data = "Test response data"
            };

            var apiResponse = new GeminiApiResponse
            {
                Candidates = new ApiCandidate[]
                {
                    new ApiCandidate
                    {
                        Content = new Content
                        {
                            Parts = new[]
                            {
                                new Part
                                {
                                    Text = JsonSerializer.Serialize(expectedResponse)
                                }
                            }
                        }
                    }
                }
            };

            var responseJson = JsonSerializer.Serialize(apiResponse);

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
                });

            var geminiService = new GeminiAIService(_configurationMock.Object, _httpClient);

            // Act
            var result = await geminiService.GetResponseAsync(promptText);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
        }

        [Fact]
        public async Task GetResponseAsync_NonJsonResponse_ReturnsRawTextInData()
        {
            // Arrange
            var promptText = "Test prompt";
            var rawResponseText = "This is not JSON but plain text";

            var apiResponse = new GeminiApiResponse
            {
                Candidates = new ApiCandidate[]
                {
                    new ApiCandidate
                    {
                        Content = new Content
                        {
                            Parts = new[]
                            {
                                new Part
                                {
                                    Text = rawResponseText
                                }
                            }
                        }
                    }
                }
            };

            var responseJson = JsonSerializer.Serialize(apiResponse);

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
                });

            var geminiService = new GeminiAIService(_configurationMock.Object, _httpClient);

            // Act
            var result = await geminiService.GetResponseAsync(promptText);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(rawResponseText, result.Data);
        }

        [Fact]
        public async Task GetResponseAsync_ApiError_ThrowsException()
        {
            // Arrange
            var promptText = "Test prompt";
            var errorMessage = "API Error Message";

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent(errorMessage, Encoding.UTF8, "application/json")
                });

            var geminiService = new GeminiAIService(_configurationMock.Object, _httpClient);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => geminiService.GetResponseAsync(promptText));
            Assert.Contains("Gemini API Error", exception.Message);
            Assert.Contains(errorMessage, exception.Message);
        }

        [Fact]
        public async Task GetResponseAsync_EmptyResponse_ReturnsErrorResponse()
        {
            // Arrange
            var promptText = "Test prompt";

            var apiResponse = new GeminiApiResponse
            {
                Candidates = new ApiCandidate[]
                {
                    new ApiCandidate
                    {
                        Content = new Content
                        {
                            Parts = new[]
                            {
                                new Part
                                {
                                    Text = ""
                                }
                            }
                        }
                    }
                }
            };

            var responseJson = JsonSerializer.Serialize(apiResponse);

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
                });

            var geminiService = new GeminiAIService(_configurationMock.Object, _httpClient);

            // Act
            var result = await geminiService.GetResponseAsync(promptText);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Contains("Empty response", result.ErrorMessage);
        }

        [Fact]
        public async Task GetResponseAsync_NullResponse_ReturnsErrorResponse()
        {
            // Arrange
            var promptText = "Test prompt";

            var apiResponse = new GeminiApiResponse
            {
                Candidates = new[]
                {
                    new ApiCandidate
                    {
                        Content = new Content
                        {
                            Parts = null
                        }
                    }
                }
            };

            var responseJson = JsonSerializer.Serialize(apiResponse);

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
                });

            var geminiService = new GeminiAIService(_configurationMock.Object, _httpClient);

            // Act
            var result = await geminiService.GetResponseAsync(promptText);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
        }
    }
}
