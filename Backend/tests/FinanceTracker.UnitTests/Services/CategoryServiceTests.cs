using FinanceTracker.DB.Entities;
using FinanceTracker.Infrastructure.Repositories;
using FinanceTracker.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace FinanceTracker.UnitTests.Services;

/// <summary>
/// Tests for CategoryService
/// </summary>
public class CategoryServiceTests
{
    private Mock<ICategoryRepository> GetMockRepository()
    {
        return new Mock<ICategoryRepository>();
    }

    private Mock<ILogger<CategoryService>> GetMockLogger()
    {
        return new Mock<ILogger<CategoryService>>();
    }

    [Fact]
    public async Task WhenCreatingCategory_ThenRepositoryIsCalledWithCorrectData()
    {
        // Arrange
        var mockRepository = GetMockRepository();
        var mockLogger = GetMockLogger();
        var service = new CategoryService(mockRepository.Object, mockLogger.Object);
        
        mockRepository
            .Setup(r => r.CreateAsync(It.IsAny<Category>(), default))
            .ReturnsAsync((Category c, CancellationToken ct) => c);

        // Act
        await service.CreateCategoryAsync("Food", default);

        // Assert
        mockRepository.Verify(
            r => r.CreateAsync(
                It.Is<Category>(c => c.Name == "Food" && c.Id != Guid.Empty),
                default),
            Times.Once);
    }

    [Fact]
    public async Task WhenCreatingCategory_ThenReturnsGeneratedId()
    {
        // Arrange
        var mockRepository = GetMockRepository();
        var mockLogger = GetMockLogger();
        var service = new CategoryService(mockRepository.Object, mockLogger.Object);
        
        var expectedId = Guid.NewGuid();
        mockRepository
            .Setup(r => r.CreateAsync(It.IsAny<Category>(), default))
            .ReturnsAsync((Category c, CancellationToken ct) => 
            {
                c.Id = expectedId;
                return c;
            });

        // Act
        var result = await service.CreateCategoryAsync("Transportation", default);

        // Assert
        Assert.Equal(expectedId, result);
    }

    [Fact]
    public async Task WhenCreatingCategory_ThenGeneratesNewGuid()
    {
        // Arrange
        var mockRepository = GetMockRepository();
        var mockLogger = GetMockLogger();
        var service = new CategoryService(mockRepository.Object, mockLogger.Object);
        
        mockRepository
            .Setup(r => r.CreateAsync(It.IsAny<Category>(), default))
            .ReturnsAsync((Category c, CancellationToken ct) => c);

        // Act
        var result = await service.CreateCategoryAsync("Housing", default);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
    }

    [Fact]
    public async Task WhenCreatingCategory_ThenLogsInformation()
    {
        // Arrange
        var mockRepository = GetMockRepository();
        var mockLogger = GetMockLogger();
        var service = new CategoryService(mockRepository.Object, mockLogger.Object);
        
        mockRepository
            .Setup(r => r.CreateAsync(It.IsAny<Category>(), default))
            .ReturnsAsync((Category c, CancellationToken ct) => c);

        // Act
        await service.CreateCategoryAsync("Entertainment", default);

        // Assert
        mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Creating category")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task WhenCreatingMultipleCategories_ThenEachGetsUniqueId()
    {
        // Arrange
        var mockRepository = GetMockRepository();
        var mockLogger = GetMockLogger();
        var service = new CategoryService(mockRepository.Object, mockLogger.Object);
        
        mockRepository
            .Setup(r => r.CreateAsync(It.IsAny<Category>(), default))
            .ReturnsAsync((Category c, CancellationToken ct) => c);

        // Act
        var result1 = await service.CreateCategoryAsync("Food", default);
        var result2 = await service.CreateCategoryAsync("Housing", default);

        // Assert
        Assert.NotEqual(result1, result2);
    }
}
