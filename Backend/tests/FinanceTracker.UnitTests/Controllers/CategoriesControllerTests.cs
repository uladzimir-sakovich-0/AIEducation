using FinanceTracker.API.Controllers;
using FinanceTracker.Infrastructure.Models.Requests;
using FinanceTracker.Infrastructure.Models.Responses;
using FinanceTracker.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;

namespace FinanceTracker.UnitTests.Controllers;

/// <summary>
/// Tests for CategoriesController with user authentication
/// </summary>
public class CategoriesControllerTests
{
    private readonly Guid _testUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    private Mock<ICategoryService> GetMockCategoryService()
    {
        return new Mock<ICategoryService>();
    }

    private Mock<ILogger<CategoriesController>> GetMockLogger()
    {
        return new Mock<ILogger<CategoriesController>>();
    }

    private CategoriesController CreateControllerWithUser(Mock<ICategoryService> mockService, Mock<ILogger<CategoriesController>> mockLogger)
    {
        var controller = new CategoriesController(mockService.Object, mockLogger.Object);
        
        // Setup HttpContext with user claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, _testUserId.ToString()),
            new Claim(ClaimTypes.Email, "test@example.com")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
        
        return controller;
    }

    [Fact]
    public async Task WhenCreatingCategory_ThenReturnsCreatedResult()
    {
        // Arrange
        var mockService = GetMockCategoryService();
        var mockLogger = GetMockLogger();
        var controller = CreateControllerWithUser(mockService, mockLogger);
        
        var categoryId = Guid.NewGuid();
        mockService
            .Setup(s => s.CreateCategoryAsync(It.IsAny<CategoryCreateRequest>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(categoryId);
        
        var request = new CategoryCreateRequest { Name = "Food" };

        // Act
        var result = await controller.CreateCategory(request, CancellationToken.None);

        // Assert
        Assert.IsType<CreatedAtActionResult>(result);
    }

    [Fact]
    public async Task WhenCreatingCategory_ThenServiceIsCalledWithCorrectUserId()
    {
        // Arrange
        var mockService = GetMockCategoryService();
        var mockLogger = GetMockLogger();
        var controller = CreateControllerWithUser(mockService, mockLogger);
        
        var categoryId = Guid.NewGuid();
        mockService
            .Setup(s => s.CreateCategoryAsync(It.IsAny<CategoryCreateRequest>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(categoryId);
        
        var request = new CategoryCreateRequest { Name = "Housing" };

        // Act
        await controller.CreateCategory(request, CancellationToken.None);

        // Assert
        mockService.Verify(
            s => s.CreateCategoryAsync(
                It.Is<CategoryCreateRequest>(r => r.Name == "Housing"),
                _testUserId,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task WhenUpdatingCategory_WithValidOwnership_ThenReturnsOkResult()
    {
        // Arrange
        var mockService = GetMockCategoryService();
        var mockLogger = GetMockLogger();
        var controller = CreateControllerWithUser(mockService, mockLogger);
        
        var categoryId = Guid.NewGuid();
        mockService
            .Setup(s => s.UpdateCategoryAsync(It.IsAny<CategoryUpdateRequest>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        var request = new CategoryUpdateRequest { Id = categoryId, Name = "Updated Food" };

        // Act
        var result = await controller.UpdateCategory(request, CancellationToken.None);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task WhenUpdatingCategory_WithInvalidOwnership_ThenReturnsBadRequest()
    {
        // Arrange
        var mockService = GetMockCategoryService();
        var mockLogger = GetMockLogger();
        var controller = CreateControllerWithUser(mockService, mockLogger);
        
        var categoryId = Guid.NewGuid();
        mockService
            .Setup(s => s.UpdateCategoryAsync(It.IsAny<CategoryUpdateRequest>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        var request = new CategoryUpdateRequest { Id = categoryId, Name = "Hacked Food" };

        // Act
        var result = await controller.UpdateCategory(request, CancellationToken.None);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task WhenUpdatingCategory_WithInvalidOwnership_ThenReturnsProblemDetails()
    {
        // Arrange
        var mockService = GetMockCategoryService();
        var mockLogger = GetMockLogger();
        var controller = CreateControllerWithUser(mockService, mockLogger);
        
        var categoryId = Guid.NewGuid();
        mockService
            .Setup(s => s.UpdateCategoryAsync(It.IsAny<CategoryUpdateRequest>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        var request = new CategoryUpdateRequest { Id = categoryId, Name = "Hacked Food" };

        // Act
        var result = await controller.UpdateCategory(request, CancellationToken.None) as BadRequestObjectResult;

        // Assert
        Assert.NotNull(result);
        var problemDetails = result.Value as ProblemDetails;
        Assert.NotNull(problemDetails);
        Assert.Contains("does not exist or you do not have permission", problemDetails.Detail);
    }

    [Fact]
    public async Task WhenUpdatingCategory_ThenServiceIsCalledWithCorrectUserId()
    {
        // Arrange
        var mockService = GetMockCategoryService();
        var mockLogger = GetMockLogger();
        var controller = CreateControllerWithUser(mockService, mockLogger);
        
        var categoryId = Guid.NewGuid();
        mockService
            .Setup(s => s.UpdateCategoryAsync(It.IsAny<CategoryUpdateRequest>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        var request = new CategoryUpdateRequest { Id = categoryId, Name = "Updated Transportation" };

        // Act
        await controller.UpdateCategory(request, CancellationToken.None);

        // Assert
        mockService.Verify(
            s => s.UpdateCategoryAsync(
                It.Is<CategoryUpdateRequest>(r => r.Id == categoryId && r.Name == "Updated Transportation"),
                _testUserId,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task WhenGettingAllCategories_ThenReturnsOkResult()
    {
        // Arrange
        var mockService = GetMockCategoryService();
        var mockLogger = GetMockLogger();
        var controller = CreateControllerWithUser(mockService, mockLogger);
        
        mockService
            .Setup(s => s.GetAllCategoriesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CategoryDto>());

        // Act
        var result = await controller.GetAllCategories(CancellationToken.None);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task WhenGettingAllCategories_ThenServiceIsCalledWithCorrectUserId()
    {
        // Arrange
        var mockService = GetMockCategoryService();
        var mockLogger = GetMockLogger();
        var controller = CreateControllerWithUser(mockService, mockLogger);
        
        mockService
            .Setup(s => s.GetAllCategoriesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CategoryDto>());

        // Act
        await controller.GetAllCategories(CancellationToken.None);

        // Assert
        mockService.Verify(s => s.GetAllCategoriesAsync(_testUserId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WhenGettingAllCategories_ThenReturnsCategoryDtos()
    {
        // Arrange
        var mockService = GetMockCategoryService();
        var mockLogger = GetMockLogger();
        var controller = CreateControllerWithUser(mockService, mockLogger);
        
        var categories = new List<CategoryDto>
        {
            new CategoryDto { Id = Guid.NewGuid(), Name = "Food" },
            new CategoryDto { Id = Guid.NewGuid(), Name = "Housing" }
        };
        
        mockService
            .Setup(s => s.GetAllCategoriesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);

        // Act
        var result = await controller.GetAllCategories(CancellationToken.None) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var returnedCategories = result.Value as List<CategoryDto>;
        Assert.NotNull(returnedCategories);
        Assert.Equal(2, returnedCategories.Count);
    }

    [Fact]
    public async Task WhenDeletingCategory_WithValidOwnership_ThenReturnsOkResult()
    {
        // Arrange
        var mockService = GetMockCategoryService();
        var mockLogger = GetMockLogger();
        var controller = CreateControllerWithUser(mockService, mockLogger);
        
        var categoryId = Guid.NewGuid();
        mockService
            .Setup(s => s.DeleteCategoryAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await controller.DeleteCategory(categoryId, CancellationToken.None);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task WhenDeletingCategory_WithInvalidOwnership_ThenReturnsBadRequest()
    {
        // Arrange
        var mockService = GetMockCategoryService();
        var mockLogger = GetMockLogger();
        var controller = CreateControllerWithUser(mockService, mockLogger);
        
        var categoryId = Guid.NewGuid();
        mockService
            .Setup(s => s.DeleteCategoryAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await controller.DeleteCategory(categoryId, CancellationToken.None);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task WhenDeletingCategory_WithInvalidOwnership_ThenReturnsProblemDetails()
    {
        // Arrange
        var mockService = GetMockCategoryService();
        var mockLogger = GetMockLogger();
        var controller = CreateControllerWithUser(mockService, mockLogger);
        
        var categoryId = Guid.NewGuid();
        mockService
            .Setup(s => s.DeleteCategoryAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await controller.DeleteCategory(categoryId, CancellationToken.None) as BadRequestObjectResult;

        // Assert
        Assert.NotNull(result);
        var problemDetails = result.Value as ProblemDetails;
        Assert.NotNull(problemDetails);
        Assert.Contains("does not exist or you do not have permission", problemDetails.Detail);
    }

    [Fact]
    public async Task WhenDeletingCategory_ThenServiceIsCalledWithCorrectUserId()
    {
        // Arrange
        var mockService = GetMockCategoryService();
        var mockLogger = GetMockLogger();
        var controller = CreateControllerWithUser(mockService, mockLogger);
        
        var categoryId = Guid.NewGuid();
        mockService
            .Setup(s => s.DeleteCategoryAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await controller.DeleteCategory(categoryId, CancellationToken.None);

        // Assert
        mockService.Verify(s => s.DeleteCategoryAsync(categoryId, _testUserId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
