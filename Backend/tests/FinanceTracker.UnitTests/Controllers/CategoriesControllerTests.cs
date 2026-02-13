using FinanceTracker.API.Controllers;
using FinanceTracker.API.Models.Requests;
using FinanceTracker.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace FinanceTracker.UnitTests.Controllers;

/// <summary>
/// Tests for CategoriesController
/// </summary>
public class CategoriesControllerTests
{
    private Mock<ICategoryService> GetMockCategoryService()
    {
        return new Mock<ICategoryService>();
    }

    private Mock<ILogger<CategoriesController>> GetMockLogger()
    {
        return new Mock<ILogger<CategoriesController>>();
    }

    [Fact]
    public async Task WhenCreatingCategory_ThenReturnsCreatedResult()
    {
        // Arrange
        var mockService = GetMockCategoryService();
        var mockLogger = GetMockLogger();
        var controller = new CategoriesController(mockService.Object, mockLogger.Object);
        
        var categoryId = Guid.NewGuid();
        mockService
            .Setup(s => s.CreateCategoryAsync(It.IsAny<string>(), default))
            .ReturnsAsync(categoryId);
        
        var request = new CategoryCreateRequest { Name = "Food" };

        // Act
        var result = await controller.CreateCategory(request, default);

        // Assert
        Assert.IsType<CreatedAtActionResult>(result);
    }

    [Fact]
    public async Task WhenCreatingCategory_ThenReturnsCreatedCategoryId()
    {
        // Arrange
        var mockService = GetMockCategoryService();
        var mockLogger = GetMockLogger();
        var controller = new CategoriesController(mockService.Object, mockLogger.Object);
        
        var categoryId = Guid.NewGuid();
        mockService
            .Setup(s => s.CreateCategoryAsync(It.IsAny<string>(), default))
            .ReturnsAsync(categoryId);
        
        var request = new CategoryCreateRequest { Name = "Transportation" };

        // Act
        var result = await controller.CreateCategory(request, default) as CreatedAtActionResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(categoryId, result.Value);
    }

    [Fact]
    public async Task WhenCreatingCategory_ThenServiceIsCalledWithCorrectName()
    {
        // Arrange
        var mockService = GetMockCategoryService();
        var mockLogger = GetMockLogger();
        var controller = new CategoriesController(mockService.Object, mockLogger.Object);
        
        var categoryId = Guid.NewGuid();
        mockService
            .Setup(s => s.CreateCategoryAsync("Housing", default))
            .ReturnsAsync(categoryId);
        
        var request = new CategoryCreateRequest { Name = "Housing" };

        // Act
        await controller.CreateCategory(request, default);

        // Assert
        mockService.Verify(
            s => s.CreateCategoryAsync("Housing", default),
            Times.Once);
    }

    [Fact]
    public async Task WhenCreatingCategory_ThenLogsInformation()
    {
        // Arrange
        var mockService = GetMockCategoryService();
        var mockLogger = GetMockLogger();
        var controller = new CategoriesController(mockService.Object, mockLogger.Object);
        
        var categoryId = Guid.NewGuid();
        mockService
            .Setup(s => s.CreateCategoryAsync(It.IsAny<string>(), default))
            .ReturnsAsync(categoryId);
        
        var request = new CategoryCreateRequest { Name = "Entertainment" };

        // Act
        await controller.CreateCategory(request, default);

        // Assert
        mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Received request to create category")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task WhenCreatingCategory_ThenReturnsCreatedAtActionWithCorrectActionName()
    {
        // Arrange
        var mockService = GetMockCategoryService();
        var mockLogger = GetMockLogger();
        var controller = new CategoriesController(mockService.Object, mockLogger.Object);
        
        var categoryId = Guid.NewGuid();
        mockService
            .Setup(s => s.CreateCategoryAsync(It.IsAny<string>(), default))
            .ReturnsAsync(categoryId);
        
        var request = new CategoryCreateRequest { Name = "Utilities" };

        // Act
        var result = await controller.CreateCategory(request, default) as CreatedAtActionResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(nameof(CategoriesController.CreateCategory), result.ActionName);
    }
}
