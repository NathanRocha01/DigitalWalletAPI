public class Transferencia
{
    public int Id { get; set; }
    public int CarteiraOrigemId { get; set; }
    public int CarteiraDestinoId { get; set; }
    public decimal Valor { get; set; }
    public DateTime Data { get; set; }
}