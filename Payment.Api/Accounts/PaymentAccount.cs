namespace Payment.Api.Accounts;

public class PaymentAccount
{
    private const long MaxBalance = 1_000_000;
    private const long MaxPoints = 1_000_000;

    /// <summary>
    /// ポイント利用前の購入金額100円につき1ポイントを付与します。
    /// </summary>
    private const long PurchaseAmountPerEarnedPoint = 100;

    public long Id { get; private set; }
    public long Balance { get; private set; }
    public long Points { get; private set; }

    public void TopUp(long topUpAmount)
    {
        if (topUpAmount is < 1 or > MaxBalance)
        {
            throw new ArgumentException($"チャージ金額は1円以上{MaxBalance:N0}円以下で指定してください。");
        }

        if (Balance + topUpAmount > MaxBalance)
        {
            throw new InvalidOperationException($"チャージ後の残高が上限の{MaxBalance:N0}円を超えます。");
        }

        Balance += topUpAmount;
    }

    public PaymentResult Pay(long purchaseAmount, long pointsToUse)
    {
        if (purchaseAmount <= 0)
        {
            throw new ArgumentException("購入金額は1円以上で指定してください。");
        }

        if (pointsToUse < 0 || pointsToUse > purchaseAmount)
        {
            throw new ArgumentException("利用ポイント数は0以上、購入金額以下で指定してください。");
        }

        if (pointsToUse > Points)
        {
            throw new InvalidOperationException("保有ポイントが不足しています。");
        }

        var amountToDebit = purchaseAmount - pointsToUse;

        if (amountToDebit > Balance)
        {
            throw new InvalidOperationException("決済に必要な残高が不足しています。");
        }

        var earnedPoints = purchaseAmount / PurchaseAmountPerEarnedPoint;
        var pointsAfterPayment = Points - pointsToUse + earnedPoints;

        if (pointsAfterPayment > MaxPoints)
        {
            throw new InvalidOperationException($"決済後の保有ポイントが上限の{MaxPoints:N0}ポイントを超えます。");
        }

        Balance -= amountToDebit;
        Points = pointsAfterPayment;

        return new PaymentResult(
            DebitedAmount: amountToDebit,
            EarnedPoints: earnedPoints
        );
    }
}