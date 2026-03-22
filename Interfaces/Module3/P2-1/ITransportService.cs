using System.Collections;
using ProRental.Domain.Entities;

namespace ProRental.Interfaces.Module3.P2_1;

public interface ITransportService
{
    Transport GetTransportation(int id);
    ArrayList GetAllTransportations();
    float GetLoadLimit();
    List<Transport> GetTransportationType();
}
