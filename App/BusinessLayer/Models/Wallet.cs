using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WalletSytem.BusinessLayer.Models;

public class Wallet
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

    public long Id { get; set; }
    public string UserId { get; set; }
    
    [JsonIgnore]
    public virtual User User { get; set; }
    
    public decimal Balance { get; set; }
    public string Currency { get; set; }
    public bool Frozen { get; internal set; }
}


public enum Operation
{
    Add, Remove
}
