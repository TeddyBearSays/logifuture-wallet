using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Security.Cryptography;
using System.Text.Json.Serialization;

namespace WalletSytem.BusinessLayer.Models;

public class User
{

    public string Id { get; set; }

    [JsonIgnore]
    public ICollection<Wallet> Wallets { get; set; }

    [Required]
    [StringLength(50)]
    public string Username { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string PasswordHash { get; set; }

    public DateTime DateCreated { get; set; }

    public bool Frozen { get; set; }

   
}
