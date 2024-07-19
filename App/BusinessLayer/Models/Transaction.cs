using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace WalletSytem.BusinessLayer.Models;
interface IValueObject<T>
{
    public T Id { get; set; }

}
public class Transaction: IValueObject<string>
{
    private string id;

    public string Id { get { return id ?? (id = CalculateHash()); }  set => id = value; }

    public long WalletId { get; set; }

    public decimal BalanceBefore { get; set; }
    public decimal BalanceAfter { get; set; }
    public DateTime Timestamp { get; set; }
    public Guid ProviderId { get; internal set; }
    public bool Commited { get; internal set; }

    public string CalculateHash()
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            string rawData = $"{WalletId}{BalanceBefore}{BalanceAfter}{Timestamp.ToString("o")}{ProviderId}";
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
