using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class UserService
{

    private readonly WalletDbContext _context;
    private readonly IConfiguration _config;

    public UserService(WalletDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public string Authenticate([FromBody] LoginRequest request)
    {
        var user = _context.Users.SingleOrDefault(u => u.Email == request.Email);
        if (user == null || !AuthHelper.VerifyPassword(request.Password, user.Password))
            throw new UnauthorizedAccessException("Invalid credentials");

        return AuthHelper.GenerateToken(user, _config);
    }

    public void CreateUser([FromBody] RegisterRequest request)
    {
        if (_context.Users.Any(u => u.Email == request.Email))
            throw new ArgumentException("Email already registered");

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            Password = AuthHelper.HashPassword(request.Password)
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        var wallet = new Wallet
        {
            UserId = user.Id,
            Amount = 0
        };

        _context.Wallet.Add(wallet);
        _context.SaveChangesAsync(); // Salva a carteira
    }

    public async Task<User?> GetUserById(int id)
    {
        return await _context.Users.FindAsync(id);
    }

}