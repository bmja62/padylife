using Entities.Common;
using Entities.Orders;
using Entities.Users;
using Parbad.Storage.EntityFrameworkCore.Domain;

namespace Entities.Payments;

public class Payment : PaymentEntity, IEntity
{

    public long? UserId { get; set; }
    public long? OrderId { get; set; }
    public bool WalletCharge { get; set; }
    public string GatewayReferenceNumber { get; set; }

    public User User { get; set; }
    public Order Order { get; set; }


    public void SetGatewayReferenceNumber(string referenceNumber)
    {
        GatewayReferenceNumber = referenceNumber;
    }
    public void SetPayed()
    {
        UpdatedOn = DateTime.Now;
        IsPaid = true;
    }
}


