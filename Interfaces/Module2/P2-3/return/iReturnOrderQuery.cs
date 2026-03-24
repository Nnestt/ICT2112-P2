using ProRental.Domain.Entities;
using ProRental.Domain.Enums;

namespace ProRental.Interfaces.Domain;

public interface iReturnOrderQuery
{
    ReturnRequestStatus GetReturnStatus(int returnRequestId);
    Returnrequest? GetReturnRequestByOrderId(int orderId);
    ICollection<Returnrequest> GetAllReturnRequests();
    Returnrequest? GetReturnRequestById(int returnRequestId);
}