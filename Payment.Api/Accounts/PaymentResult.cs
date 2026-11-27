namespace Payment.Api.Accounts;

/// <summary>
/// 1件の決済で引き落とした金額と付与したポイント数を表します。
/// </summary>
/// <param name="DebitedAmount">ポイント利用分を差し引いた後、口座残高から実際に引き落とした金額です。</param>
/// <param name="EarnedPoints">ポイント利用前の購入金額をもとに、今回の決済で新たに付与したポイント数です。</param>
public sealed record PaymentResult(long DebitedAmount, long EarnedPoints);