using Entities.Common;
using Entities.Wallets;

namespace Entities.Users
{
    public class UserPoints : BaseEntity<long>
    {
        private UserPoints() { }

        public UserPoints(long userId)
        {
            UserId = userId;
            PointTransactions = new List<PointTransaction>();
        }

        // Foreign Key
        public long UserId { get; private set; }

        // محاسبه امتیازات جمع‌آوری شده (کاهش‌نیافته)
        public int EarnedPoints => PointTransactions
            .Where(t => t.TransactionType == PointTransactionType.Earn && !t.IsReverted)
            .Sum(t => t.Amount);

        // محاسبه امتیازات مصرف شده
        public int ConsumedPoints => PointTransactions
            .Where(t => t.TransactionType == PointTransactionType.Consume && !t.IsReverted)
            .Sum(t => t.Amount);

        // امتیازات قابل استفاده (جمع کل کاهش‌نیافته)
        public int AvailablePoints => EarnedPoints - ConsumedPoints;

        // نرخ تبدیل امتیاز به پول (به ریال)
        public const int PointsToMoneyRatio = 1000; // هر 1000 امتیاز = 1000 تومان

        // Navigation Properties
        public User User { get; private set; }
        public ICollection<PointTransaction> PointTransactions { get; private set; } = new List<PointTransaction>();

        // متد افزایش امتیاز
        public void EarnPoints(int amount, string reason, long? referenceId = null, EntityType referenceType = default)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero", nameof(amount));

            var transaction = new PointTransaction(
                userId: UserId,
                amount: amount,
                transactionType: PointTransactionType.Earn,
                reason: reason,
                referenceId: referenceId,
                referenceEntityType: referenceType);

            PointTransactions.Add(transaction);

        }

        // متد مصرف امتیاز
        public void ConsumePoints(int amount, string reason, long? referenceId = null, EntityType referenceType = default)
        {
            if (amount >= 0 && AvailablePoints >= amount)
            {
                var transaction = new PointTransaction(
                    userId: UserId,
                    amount: amount,
                    transactionType: PointTransactionType.Consume,
                    reason: reason,
                    referenceId: referenceId,
                    referenceEntityType: referenceType);

                PointTransactions.Add(transaction);

            }
        }

        // متد برگشت امتیاز (برای مواردی که تراکنش باید برگشت بخورد)
        public void RevertTransaction(long transactionId, string reason)
        {
            var transaction = PointTransactions.FirstOrDefault(t => t.Id == transactionId);
            if (transaction != null && !transaction.IsReverted)
            {
                // ایجاد تراکنش معکوس
                var reverseTransaction = new PointTransaction(
                    userId: UserId,
                    amount: transaction.Amount,
                    transactionType: transaction.TransactionType == PointTransactionType.Earn
                        ? PointTransactionType.Consume
                        : PointTransactionType.Earn,
                    reason: $"Revert: {reason}",
                    referenceId: transaction.Id,
                    referenceEntityType: transaction.ReferenceEntityType);

                transaction.IsReverted = true;
                transaction.RevertReason = reason;
                PointTransactions.Add(reverseTransaction);
            }

        }

        // محاسبه ارزش پولی امتیازات
        public decimal CalculateMoneyValue(int points)
        {
            return points * (PointsToMoneyRatio / 100m); // تبدیل به تومان
        }
        // در کلاس UserPoints این متد را اضافه کنید
        public void ConvertPointsToWalletCredit(Wallet wallet, int pointsToConvert, string description = "تبدیل امتیاز به اعتبار")
        {
            if (wallet == null)
                throw new ArgumentNullException(nameof(wallet));

            if (pointsToConvert <= 0)
                throw new ArgumentException("مقدار امتیاز باید بیشتر از صفر باشد", nameof(pointsToConvert));

            if (pointsToConvert > AvailablePoints)
                throw new InvalidOperationException("امتیاز قابل استفاده کاربر کافی نیست");

            // محاسبه ارزش پولی امتیازات
            decimal moneyValue = CalculateMoneyValue(pointsToConvert);

            // ثبت تراکنش مصرف امتیاز
            ConsumePoints(pointsToConvert, description);

            // واریز معادل پولی به کیف پول
            wallet.Deposit(moneyValue, UserId, $"واریز بابت تبدیل {pointsToConvert} امتیاز");

            // می‌توانید لاگ اضافی هم اینجا ثبت کنید اگر نیاز بود
        }
    }
    public class PointTransaction : BaseEntity<long>
    {
        private PointTransaction() { }

        public PointTransaction(long userId, int amount, PointTransactionType transactionType, string reason, long? referenceId, EntityType referenceEntityType)
        {
            UserId = userId;
            Amount = amount;
            TransactionType = transactionType;
            Reason = reason;
            ReferenceId = referenceId;
            TransactionDate = DateTime.UtcNow;
            IsReverted = false;
            ReferenceEntityType = referenceEntityType;
        }

        public long UserId { get; private set; }
        public int Amount { get; private set; }
        public PointTransactionType TransactionType { get; private set; }
        public string Reason { get; private set; }
        public long? ReferenceId { get; private set; }
        public EntityType ReferenceEntityType { get; set; }
        public DateTime TransactionDate { get; private set; }
        public bool IsReverted { get; set; }
        public string RevertReason { get; set; }

        // Navigation Properties
        public UserPoints UserPoints { get; private set; }
    }

    public enum PointTransactionType
    {
        Earn = 1,    // افزایش امتیاز
        Consume = 2  // مصرف امتیاز
    }
}
