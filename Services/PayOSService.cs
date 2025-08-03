using Net.payOS;
using Net.payOS.Types;

public class PayOSService
{
    private readonly PayOS _payOS;

    public PayOSService(IConfiguration config)
    {
        _payOS = new PayOS(
            config["PayOS:ClientId"],
            config["PayOS:ApiKey"],
            config["PayOS:ChecksumKey"]
        );
    }

    public async Task<CreatePaymentResult?> TaoLinkThanhToan(
        long orderCode,
        int amount,
        string description,
        List<ItemData> items,
        string returnUrl,
        string cancelUrl)
    {
        var paymentData = new PaymentData(
            orderCode,
            amount,
            description,
            items,
            cancelUrl,
            returnUrl
        );

        return await _payOS.createPaymentLink(paymentData);
    }
}
