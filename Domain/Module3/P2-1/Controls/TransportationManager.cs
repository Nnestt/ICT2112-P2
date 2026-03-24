using System.Collections;
using ProRental.Data.Module3.P2_1.Interfaces;
using ProRental.Domain.Entities;
using ProRental.Interfaces.Module3.P2_1;

namespace ProRental.Domain.Module3.P2_1.Controls;

public class TransportationManager : ITransportService
{
    private readonly ITransportMapper _transportMapper;
    private readonly ArrayList _transportations = new();

    public TransportationManager(ITransportMapper transportMapper)
    {
        _transportMapper = transportMapper;
    }

    public Transport GetTransportation(int id)
    {
        return _transportMapper.FindById(id);
    }

    public ArrayList GetAllTransportations()
    {
        _transportations.Clear();

        foreach (var transport in _transportMapper.FindAvailable())
        {
            _transportations.Add(transport);
        }

        return _transportations;
    }

    public float GetLoadLimit()
    {
        return _transportMapper.FindAvailable()
            .Select(t => t.ReadMaxLoadKg())
            .DefaultIfEmpty(0f)
            .Max();
    }

    public List<Transport> GetTransportationType()
    {
        return _transportMapper.FindAvailable()
            .GroupBy(t => t.ReadTransportationType())
            .Select(g => g.First())
            .ToList();
    }
}
