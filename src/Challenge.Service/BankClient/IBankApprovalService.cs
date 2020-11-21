namespace Challenge.Service.BankClient
{
    public interface IBankApprovalService
    {
        BankApprovalResponse Approve(BankApprovalRequest request);
    }
}
