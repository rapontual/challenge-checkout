using Challenge.Core.Model;

namespace Challenge.Service
{
    public interface IApprovePaymentService
    {
        PaymentTransaction ApprovePayment(PaymentTransaction payment);

        string ValidateCard(string cardNumber, string expire, string CVV, string owernName);
    }
}
