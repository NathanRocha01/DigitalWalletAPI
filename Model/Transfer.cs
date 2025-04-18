public class Transfer
{
    public int Id { get; set; }
    public int OriginWalletId { get; set; }
    public int DestinationWalletId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}