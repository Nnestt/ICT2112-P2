namespace ProRental.Domain.Entities;
using ProRental.Domain.Enums;
public partial class Purchaseorder
{
    private POStatus _status;
    private POStatus status { get => _status; set => _status = value; }
    public void UpdateStatus(POStatus newValue) => _status = newValue;
}
