public class TransferService
{
    private readonly IWalletDbContext _context;

    public TransferService(IWalletDbContext dbContext)
    {
        _context = dbContext;
    }

    public void ExecuteTransfer(int originUserId, TransferRequest request)
    {
        var originWallet = _context.Wallet.SingleOrDefault(w => w.UserId == originUserId);
        var destinationWallet = _context.Wallet.SingleOrDefault(w => w.UserId == request.DestinationId);

        if (originWallet == null || destinationWallet == null)
            throw new ArgumentException("Invalid wallet");

        if (originWallet.Amount < request.Amount)
            throw new InvalidOperationException("Insufficient balance");

        if(request.Amount < 0)
          throw new ArgumentException("Invalid transfer amount");


        originWallet.Amount -= request.Amount;
        destinationWallet.Amount += request.Amount;

        _context.Transfers.Add(new Transfer
        {
            OriginWalletId = originUserId,
            DestinationWalletId = request.DestinationId,
            Amount = request.Amount,
            Date = DateTime.UtcNow  
        });

        _context.SaveChanges();
    }

    public List<Transfer> GetUserTransfers(int userId, DateTime? startDate, DateTime? endDate)
    {
        return _context.Transfers
            .Where(t => t.OriginWalletId == userId || t.DestinationWalletId == userId)
            .Where(t => (!startDate.HasValue || t.Date >= startDate) &&
                        (!endDate.HasValue || t.Date <= endDate))
            .OrderByDescending(t => t.Date)
            .ToList();
    }
}