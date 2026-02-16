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
/// Tests for TransactionsController with user authentication
/// </summary>
public class TransactionsControllerTests
{
    private readonly Guid _testUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    private Mock<ITransactionService> GetMockTransactionService()
    {
        return new Mock<ITransactionService>();
    }

    private Mock<ILogger<TransactionsController>> GetMockLogger()
    {
        return new Mock<ILogger<TransactionsController>>();
    }

    private TransactionsController CreateControllerWithUser(Mock<ITransactionService> mockService, Mock<ILogger<TransactionsController>> mockLogger)
    {
        var controller = new TransactionsController(mockService.Object, mockLogger.Object);
        
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
    public async Task WhenCreatingTransaction_WithValidData_ThenReturnsCreatedResult()
    {
        // Arrange
        var mockService = GetMockTransactionService();
        var mockLogger = GetMockLogger();
        var controller = CreateControllerWithUser(mockService, mockLogger);
        
        var transactionId = Guid.NewGuid();
        mockService
            .Setup(s => s.CreateTransactionAsync(It.IsAny<TransactionCreateRequest>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(transactionId);
        
        var request = new TransactionCreateRequest
        {
            AccountId = Guid.NewGuid(),
            Amount = 100.00m,
            Timestamp = DateTime.UtcNow,
            CategoryId = Guid.NewGuid(),
            Notes = "Grocery shopping"
        };

        // Act
        var result = await controller.CreateTransaction(request, CancellationToken.None);

        // Assert
        Assert.IsType<CreatedResult>(result);
    }

    [Fact]
    public async Task WhenCreatingTransaction_WithValidData_ThenServiceIsCalledWithCorrectUserId()
    {
        // Arrange
        var mockService = GetMockTransactionService();
        var mockLogger = GetMockLogger();
        var controller = CreateControllerWithUser(mockService, mockLogger);
        
        var transactionId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        
        mockService
            .Setup(s => s.CreateTransactionAsync(It.IsAny<TransactionCreateRequest>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(transactionId);
        
        var request = new TransactionCreateRequest
        {
            AccountId = accountId,
            Amount = 250.50m,
            Timestamp = DateTime.UtcNow,
            CategoryId = categoryId,
            Notes = "Salary"
        };

        // Act
        await controller.CreateTransaction(request, CancellationToken.None);

        // Assert
        mockService.Verify(
            s => s.CreateTransactionAsync(
                It.Is<TransactionCreateRequest>(r =>
                    r.AccountId == accountId &&
                    r.Amount == 250.50m &&
                    r.CategoryId == categoryId &&
                    r.Notes == "Salary"),
                _testUserId,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task WhenCreatingTransaction_WithInvalidOwnership_ThenReturnsBadRequest()
    {
        // Arrange
        var mockService = GetMockTransactionService();
        var mockLogger = GetMockLogger();
        var controller = CreateControllerWithUser(mockService, mockLogger);
        
        mockService
            .Setup(s => s.CreateTransactionAsync(It.IsAny<TransactionCreateRequest>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid?)null);
        
        var request = new TransactionCreateRequest
        {
            AccountId = Guid.NewGuid(),
            Amount = 100.00m,
            Timestamp = DateTime.UtcNow,
            CategoryId = Guid.NewGuid(),
            Notes = "Unauthorized"
        };

        // Act
        var result = await controller.CreateTransaction(request, CancellationToken.None);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task WhenCreatingTransaction_WithInvalidOwnership_ThenReturnsProblemDetails()
    {
        // Arrange
        var mockService = GetMockTransactionService();
        var mockLogger = GetMockLogger();
        var controller = CreateControllerWithUser(mockService, mockLogger);
        
        mockService
            .Setup(s => s.CreateTransactionAsync(It.IsAny<TransactionCreateRequest>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid?)null);
        
        var request = new TransactionCreateRequest
        {
            AccountId = Guid.NewGuid(),
            Amount = 100.00m,
            Timestamp = DateTime.UtcNow,
            CategoryId = Guid.NewGuid(),
            Notes = "Unauthorized"
        };

        // Act
        var result = await controller.CreateTransaction(request, CancellationToken.None) as BadRequestObjectResult;

        // Assert
        Assert.NotNull(result);
        var problemDetails = result.Value as ProblemDetails;
        Assert.NotNull(problemDetails);
        Assert.Contains("does not exist or you do not have permission", problemDetails.Detail);
    }

    [Fact]
    public async Task WhenUpdatingTransaction_WithValidOwnership_ThenReturnsOkResult()
    {
        // Arrange
        var mockService = GetMockTransactionService();
        var mockLogger = GetMockLogger();
        var controller = CreateControllerWithUser(mockService, mockLogger);
        
        var transactionId = Guid.NewGuid();
        mockService
            .Setup(s => s.UpdateTransactionAsync(It.IsAny<TransactionUpdateRequest>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        var request = new TransactionUpdateRequest
        {
            Id = transactionId,
            AccountId = Guid.NewGuid(),
            Amount = 150.00m,
            Timestamp = DateTime.UtcNow,
            CategoryId = Guid.NewGuid(),
            Notes = "Updated grocery shopping"
        };

        // Act
        var result = await controller.UpdateTransaction(request, CancellationToken.None);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task WhenUpdatingTransaction_WithValidOwnership_ThenServiceIsCalledWithCorrectUserId()
    {
        // Arrange
        var mockService = GetMockTransactionService();
        var mockLogger = GetMockLogger();
        var controller = CreateControllerWithUser(mockService, mockLogger);
        
        var transactionId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        
        mockService
            .Setup(s => s.UpdateTransactionAsync(It.IsAny<TransactionUpdateRequest>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        var request = new TransactionUpdateRequest
        {
            Id = transactionId,
            AccountId = accountId,
            Amount = 175.25m,
            Timestamp = DateTime.UtcNow,
            CategoryId = categoryId,
            Notes = "Updated transportation"
        };

        // Act
        await controller.UpdateTransaction(request, CancellationToken.None);

        // Assert
        mockService.Verify(
            s => s.UpdateTransactionAsync(
                It.Is<TransactionUpdateRequest>(r =>
                    r.Id == transactionId &&
                    r.AccountId == accountId &&
                    r.Amount == 175.25m &&
                    r.CategoryId == categoryId &&
                    r.Notes == "Updated transportation"),
                _testUserId,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task WhenUpdatingTransaction_WithInvalidOwnership_ThenReturnsBadRequest()
    {
        // Arrange
        var mockService = GetMockTransactionService();
        var mockLogger = GetMockLogger();
        var controller = CreateControllerWithUser(mockService, mockLogger);
        
        var transactionId = Guid.NewGuid();
        mockService
            .Setup(s => s.UpdateTransactionAsync(It.IsAny<TransactionUpdateRequest>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        var request = new TransactionUpdateRequest
        {
            Id = transactionId,
            AccountId = Guid.NewGuid(),
            Amount = 999.99m,
            Timestamp = DateTime.UtcNow,
            CategoryId = Guid.NewGuid(),
            Notes = "Hacked transaction"
        };

        // Act
        var result = await controller.UpdateTransaction(request, CancellationToken.None);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task WhenUpdatingTransaction_WithInvalidOwnership_ThenReturnsProblemDetails()
    {
        // Arrange
        var mockService = GetMockTransactionService();
        var mockLogger = GetMockLogger();
        var controller = CreateControllerWithUser(mockService, mockLogger);
        
        var transactionId = Guid.NewGuid();
        mockService
            .Setup(s => s.UpdateTransactionAsync(It.IsAny<TransactionUpdateRequest>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        var request = new TransactionUpdateRequest
        {
            Id = transactionId,
            AccountId = Guid.NewGuid(),
            Amount = 999.99m,
            Timestamp = DateTime.UtcNow,
            CategoryId = Guid.NewGuid(),
            Notes = "Hacked transaction"
        };

        // Act
        var result = await controller.UpdateTransaction(request, CancellationToken.None) as BadRequestObjectResult;

        // Assert
        Assert.NotNull(result);
        var problemDetails = result.Value as ProblemDetails;
        Assert.NotNull(problemDetails);
        Assert.Contains("does not exist or you do not have permission", problemDetails.Detail);
    }

    [Fact]
    public async Task WhenGettingAllTransactions_ThenReturnsOkResult()
    {
        // Arrange
        var mockService = GetMockTransactionService();
        var mockLogger = GetMockLogger();
        var controller = CreateControllerWithUser(mockService, mockLogger);
        
        mockService
            .Setup(s => s.GetAllTransactionsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TransactionDto>());

        // Act
        var result = await controller.GetAllTransactions(CancellationToken.None);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task WhenGettingAllTransactions_ThenServiceIsCalledWithCorrectUserId()
    {
        // Arrange
        var mockService = GetMockTransactionService();
        var mockLogger = GetMockLogger();
        var controller = CreateControllerWithUser(mockService, mockLogger);
        
        mockService
            .Setup(s => s.GetAllTransactionsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TransactionDto>());

        // Act
        await controller.GetAllTransactions(CancellationToken.None);

        // Assert
        mockService.Verify(s => s.GetAllTransactionsAsync(_testUserId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WhenGettingAllTransactions_ThenReturnsTransactionDtos()
    {
        // Arrange
        var mockService = GetMockTransactionService();
        var mockLogger = GetMockLogger();
        var controller = CreateControllerWithUser(mockService, mockLogger);
        
        var transactions = new List<TransactionDto>
        {
            new TransactionDto
            {
                Id = Guid.NewGuid(),
                AccountId = Guid.NewGuid(),
                Amount = 100.00m,
                Timestamp = DateTime.UtcNow,
                CategoryId = Guid.NewGuid(),
                Notes = "Transaction 1"
            },
            new TransactionDto
            {
                Id = Guid.NewGuid(),
                AccountId = Guid.NewGuid(),
                Amount = 200.00m,
                Timestamp = DateTime.UtcNow,
                CategoryId = Guid.NewGuid(),
                Notes = "Transaction 2"
            }
        };
        
        mockService
            .Setup(s => s.GetAllTransactionsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(transactions);

        // Act
        var result = await controller.GetAllTransactions(CancellationToken.None) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var returnedTransactions = result.Value as List<TransactionDto>;
        Assert.NotNull(returnedTransactions);
        Assert.Equal(2, returnedTransactions.Count);
    }

    [Fact]
    public async Task WhenDeletingTransaction_WithValidOwnership_ThenReturnsOkResult()
    {
        // Arrange
        var mockService = GetMockTransactionService();
        var mockLogger = GetMockLogger();
        var controller = CreateControllerWithUser(mockService, mockLogger);
        
        var transactionId = Guid.NewGuid();
        mockService
            .Setup(s => s.DeleteTransactionAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await controller.DeleteTransaction(transactionId, CancellationToken.None);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task WhenDeletingTransaction_WithValidOwnership_ThenServiceIsCalledWithCorrectUserId()
    {
        // Arrange
        var mockService = GetMockTransactionService();
        var mockLogger = GetMockLogger();
        var controller = CreateControllerWithUser(mockService, mockLogger);
        
        var transactionId = Guid.NewGuid();
        mockService
            .Setup(s => s.DeleteTransactionAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await controller.DeleteTransaction(transactionId, CancellationToken.None);

        // Assert
        mockService.Verify(s => s.DeleteTransactionAsync(transactionId, _testUserId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WhenDeletingTransaction_WithInvalidOwnership_ThenReturnsBadRequest()
    {
        // Arrange
        var mockService = GetMockTransactionService();
        var mockLogger = GetMockLogger();
        var controller = CreateControllerWithUser(mockService, mockLogger);
        
        var transactionId = Guid.NewGuid();
        mockService
            .Setup(s => s.DeleteTransactionAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await controller.DeleteTransaction(transactionId, CancellationToken.None);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task WhenDeletingTransaction_WithInvalidOwnership_ThenReturnsProblemDetails()
    {
        // Arrange
        var mockService = GetMockTransactionService();
        var mockLogger = GetMockLogger();
        var controller = CreateControllerWithUser(mockService, mockLogger);
        
        var transactionId = Guid.NewGuid();
        mockService
            .Setup(s => s.DeleteTransactionAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await controller.DeleteTransaction(transactionId, CancellationToken.None) as BadRequestObjectResult;

        // Assert
        Assert.NotNull(result);
        var problemDetails = result.Value as ProblemDetails;
        Assert.NotNull(problemDetails);
        Assert.Contains("does not exist or you do not have permission", problemDetails.Detail);
    }
}
