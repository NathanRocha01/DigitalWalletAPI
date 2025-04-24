using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using Microsoft.Extensions.Configuration;

public class UserServiceTests
{
    private readonly Mock<IWalletDbContext> _mockContext;
    private readonly Mock<IConfiguration> _mockConfig;
    private readonly UserService _userService;

    private readonly List<User> _testUsers;
    private readonly List<Wallet> _testWallets;
    private readonly Mock<DbSet<User>> _mockUsersDbSet;
    private readonly Mock<DbSet<Wallet>> _mockWalletsDbSet;

    public UserServiceTests()
    {
        _mockContext = new Mock<IWalletDbContext>();
        _mockConfig = new Mock<IConfiguration>();

        _mockConfig.Setup(c => c["Jwt:Key"]).Returns("CHAVE_SECRETA_COM_NO_MINIMO_32_CHARACTERS");
        _mockConfig.Setup(c => c["Jwt:Issuer"]).Returns("CarteiraDigitalAPI");

        _testUsers = new List<User>
        {
            new User
            {
                Id = 1,
                Email = "existing@example.com",
                Password = AuthHelper.HashPassword("valid_password")
            }
        };

        _testWallets = new List<Wallet>();

        _mockUsersDbSet = MockDbSetHelper.GetQueryableMockDbSet(_testUsers);
        _mockWalletsDbSet = MockDbSetHelper.GetQueryableMockDbSet(_testWallets);

        _mockContext.Setup(c => c.Users).Returns(_mockUsersDbSet.Object);
        _mockContext.Setup(c => c.Wallet).Returns(_mockWalletsDbSet.Object);
        _mockContext.Setup(c => c.SaveChanges()).Returns(1);

        _userService = new UserService(_mockContext.Object, _mockConfig.Object);
    }

    [Fact]
    public void Authenticate_WithValidCredentials_ReturnsToken()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "existing@example.com",
            Password = "valid_password"
        };

        // Act
        var result = _userService.Authenticate(request);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void Authenticate_WithInvalidCredentials_ThrowsException()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "nonexistent@example.com",
            Password = "wrong_password"
        };

        // Act & Assert
        Assert.Throws<UnauthorizedAccessException>(() => _userService.Authenticate(request));
    }

    [Fact]
    public void CreateUser_WithNewEmail_CreatesUserAndWallet()
    {
        // Arrange
        var initialUserCount = _testUsers.Count;
        var initialWalletCount = _testWallets.Count;

        var request = new RegisterRequest
        {
            Name = "New User",
            Email = "new@example.com",
            Password = "new_password"
        };

        // Act
        _userService.CreateUser(request);

        // Assert
        Assert.Equal(initialUserCount + 1, _testUsers.Count);
        Assert.Equal(initialWalletCount + 1, _testWallets.Count);
        Assert.Equal(request.Email, _testUsers.Last().Email);
        Assert.Equal(0, _testWallets.Last().Amount);
    }

    [Fact]
    public void CreateUser_WithExistingEmail_ThrowsException()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Name = "Existing User",
            Email = "existing@example.com", // Email já existe
            Password = "password"
        };

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => _userService.CreateUser(request));
        Assert.Contains("already registered", ex.Message);
    }

    [Fact]
    public async Task GetUserById_ExistingUser_ReturnsUser()
    {
        // Arrange
        var existingUserId = 1;
        _mockUsersDbSet.Setup(m => m.FindAsync(existingUserId))
                      .ReturnsAsync(_testUsers.First());

        // Act
        var result = await _userService.GetUserById(existingUserId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(existingUserId, result.Id);
    }

    [Fact]
    public async Task GetUserById_NonExistingUser_ReturnsNull()
    {
        // Arrange
        var nonExistingUserId = 999;
        _mockUsersDbSet.Setup(m => m.FindAsync(nonExistingUserId))
                      .ReturnsAsync((User)null);

        // Act
        var result = await _userService.GetUserById(nonExistingUserId);

        // Assert
        Assert.Null(result);
    }
}