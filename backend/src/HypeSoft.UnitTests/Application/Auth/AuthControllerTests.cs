using FluentAssertions;
using HypeSoft.API.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Security.Claims;
using System.Text.Json;

namespace HypeSoft.UnitTests.Application.Auth;

public class AuthControllerTests
{
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly Mock<ILogger<AuthController>> _loggerMock;
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;

    public AuthControllerTests()
    {
        _configurationMock = new Mock<IConfiguration>();
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _loggerMock = new Mock<ILogger<AuthController>>();
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();

        SetupConfiguration();
    }

    private void SetupConfiguration()
    {
        _configurationMock.Setup(x => x["Keycloak:Authority"])
            .Returns("http://keycloak:8080/realms/hypesoft");
        _configurationMock.Setup(x => x["Keycloak:ClientId"])
            .Returns("hypesoft-api");
        _configurationMock.Setup(x => x["Keycloak:ClientSecret"])
            .Returns("hypesoft-api-secret");
    }

    private AuthController CreateController(HttpResponseMessage? responseMessage = null)
    {
        var response = responseMessage ?? new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(new
            {
                access_token = "test-access-token",
                refresh_token = "test-refresh-token",
                expires_in = 3600,
                token_type = "Bearer"
            }))
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(httpClient);

        return new AuthController(
            _configurationMock.Object,
            _httpClientFactoryMock.Object,
            _loggerMock.Object);
    }

    #region Login Tests

    [Fact]
    public async Task Login_ValidCredentials_ShouldReturnTokenResponse()
    {
        // Arrange
        var controller = CreateController();
        var request = new LoginRequest("admin", "admin123");

        // Act
        var result = await controller.Login(request);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var tokenResponse = okResult.Value.Should().BeOfType<TokenResponse>().Subject;
        
        tokenResponse.AccessToken.Should().Be("test-access-token");
        tokenResponse.RefreshToken.Should().Be("test-refresh-token");
        tokenResponse.ExpiresIn.Should().Be(3600);
        tokenResponse.TokenType.Should().Be("Bearer");
    }

    [Fact]
    public async Task Login_InvalidCredentials_ShouldReturnUnauthorized()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
        var controller = CreateController(response);
        var request = new LoginRequest("invalid", "invalid");

        // Act
        var result = await controller.Login(request);

        // Assert
        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task Login_EmptyUsername_ShouldStillAttemptLogin()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
        var controller = CreateController(response);
        var request = new LoginRequest("", "password");

        // Act
        var result = await controller.Login(request);

        // Assert
        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task Login_EmptyPassword_ShouldStillAttemptLogin()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
        var controller = CreateController(response);
        var request = new LoginRequest("username", "");

        // Act
        var result = await controller.Login(request);

        // Assert
        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task Login_KeycloakUnavailable_ShouldReturnServerError()
    {
        // Arrange
        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Connection refused"));

        var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(httpClient);

        var controller = new AuthController(
            _configurationMock.Object,
            _httpClientFactoryMock.Object,
            _loggerMock.Object);

        var request = new LoginRequest("admin", "admin123");

        // Act
        var result = await controller.Login(request);

        // Assert
        var statusResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusResult.StatusCode.Should().Be(500);
    }

    #endregion

    #region RefreshToken Tests

    [Fact]
    public async Task RefreshToken_ValidToken_ShouldReturnNewTokenResponse()
    {
        // Arrange
        var controller = CreateController();
        var request = new RefreshTokenRequest("valid-refresh-token");

        // Act
        var result = await controller.RefreshToken(request);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var tokenResponse = okResult.Value.Should().BeOfType<TokenResponse>().Subject;
        
        tokenResponse.AccessToken.Should().NotBeNullOrEmpty();
        tokenResponse.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task RefreshToken_InvalidToken_ShouldReturnUnauthorized()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
        var controller = CreateController(response);
        var request = new RefreshTokenRequest("invalid-refresh-token");

        // Act
        var result = await controller.RefreshToken(request);

        // Assert
        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task RefreshToken_ExpiredToken_ShouldReturnUnauthorized()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
        var controller = CreateController(response);
        var request = new RefreshTokenRequest("expired-refresh-token");

        // Act
        var result = await controller.RefreshToken(request);

        // Assert
        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    #endregion

    #region GetCurrentUser Tests

    [Fact]
    public void GetCurrentUser_AuthenticatedUser_ShouldReturnUserInfo()
    {
        // Arrange
        var controller = CreateController();
        
        var claims = new List<Claim>
        {
            new Claim("sub", "user-123"),
            new Claim("preferred_username", "testuser"),
            new Claim("email", "test@example.com"),
            new Claim("given_name", "Test"),
            new Claim("family_name", "User")
        };
        
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        // Act
        var result = controller.GetCurrentUser();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var userInfo = okResult.Value.Should().BeOfType<UserInfo>().Subject;
        
        userInfo.Id.Should().Be("user-123");
        userInfo.Username.Should().Be("testuser");
        userInfo.Email.Should().Be("test@example.com");
        userInfo.FirstName.Should().Be("Test");
        userInfo.LastName.Should().Be("User");
    }

    [Fact]
    public void GetCurrentUser_UserWithMinimalClaims_ShouldReturnPartialUserInfo()
    {
        // Arrange
        var controller = CreateController();
        
        var claims = new List<Claim>
        {
            new Claim("sub", "user-456")
        };
        
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        // Act
        var result = controller.GetCurrentUser();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var userInfo = okResult.Value.Should().BeOfType<UserInfo>().Subject;
        
        userInfo.Id.Should().Be("user-456");
        userInfo.Username.Should().BeEmpty();
        userInfo.Email.Should().BeEmpty();
    }

    #endregion

    #region Register Tests

    [Fact]
    public async Task Register_ValidRequest_ShouldReturnCreated()
    {
        // Arrange
        var adminTokenResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(new
            {
                access_token = "admin-token"
            }))
        };

        var createUserResponse = new HttpResponseMessage(HttpStatusCode.Created);

        var callCount = 0;
        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() =>
            {
                callCount++;
                return callCount == 1 ? adminTokenResponse : createUserResponse;
            });

        var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(httpClient);

        var controller = new AuthController(
            _configurationMock.Object,
            _httpClientFactoryMock.Object,
            _loggerMock.Object);

        var request = new RegisterRequest(
            "newuser",
            "newuser@example.com",
            "password123",
            "New",
            "User");

        // Act
        var result = await controller.Register(request);

        // Assert
        var createdResult = result.Should().BeOfType<CreatedResult>().Subject;
        createdResult.StatusCode.Should().Be(201);
    }

    [Fact]
    public async Task Register_DuplicateUser_ShouldReturnBadRequest()
    {
        // Arrange
        var adminTokenResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(new
            {
                access_token = "admin-token"
            }))
        };

        var conflictResponse = new HttpResponseMessage(HttpStatusCode.Conflict);

        var callCount = 0;
        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() =>
            {
                callCount++;
                return callCount == 1 ? adminTokenResponse : conflictResponse;
            });

        var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(httpClient);

        var controller = new AuthController(
            _configurationMock.Object,
            _httpClientFactoryMock.Object,
            _loggerMock.Object);

        var request = new RegisterRequest(
            "existinguser",
            "existing@example.com",
            "password123");

        // Act
        var result = await controller.Register(request);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Register_AdminTokenUnavailable_ShouldReturnServerError()
    {
        // Arrange
        var unauthorizedResponse = new HttpResponseMessage(HttpStatusCode.Unauthorized);

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(unauthorizedResponse);

        var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(httpClient);

        var controller = new AuthController(
            _configurationMock.Object,
            _httpClientFactoryMock.Object,
            _loggerMock.Object);

        var request = new RegisterRequest(
            "newuser",
            "newuser@example.com",
            "password123");

        // Act
        var result = await controller.Register(request);

        // Assert
        var statusResult = result.Should().BeOfType<ObjectResult>().Subject;
        statusResult.StatusCode.Should().Be(500);
    }

    #endregion
}
