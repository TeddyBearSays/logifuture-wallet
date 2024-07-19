using System.ComponentModel.DataAnnotations;

namespace WalletSytem.BusinessLayer.Models;

public class Funds
{
    public Operation Operation { get; set; }
    public decimal Amount { get; set; }

    [Required]
    public DateTime Timestamp { get; }

}