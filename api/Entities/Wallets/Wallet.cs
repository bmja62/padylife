using Entities.Common;
using Entities.Users;

namespace Entities.Wallets
{
    public class Wallet : BaseEntity<long>, IEntity
    {
        private Wallet()
        {

        }

        public Wallet(long userId)
        {
            UserId = userId;
            Credit = 0;
        }
        public decimal Credit { get; private set; }
        public long UserId { get; private set; }
        public User User { get; private set; }

        public IReadOnlyCollection<WalletTransaction> Transactions => _transactions.AsReadOnly();
        private readonly List<WalletTransaction> _transactions = [];
        /// <summary>
        /// برداشت از حساب
        /// </summary>
        /// <param name="credit">مقدار برداشت</param>
        /// <param name="userId">کاربری که این عملیات را انجام داده است</param>
        /// <param name="description">توضیحات عملیات</param>

        public void Withdraw(decimal credit, long? userId, string description)
        {
            Credit -= credit;
            UpdatedAt = DateTime.Now;
            _transactions.Add(new WalletTransaction(Id, userId, credit, WalletTransactionOperation.Withdraw, description));
        }
        /// <summary>
        /// واریز به حساب
        /// </summary>
        /// <param name="credit">مقدار واریزی</param>
        /// <param name="userId">کاربری که این عملیات را انجام داده است</param>
        /// <param name="description">توضیحات عملیات</param>
        public void Deposit(decimal credit, long? userId, string description)
        {
            Credit += credit;
            UpdatedAt = DateTime.Now;
            _transactions.Add(new WalletTransaction(Id, userId, credit, WalletTransactionOperation.Deposit, description));
        }

    }

    public class WalletTransaction : BaseEntity
    {
        private WalletTransaction()
        {

        }
        public WalletTransaction(long walletId, long? userId, decimal amount, WalletTransactionOperation operation, string description)
        {
            WalletId = walletId;
            CreatedByUserId = userId;
            Amount = amount;
            Operation = operation;
            Description = description;

        }
        public Wallet Wallet { get; private set; }
        public long WalletId { get; private set; }
        public decimal Amount { get; private set; }
        public long? CreatedByUserId { get; private set; }
        public User CreatedByUser { get; private set; }
        public string Description { get; private set; }
        public WalletTransactionOperation Operation { get; private set; }
    }
}
