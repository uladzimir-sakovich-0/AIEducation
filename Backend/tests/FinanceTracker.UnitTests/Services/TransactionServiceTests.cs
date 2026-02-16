using FinanceTracker.DB.Entities;
using FinanceTracker.Infrastructure.Models.Requests;
using FinanceTracker.Infrastructure.Repositories;
using FinanceTracker.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace FinanceTracker.UnitTests.Services;

/// <summary>
/// Tests for TransactionService with user ownership validation
/// </summary>
public class TransactionServiceTests
{
    private readonly Guid _testUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private readonly Guid _otherUserId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    private Mock<ITransactionRepository> GetMockRepository()
    {
        return new Mock<ITransactionRepository>();
    }

    private Mock<ILogger<TransactionService>> GetMockLogger()
    {
        return new Mock<ILogger<TransactionService>>();
    }

    [Fact]
    public async Task WhenCreatingTransaction_WithValidOwnership_ThenRepositoryIsCalledWithCorrectData()
    {
        // Arrange
        var mockRepository = GetMockRepository();
        var mockLogger = GetMockLogger();
        var service = new TransactionService(mockRepository.Object, mockLogger.Object);
        
        var accountId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        
        mockRepository
            .Setup(r => r.ValidateAccountOwnershipAsync(accountId, _testUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        mockRepository
            .Setup(r => r.ValidateCategoryOwnershipAsync(categoryId, _testUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        mockRepository
            .Setup(r => r.CreateAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Transaction t, CancellationToken ct) => t);

        var request = new TransactionCreateRequest
        {
            AccountId = accountId,
            Amount = 100.50m,
            Timestamp = DateTime.UtcNow,
            CategoryId = categoryId,
            Notes = "Grocery shopping"
        };

        // Act
        await service.CreateTransactionAsync(request, _testUserId, CancellationToken.None);

        // Assert
        mockRepository.Verify(
            r => r.CreateAsync(
                It.Is<Transaction>(t => 
                    t.AccountId == accountId &&
                    t.Amount == 100.50m &&
                    t.CategoryId == categoryId &&
                    t.Notes == "Grocery shopping" &&
                    t.Id != Guid.Empty),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task WhenCreatingTransaction_WithValidOwnership_ThenReturnsGeneratedId()
    {
        // Arrange
        var mockRepository = GetMockRepository();
        var mockLogger = GetMockLogger();
        var service = new TransactionService(mockRepository.Object, mockLogger.Object);
        
        var accountId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var expectedId = Guid.NewGuid();
        
        mockRepository
            .Setup(r => r.ValidateAccountOwnershipAsync(accountId, _testUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        mockRepository
            .Setup(r => r.ValidateCategoryOwnershipAsync(categoryId, _testUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        mockRepository
            .Setup(r => r.CreateAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Transaction t, CancellationToken ct) =>
            {
                t.Id = expectedId;
                return t;
            });

        var request = new TransactionCreateRequest
        {
            AccountId = accountId,
            Amount = 250.00m,
            Timestamp = DateTime.UtcNow,
            CategoryId = categoryId,
            Notes = "Salary"
        };

        // Act
        var result = await service.CreateTransactionAsync(request, _testUserId, CancellationToken.None);

        // Assert
        Assert.Equal(expectedId, result);
    }

    [Fact]
    public async Task WhenCreatingTransaction_WithInvalidAccountOwnership_ThenReturnsNull()
    {
        // Arrange
        var mockRepository = GetMockRepository();
        var mockLogger = GetMockLogger();
        var service = new TransactionService(mockRepository.Object, mockLogger.Object);
        
        var accountId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        
        mockRepository
            .Setup(r => r.ValidateAccountOwnershipAsync(accountId, _testUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var request = new TransactionCreateRequest
        {
            AccountId = accountId,
            Amount = 100.00m,
            Timestamp = DateTime.UtcNow,
            CategoryId = categoryId,
            Notes = "Unauthorized"
        };

        // Act
        var result = await service.CreateTransactionAsync(request, _testUserId, CancellationToken.None);

        // Assert
        Assert.Null(result);
        mockRepository.Verify(r => r.CreateAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task WhenCreatingTransaction_WithInvalidCategoryOwnership_ThenReturnsNull()
    {
        // Arrange
        var mockRepository = GetMockRepository();
        var mockLogger = GetMockLogger();
        var service = new TransactionService(mockRepository.Object, mockLogger.Object);
        
        var accountId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        
        mockRepository
            .Setup(r => r.ValidateAccountOwnershipAsync(accountId, _testUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        mockRepository
            .Setup(r => r.ValidateCategoryOwnershipAsync(categoryId, _testUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var request = new TransactionCreateRequest
        {
            AccountId = accountId,
            Amount = 100.00m,
            Timestamp = DateTime.UtcNow,
            CategoryId = categoryId,
            Notes = "Unauthorized category"
        };

        // Act
        var result = await service.CreateTransactionAsync(request, _testUserId, CancellationToken.None);

        // Assert
        Assert.Null(result);
        mockRepository.Verify(r => r.CreateAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task WhenUpdatingTransaction_WithValidOwnership_ThenRepositoryIsCalledAndReturnsTrue()
    {
        // Arrange
        var mockRepository = GetMockRepository();
        var mockLogger = GetMockLogger();
        var service = new TransactionService(mockRepository.Object, mockLogger.Object);
        
        var transactionId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var updatedTransaction = new Transaction
        {
            Id = transactionId,
            AccountId = accountId,
            Amount = 150.00m,
            Timestamp = DateTime.UtcNow,
            CategoryId = categoryId,
            Notes = "Updated note"
        };
        
        mockRepository
            .Setup(r => r.ValidateAccountOwnershipAsync(accountId, _testUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        mockRepository
            .Setup(r => r.ValidateCategoryOwnershipAsync(categoryId, _testUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        mockRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Transaction>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedTransaction);

        var request = new TransactionUpdateRequest
        {
            Id = transactionId,
            AccountId = accountId,
            Amount = 150.00m,
            Timestamp = DateTime.UtcNow,
            CategoryId = categoryId,
            Notes = "Updated note"
        };

        // Act
        var result = await service.UpdateTransactionAsync(request, _testUserId, CancellationToken.None);

        // Assert
        Assert.True(result);
        mockRepository.Verify(
            r => r.UpdateAsync(
                It.Is<Transaction>(t =>
                    t.Id == transactionId &&
                    t.AccountId == accountId &&
                    t.Amount == 150.00m &&
                    t.CategoryId == categoryId &&
                    t.Notes == "Updated note"),
                _testUserId,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task WhenUpdatingTransaction_WithInvalidAccountOwnership_ThenReturnsFalse()
    {
        // Arrange
        var mockRepository = GetMockRepository();
        var mockLogger = GetMockLogger();
        var service = new TransactionService(mockRepository.Object, mockLogger.Object);
        
        var transactionId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        
        mockRepository
            .Setup(r => r.ValidateAccountOwnershipAsync(accountId, _testUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var request = new TransactionUpdateRequest
        {
            Id = transactionId,
            AccountId = accountId,
            Amount = 150.00m,
            Timestamp = DateTime.UtcNow,
            CategoryId = categoryId,
            Notes = "Unauthorized update"
        };

        // Act
        var result = await service.UpdateTransactionAsync(request, _testUserId, CancellationToken.None);

        // Assert
        Assert.False(result);
        mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Transaction>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task WhenUpdatingTransaction_WithInvalidCategoryOwnership_ThenReturnsFalse()
    {
        // Arrange
        var mockRepository = GetMockRepository();
        var mockLogger = GetMockLogger();
        var service = new TransactionService(mockRepository.Object, mockLogger.Object);
        
        var transactionId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        
        mockRepository
            .Setup(r => r.ValidateAccountOwnershipAsync(accountId, _testUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        mockRepository
            .Setup(r => r.ValidateCategoryOwnershipAsync(categoryId, _testUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var request = new TransactionUpdateRequest
        {
            Id = transactionId,
            AccountId = accountId,
            Amount = 150.00m,
            Timestamp = DateTime.UtcNow,
            CategoryId = categoryId,
            Notes = "Unauthorized category update"
        };

        // Act
        var result = await service.UpdateTransactionAsync(request, _testUserId, CancellationToken.None);

        // Assert
        Assert.False(result);
        mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Transaction>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task WhenUpdatingTransaction_WithNonExistentTransaction_ThenReturnsFalse()
    {
        // Arrange
        var mockRepository = GetMockRepository();
        var mockLogger = GetMockLogger();
        var service = new TransactionService(mockRepository.Object, mockLogger.Object);
        
        var transactionId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        
        mockRepository
            .Setup(r => r.ValidateAccountOwnershipAsync(accountId, _testUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        mockRepository
            .Setup(r => r.ValidateCategoryOwnershipAsync(categoryId, _testUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        mockRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Transaction>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Transaction?)null);

        var request = new TransactionUpdateRequest
        {
            Id = transactionId,
            AccountId = accountId,
            Amount = 150.00m,
            Timestamp = DateTime.UtcNow,
            CategoryId = categoryId,
            Notes = "Non-existent"
        };

        // Act
        var result = await service.UpdateTransactionAsync(request, _testUserId, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task WhenGettingAllTransactions_ThenRepositoryIsCalledWithUserId()
    {
        // Arrange
        var mockRepository = GetMockRepository();
        var mockLogger = GetMockLogger();
        var service = new TransactionService(mockRepository.Object, mockLogger.Object);
        
        var accountId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var transactions = new List<Transaction>
        {
            new Transaction { Id = Guid.NewGuid(), AccountId = accountId, Amount = 100.00m, Timestamp = DateTime.UtcNow, CategoryId = categoryId, Notes = "Transaction 1" },
            new Transaction { Id = Guid.NewGuid(), AccountId = accountId, Amount = 200.00m, Timestamp = DateTime.UtcNow, CategoryId = categoryId, Notes = "Transaction 2" }
        };
        
        mockRepository
            .Setup(r => r.GetAllAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(transactions);

        // Act
        await service.GetAllTransactionsAsync(_testUserId, CancellationToken.None);

        // Assert
        mockRepository.Verify(r => r.GetAllAsync(_testUserId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WhenGettingAllTransactions_ThenReturnsTransactionDtos()
    {
        // Arrange
        var mockRepository = GetMockRepository();
        var mockLogger = GetMockLogger();
        var service = new TransactionService(mockRepository.Object, mockLogger.Object);
        
        var transactionId1 = Guid.NewGuid();
        var transactionId2 = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var transactions = new List<Transaction>
        {
            new Transaction { Id = transactionId1, AccountId = accountId, Amount = 100.00m, Timestamp = DateTime.UtcNow, CategoryId = categoryId, Notes = "Transaction 1" },
            new Transaction { Id = transactionId2, AccountId = accountId, Amount = 200.00m, Timestamp = DateTime.UtcNow, CategoryId = categoryId, Notes = "Transaction 2" }
        };
        
        mockRepository
            .Setup(r => r.GetAllAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(transactions);

        // Act
        var result = await service.GetAllTransactionsAsync(_testUserId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, dto => dto.Id == transactionId1 && dto.Amount == 100.00m && dto.Notes == "Transaction 1");
        Assert.Contains(result, dto => dto.Id == transactionId2 && dto.Amount == 200.00m && dto.Notes == "Transaction 2");
    }

    [Fact]
    public async Task WhenDeletingTransaction_WithValidOwnership_ThenReturnsTrue()
    {
        // Arrange
        var mockRepository = GetMockRepository();
        var mockLogger = GetMockLogger();
        var service = new TransactionService(mockRepository.Object, mockLogger.Object);
        
        var transactionId = Guid.NewGuid();
        mockRepository
            .Setup(r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await service.DeleteTransactionAsync(transactionId, _testUserId, CancellationToken.None);

        // Assert
        Assert.True(result);
        mockRepository.Verify(r => r.DeleteAsync(transactionId, _testUserId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WhenDeletingTransaction_WithInvalidOwnership_ThenReturnsFalse()
    {
        // Arrange
        var mockRepository = GetMockRepository();
        var mockLogger = GetMockLogger();
        var service = new TransactionService(mockRepository.Object, mockLogger.Object);
        
        var transactionId = Guid.NewGuid();
        mockRepository
            .Setup(r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await service.DeleteTransactionAsync(transactionId, _otherUserId, CancellationToken.None);

        // Assert
        Assert.False(result);
    }
}
