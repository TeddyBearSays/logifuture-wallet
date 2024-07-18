namespace WalletSytem.BusinessLayer;

public interface ICorrelationIdProvider
{
    Guid CorrelationId { get; set; }
}

public class CorrelationIdProvider : ICorrelationIdProvider
{
    public Guid CorrelationId { get; set; }
}
