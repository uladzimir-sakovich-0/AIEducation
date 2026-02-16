using FinanceTracker.DB.Entities;
using FinanceTracker.Infrastructure.Models.Requests;
using FinanceTracker.Infrastructure.Repositories;
using FinanceTracker.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace FinanceTracker.UnitTests.Services;

/// <summary>
/// Tests for CategoryService with user ownership validation
/// </summary>
public class CategoryServiceTests
{
    private readonly Guid _testUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private readonly Guid _otherUserId = Guid.Parse("22222222-2222-2222-2222-222222222222");

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
            .Setup(r => r.CreateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category c, CancellationToken ct) => c);

        var request = new CategoryCreateRequest { Name = "Food" };

        // Act
        await service.CreateCategoryAsync(request, _testUserId, CancellationToken.None);

        // Assert
        mockRepository.Verify(
            r => r.CreateAsync(
                It.Is<Category>(c => c.Name == "Food" && c.UserId == _testUserId && c.Id != Guid.Empty),
                It.IsAny<CancellationToken>()),
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
            .Setup(r => r.CreateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category c, CancellationToken ct) => 
            {
                c.Id = expectedId;
                return c;
            });

        var request = new CategoryCreateRequest { Name = "Transportation" };

        // Act
        var result = await service.CreateCategoryAsync(request, _testUserId, CancellationToken.None);

        // Assert
        Assert.Equal(expectedId, result);
    }

    [Fact]
    public async Task WhenUpdatingCategory_WithValidOwnership_ThenRepositoryIsCalledAndReturnsTrue()
    {
        // Arrange
        var mockRepository = GetMockRepository();
        var mockLogger = GetMockLogger();
        var service = new CategoryService(mockRepository.Object, mockLogger.Object);
        
        var categoryId = Guid.NewGuid();
        var updatedCategory = new Category { Id = categoryId, Name = "Updated Food", UserId = _testUserId };
        
        mockRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Category>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedCategory);

        var request = new CategoryUpdateRequest { Id = categoryId, Name = "Updated Food" };

        // Act
        var result = await service.UpdateCategoryAsync(request, _testUserId, CancellationToken.None);

        // Assert
        Assert.True(result);
        mockRepository.Verify(
            r => r.UpdateAsync(
                It.Is<Category>(c => c.Id == categoryId && c.Name == "Updated Food" && c.UserId == _testUserId),
                _testUserId,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task WhenUpdatingCategory_WithInvalidOwnership_ThenReturnsFalse()
    {
        // Arrange
        var mockRepository = GetMockRepository();
        var mockLogger = GetMockLogger();
        var service = new CategoryService(mockRepository.Object, mockLogger.Object);
        
        var categoryId = Guid.NewGuid();
        
        mockRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Category>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        var request = new CategoryUpdateRequest { Id = categoryId, Name = "Hacked Food" };

        // Act
        var result = await service.UpdateCategoryAsync(request, _otherUserId, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task WhenGettingAllCategories_ThenRepositoryIsCalledWithUserId()
    {
        // Arrange
        var mockRepository = GetMockRepository();
        var mockLogger = GetMockLogger();
        var service = new CategoryService(mockRepository.Object, mockLogger.Object);
        
        var categories = new List<Category>
        {
            new Category { Id = Guid.NewGuid(), Name = "Food", UserId = _testUserId },
            new Category { Id = Guid.NewGuid(), Name = "Housing", UserId = _testUserId }
        };
        
        mockRepository
            .Setup(r => r.GetAllAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);

        // Act
        await service.GetAllCategoriesAsync(_testUserId, CancellationToken.None);

        // Assert
        mockRepository.Verify(r => r.GetAllAsync(_testUserId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WhenGettingAllCategories_ThenReturnsCategoryDtos()
    {
        // Arrange
        var mockRepository = GetMockRepository();
        var mockLogger = GetMockLogger();
        var service = new CategoryService(mockRepository.Object, mockLogger.Object);
        
        var categoryId1 = Guid.NewGuid();
        var categoryId2 = Guid.NewGuid();
        var categories = new List<Category>
        {
            new Category { Id = categoryId1, Name = "Food", UserId = _testUserId },
            new Category { Id = categoryId2, Name = "Housing", UserId = _testUserId }
        };
        
        mockRepository
            .Setup(r => r.GetAllAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);

        // Act
        var result = await service.GetAllCategoriesAsync(_testUserId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, dto => dto.Id == categoryId1 && dto.Name == "Food");
        Assert.Contains(result, dto => dto.Id == categoryId2 && dto.Name == "Housing");
    }

    [Fact]
    public async Task WhenDeletingCategory_WithValidOwnership_ThenReturnsTrue()
    {
        // Arrange
        var mockRepository = GetMockRepository();
        var mockLogger = GetMockLogger();
        var service = new CategoryService(mockRepository.Object, mockLogger.Object);
        
        var categoryId = Guid.NewGuid();
        mockRepository
            .Setup(r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await service.DeleteCategoryAsync(categoryId, _testUserId, CancellationToken.None);

        // Assert
        Assert.True(result);
        mockRepository.Verify(r => r.DeleteAsync(categoryId, _testUserId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WhenDeletingCategory_WithInvalidOwnership_ThenReturnsFalse()
    {
        // Arrange
        var mockRepository = GetMockRepository();
        var mockLogger = GetMockLogger();
        var service = new CategoryService(mockRepository.Object, mockLogger.Object);
        
        var categoryId = Guid.NewGuid();
        mockRepository
            .Setup(r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await service.DeleteCategoryAsync(categoryId, _otherUserId, CancellationToken.None);

        // Assert
        Assert.False(result);
    }
}
