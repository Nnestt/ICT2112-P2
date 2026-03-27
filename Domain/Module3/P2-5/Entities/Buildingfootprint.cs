using System;

namespace ProRental.Domain.Entities;

public partial class Buildingfootprint
{
    public static Buildingfootprint Create(
        DateTime timehourly,
        string? zone,
        string? block,
        string? floor,
        string? room,
        double totalroomco2)
    {
        return new Buildingfootprint
        {
            Timehourly = timehourly,
            Zone = zone,
            Block = block,
            Floor = floor,
            Room = room,
            Totalroomco2 = totalroomco2
        };
    }

    public DateTime ReadTimehourly() => Timehourly;
    public string? ReadZone() => Zone;
    public string? ReadBlock() => Block;
    public string? ReadFloor() => Floor;
    public string? ReadRoom() => Room;
    public double ReadTotalroomco2() => Totalroomco2;
}
