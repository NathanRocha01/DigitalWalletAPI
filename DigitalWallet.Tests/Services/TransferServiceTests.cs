using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using DigitalWalletAPI.Migrations;

public class TransferServiceTests
{
    private readonly Mock<IWalletDbContext> _mockRepo;
    private readonly TransferService _service;
    private readonly List<Wallet> _mockWallets = new();
    private readonly List<Transfer> _mockTransfers = new();

    public TransferServiceTests()
    {
        _mockRepo = new Mock<IWalletDbContext>();
        var mockWalletDbSet = MockDbSetHelper.GetQueryableMockDbSet(_mockWallets);
        var mockTransferDbSet = MockDbSetHelper.GetQueryableMockDbSet(_mockTransfers);

        _mockRepo.Setup(x => x.Wallet).Returns(mockWalletDbSet.Object);
        _mockRepo.Setup(x => x.Transfers).Returns(mockTransferDbSet.Object);
        _mockRepo.Setup(x => x.SaveChanges()).Returns(1);

        _service = new TransferService(_mockRepo.Object);
    }

    [Fact]
    public void ExecuteTransferAsync_WithValidData_ShouldCompleteTransfer()
    {
        // Arrange
        var originUser = CreateValidUser(1);
        var destinationUser = CreateValidUser(2); ;
        destinationUser.Email = "destination@test.com";

        _mockWallets.AddRange(new[]
            {
            CreateValidWallet(originUser.Id, 100),
            CreateValidWallet(destinationUser.Id, 50)
        });

        var request = new TransferRequest
        {
            DestinationId = destinationUser.Id,
            Amount = 30
        };

        // Act
        _service.ExecuteTransfer(originUser.Id, request);

        // Assert
        _mockWallets[0].Amount.Should().Be(70);
        _mockWallets[1].Amount.Should().Be(80);
        _mockTransfers.Should().HaveCount(1);
    }

    [Fact]
    public void ExecuteTransfer_WithInsufficientBalance_ShouldThrow()
    {
        // Arrange
        _mockWallets.AddRange(new[]
            {
            CreateValidWallet(1, 10),
            CreateValidWallet(2, 50)
        });

        var request = new TransferRequest
        {
            DestinationId = 2,
            Amount = 30
        };

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            _service.ExecuteTransfer(1, request));

        Assert.Contains("Insufficient balance", exception.Message);
    }

    [Fact]
    public void ExecuteTransfer_WithInvalidUser_ShouldThrowArgumentException()
    {
        // Arrange
        _mockWallets.Add(new Wallet { Id = 1, UserId = 1, Amount = 100 });

        var invalidRequest = new TransferRequest
        {
            DestinationId = 999,
            Amount = 10
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            _service.ExecuteTransfer(1, invalidRequest));

        exception.Message.Should().Contain("Invalid wallet");
    }
    [Fact]
    public void ExecuteTransfer_WithNegativeAmount_ShouldThrowArgumentException()
    {
        // Arrange
        _mockWallets.AddRange(new[]
        {
        new Wallet { Id = 1, UserId = 1, Amount = 100 },
        new Wallet { Id = 2, UserId = 2, Amount = 50 }
    });

        var invalidRequest = new TransferRequest
        {
            DestinationId = 2,
            Amount = -10
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            _service.ExecuteTransfer(1, invalidRequest));

        exception.Message.Should().Contain("Invalid transfer amount");
    }

    private User CreateValidUser(int? id = null)
    {
        return new User
        {
            Id = id ?? new Random().Next(1, 10000),
            Name = "Test User",
            Email = "valid@test.com",
            Password = AuthHelper.HashPassword("SenhaV@lida123") 
        };
    }

    private Wallet CreateValidWallet(int userId, decimal amount)
    {
        return new Wallet
        {
            Id = userId,
            UserId = userId,
            Amount = amount
        };
    }

}