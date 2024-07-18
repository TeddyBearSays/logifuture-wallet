namespace WalletSytem.BusinessLayer.Models;

public class Wallet
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    
    public decimal Balance { get; set; }
    public string Currency { get; set; }
    public bool Frozen { get; internal set; }
}


public class User
{
    public Guid Id { get; set; }
  
    public ICollection<Wallet> Wallets { get; set; }
}


public enum Operation
{
    Add, Remove
}
public class Funds
{
    public Operation Operation { get; set; }
    public decimal Amount { get; set; }
    public DateTime Timestamp { get; }

}