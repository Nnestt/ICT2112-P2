using System;
using System.Collections.Generic;

namespace ProRental.Domain.Entities;

public partial class Buildingfootprint
{
    private int _buildingcarbonfootprintid;
    public int Buildingcarbonfootprintid { get => _buildingcarbonfootprintid; set => _buildingcarbonfootprintid = value; }

    private DateTime _timehourly;
    public DateTime Timehourly { get => _timehourly; set => _timehourly = value; }

    private string? _zone;
    public string? Zone { get => _zone; set => _zone = value; }

    private string? _block;
    public string? Block { get => _block; set => _block = value; }

    private string? _floor;
    public string? Floor { get => _floor; set => _floor = value; }

    private string? _room;
    public string? Room { get => _room; set => _room = value; }

    private double _totalroomco2;
    public double Totalroomco2 { get => _totalroomco2; set => _totalroomco2 = value; }
}
