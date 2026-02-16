using FinanceTracker.API.Controllers;
using FinanceTracker.Infrastructure.Models.Requests;
using FinanceTracker.Infrastructure.Models.Responses;
using FinanceTracker.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
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
            .Setup(s => s.CreateCategoryAsync(It.IsAny<CategoryCreateRequest>(), default))
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
            .Setup(s => s.CreateCategoryAsync(It.IsAny<CategoryCreateRequest>(), default))
            .ReturnsAsync(categoryId);
        
        var request = new CategoryCreateRequest { Name = "Transportation" };

        // Act
        var result = await controller.CreateCategory(request, default) as CreatedAtActionResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(categoryId, result.Value);
    }

    [Fact]
    public async Task WhenCreatingCategory_ThenServiceIsCalledWithCorrectRequest()
    {
        // Arrange
        var mockService = GetMockCategoryService();
        var mockLogger = GetMockLogger();
        var controller = new CategoriesController(mockService.Object, mockLogger.Object);
        
        var categoryId = Guid.NewGuid();
        mockService
            .Setup(s => s.CreateCategoryAsync(It.IsAny<CategoryCreateRequest>(), default))
            .ReturnsAsync(categoryId);
        
        var request = new CategoryCreateRequest { Name = "Housing" };

        // Act
        await controller.CreateCategory(request, default);

        // Assert
        mockService.Verify(
            s => s.CreateCategoryAsync(
                It.Is<CategoryCreateRequest>(r => r.Name == "Housing"), 
                default),
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
            .Setup(s => s.CreateCategoryAsync(It.IsAny<CategoryCreateRequest>(), default))
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
            .Setup(s => s.CreateCategoryAsync(It.IsAny<CategoryCreateRequest>(), default))
            .ReturnsAsync(categoryId);
        
        var request = new CategoryCreateRequest { Name = "Utilities" };

        // Act
        var result = await controller.CreateCategory(request, default) as CreatedAtActionResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(nameof(CategoriesController.CreateCategory), result.ActionName);
    }

    [Fact]
    public async Task WhenUpdatingCategory_ThenReturnsOkResult()
    {
        // Arrange
        var mockService = GetMockCategoryService();
        var mockLogger = GetMockLogger();
        var controller = new CategoriesController(mockService.Object, mockLogger.Object);
        
        var categoryId = Guid.NewGuid();
        mockService
            .Setup(s => s.UpdateCategoryAsync(It.IsAny<CategoryUpdateRequest>(), default))
            .Returns(Task.CompletedTask);
        
        var request = new CategoryUpdateRequest { Id = categoryId, Name = "Updated Food" };

        // Act
        var result = await controller.UpdateCategory(request, default);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task WhenUpdatingCategory_ThenServiceIsCalledWithCorrectRequest()
    {
        // Arrange
        var mockService = GetMockCategoryService();
        var mockLogger = GetMockLogger();
        var controller = new CategoriesController(mockService.Object, mockLogger.Object);
        
        var categoryId = Guid.NewGuid();
        mockService
            .Setup(s => s.UpdateCategoryAsync(It.IsAny<CategoryUpdateRequest>(), default))
            .Returns(Task.CompletedTask);
        
        var request = new CategoryUpdateRequest { Id = categoryId, Name = "Updated Transportation" };

        // Act
        await controller.UpdateCategory(request, default);

        // Assert
        mockService.Verify(
            s => s.UpdateCategoryAsync(
                It.Is<CategoryUpdateRequest>(r => r.Id == categoryId && r.Name == "Updated Transportation"), 
                default),
            Times.Once);
    }

    [Fact]
    public async Task WhenUpdatingCategory_ThenLogsInformation()
    {
        // Arrange
        var mockService = GetMockCategoryService();
        var mockLogger = GetMockLogger();
        var controller = new CategoriesController(mockService.Object, mockLogger.Object);
        
        var categoryId = Guid.NewGuid();
        mockService
            .Setup(s => s.UpdateCategoryAsync(It.IsAny<CategoryUpdateRequest>(), default))
            .Returns(Task.CompletedTask);
        
        var request = new CategoryUpdateRequest { Id = categoryId, Name = "Updated Entertainment" };

        // Act
        await controller.UpdateCategory(request, default);

        // Assert
        mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Received request to update category")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task WhenGettingAllCategories_ThenReturnsOkResult()
    {
        // Arrange
        var mockService = GetMockCategoryService();
        var mockLogger = GetMockLogger();
        var controller = new CategoriesController(mockService.Object, mockLogger.Object);
        
        mockService
            .Setup(s => s.GetAllCategoriesAsync(default))
            .ReturnsAsync(new List<CategoryDto>());

        // Act
        var result = await controller.GetAllCategories(default);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task WhenGettingAllCategories_ThenReturnsCategoryDtos()
    {
        // Arrange
        var mockService = GetMockCategoryService();
        var mockLogger = GetMockLogger();
        var controller = new CategoriesController(mockService.Object, mockLogger.Object);
        
        var categories = new List<CategoryDto>
        {
            new CategoryDto { Id = Guid.NewGuid(), Name = "Food" },
            new CategoryDto { Id = Guid.NewGuid(), Name = "Housing" }
        };
        
        mockService
            .Setup(s => s.GetAllCategoriesAsync(default))
            .ReturnsAsync(categories);

        // Act
        var result = await controller.GetAllCategories(default) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var returnedCategories = result.Value as List<CategoryDto>;
        Assert.NotNull(returnedCategories);
        Assert.Equal(2, returnedCategories.Count);
    }

    [Fact]
    public async Task WhenGettingAllCategories_WithNoCategories_ThenReturnsEmptyList()
    {
        // Arrange
        var mockService = GetMockCategoryService();
        var mockLogger = GetMockLogger();
        var controller = new CategoriesController(mockService.Object, mockLogger.Object);
        
        mockService
            .Setup(s => s.GetAllCategoriesAsync(default))
            .ReturnsAsync(new List<CategoryDto>());

        // Act
        var result = await controller.GetAllCategories(default) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var returnedCategories = result.Value as List<CategoryDto>;
        Assert.NotNull(returnedCategories);
        Assert.Empty(returnedCategories);
    }

    [Fact]
    public async Task WhenGettingAllCategories_ThenServiceIsCalled()
    {
        // Arrange
        var mockService = GetMockCategoryService();
        var mockLogger = GetMockLogger();
        var controller = new CategoriesController(mockService.Object, mockLogger.Object);
        
        mockService
            .Setup(s => s.GetAllCategoriesAsync(default))
            .ReturnsAsync(new List<CategoryDto>());

        // Act
        await controller.GetAllCategories(default);

        // Assert
        mockService.Verify(s => s.GetAllCategoriesAsync(default), Times.Once);
    }

    [Fact]
    public async Task WhenGettingAllCategories_ThenLogsInformation()
    {
        // Arrange
        var mockService = GetMockCategoryService();
        var mockLogger = GetMockLogger();
        var controller = new CategoriesController(mockService.Object, mockLogger.Object);
        
        mockService
            .Setup(s => s.GetAllCategoriesAsync(default))
            .ReturnsAsync(new List<CategoryDto>());

        // Act
        await controller.GetAllCategories(default);

        // Assert
        mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Received request to get all categories")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task WhenDeletingCategory_WithValidId_ThenReturnsOkResult()
    {
        // Arrange
        var mockService = GetMockCategoryService();
        var mockLogger = GetMockLogger();
        var controller = new CategoriesController(mockService.Object, mockLogger.Object);
        
        var categoryId = Guid.NewGuid();
        mockService
            .Setup(s => s.DeleteCategoryAsync(categoryId, default))
            .ReturnsAsync(true);

        // Act
        var result = await controller.DeleteCategory(categoryId, default);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task WhenDeletingCategory_WithInvalidId_ThenReturnsBadRequest()
    {
        // Arrange
        var mockService = GetMockCategoryService();
        var mockLogger = GetMockLogger();
        var controller = new CategoriesController(mockService.Object, mockLogger.Object);
        
        var categoryId = Guid.NewGuid();
        mockService
            .Setup(s => s.DeleteCategoryAsync(categoryId, default))
            .ReturnsAsync(false);

        // Act
        var result = await controller.DeleteCategory(categoryId, default);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task WhenDeletingCategory_WithInvalidId_ThenReturnsProblemDetails()
    {
        // Arrange
        var mockService = GetMockCategoryService();
        var mockLogger = GetMockLogger();
        var controller = new CategoriesController(mockService.Object, mockLogger.Object);
        
        var categoryId = Guid.NewGuid();
        mockService
            .Setup(s => s.DeleteCategoryAsync(categoryId, default))
            .ReturnsAsync(false);

        // Act
        var result = await controller.DeleteCategory(categoryId, default) as BadRequestObjectResult;

        // Assert
        Assert.NotNull(result);
        var problemDetails = result.Value as ProblemDetails;
        Assert.NotNull(problemDetails);
        Assert.Equal("Category Not Found", problemDetails.Title);
        Assert.Equal(StatusCodes.Status400BadRequest, problemDetails.Status);
    }

    [Fact]
    public async Task WhenDeletingCategory_ThenServiceIsCalledWithCorrectId()
    {
        // Arrange
        var mockService = GetMockCategoryService();
        var mockLogger = GetMockLogger();
        var controller = new CategoriesController(mockService.Object, mockLogger.Object);
        
        var categoryId = Guid.NewGuid();
        mockService
            .Setup(s => s.DeleteCategoryAsync(categoryId, default))
            .ReturnsAsync(true);

        // Act
        await controller.DeleteCategory(categoryId, default);

        // Assert
        mockService.Verify(s => s.DeleteCategoryAsync(categoryId, default), Times.Once);
    }

    [Fact]
    public async Task WhenDeletingCategory_WithValidId_ThenLogsInformation()
    {
        // Arrange
        var mockService = GetMockCategoryService();
        var mockLogger = GetMockLogger();
        var controller = new CategoriesController(mockService.Object, mockLogger.Object);
        
        var categoryId = Guid.NewGuid();
        mockService
            .Setup(s => s.DeleteCategoryAsync(categoryId, default))
            .ReturnsAsync(true);

        // Act
        await controller.DeleteCategory(categoryId, default);

        // Assert
        mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Received request to delete category")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task WhenDeletingCategory_WithInvalidId_ThenLogsWarning()
    {
        // Arrange
        var mockService = GetMockCategoryService();
        var mockLogger = GetMockLogger();
        var controller = new CategoriesController(mockService.Object, mockLogger.Object);
        
        var categoryId = Guid.NewGuid();
        mockService
            .Setup(s => s.DeleteCategoryAsync(categoryId, default))
            .ReturnsAsync(false);

        // Act
        await controller.DeleteCategory(categoryId, default);

        // Assert
        mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("not found")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
