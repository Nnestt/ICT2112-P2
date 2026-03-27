using System;

namespace ProRental.Domain.Entities;

public partial class Buildingfootprint
{
    private int _buildingcarbonfootprintid;
    private DateTime _timehourly;
    private string? _zone;
    private string? _block;
    private string? _floor;
    private string? _room;
    private double _totalroomco2;

    private int Buildingcarbonfootprintid
    {
        get => _buildingcarbonfootprintid;
        set => _buildingcarbonfootprintid = value;
    }

    private DateTime Timehourly
    {
        get => _timehourly;
        set => _timehourly = value;
    }

    private string? Zone
    {
        get => _zone;
        set => _zone = value;
    }

    private string? Block
    {
        get => _block;
        set => _block = value;
    }

    private string? Floor
    {
        get => _floor;
        set => _floor = value;
    }

    private string? Room
    {
        get => _room;
        set => _room = value;
    }

    private double Totalroomco2
    {
        get => _totalroomco2;
        set => _totalroomco2 = value;
    }

    private int GetBuildingcarbonfootprintid()
    {
        return _buildingcarbonfootprintid;
    }

    private void SetBuildingcarbonfootprintid(int buildingcarbonfootprintid)
    {
        _buildingcarbonfootprintid = buildingcarbonfootprintid;
    }

    private DateTime GetTimehourly()
    {
        return _timehourly;
    }

    private void SetTimehourly(DateTime timehourly)
    {
        _timehourly = timehourly;
    }

    private string? GetZone()
    {
        return _zone;
    }

    private void SetZone(string? zone)
    {
        _zone = zone;
    }

    private string? GetBlock()
    {
        return _block;
    }

    private void SetBlock(string? block)
    {
        _block = block;
    }

    private string? GetFloor()
    {
        return _floor;
    }

    private void SetFloor(string? floor)
    {
        _floor = floor;
    }

    private string? GetRoom()
    {
        return _room;
    }

    private void SetRoom(string? room)
    {
        _room = room;
    }

    private double GetTotalroomco2()
    {
        return _totalroomco2;
    }

    private void SetTotalroomco2(double totalroomco2)
    {
        _totalroomco2 = totalroomco2;
    }
}
