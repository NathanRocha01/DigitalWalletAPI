public class WalletService
{
    private readonly WalletDbContext _context;
    public WalletService(WalletDbContext context)
    {
        _context = context;
    }
    public decimal GetUserBalance(int userId)
    {
        var wallet = _context.Wallet.SingleOrDefault(w => w.UserId == userId);
        return wallet?.Amount ?? 0; 
    }

    public void AddBalance(int userId, decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be positive");

        var wallet = _context.Wallet.SingleOrDefault(w => w.UserId == userId);

        if (wallet == null)
            throw new KeyNotFoundException("Wallet not found");

        wallet.Amount += amount;
        _context.SaveChanges();
    }
}