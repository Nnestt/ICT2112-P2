using ProRental.Domain.Enums;

namespace ProRental.Domain.Entities;

public partial class DeliveryBatch
{
    public BatchStatus? DeliveryBatchStatus { get; set; }
}
