using ProRental.Data.UnitOfWork;
using ProRental.Domain.Entities;
using ProRental.Interfaces.Data;
using ProRental.Interfaces.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data.Common;

namespace ProRental.Domain.Controls;

public class RewardsControl : IRewardsControl
{
    private const double LowThreshold      = 50.0;
    private const double ModerateThreshold = 100.0;
    private const double HighThreshold     = 200.0;

    private readonly IOrderCarbonDataGateway  _carbonGateway;
    private readonly IRewardGateway           _rewardGateway;
    private readonly IOrderGateway            _orderGateway;
    private readonly IProductFootprintService _productFootprintService;
    private readonly AppDbContext             _db;

    public RewardsControl(
        IOrderCarbonDataGateway  carbonGateway,
        IRewardGateway           rewardGateway,
        IOrderGateway            orderGateway,
        IProductFootprintService productFootprintService,
        AppDbContext             db)
    {
        _carbonGateway           = carbonGateway;
        _rewardGateway           = rewardGateway;
        _orderGateway            = orderGateway;
        _productFootprintService = productFootprintService;
        _db                      = db;
    }

    public Ordercarbondatum CreateOrderCarbonData(int orderId, double totalCarbon)
    {
        Console.WriteLine(_db.Database.GetDbConnection().ConnectionString);

        var existing = _carbonGateway.FindByOrderId(orderId);
        if (existing is not null) return existing;

        double productCarbon   = _productFootprintService.CalculateProductFootprint(orderId);
        double packagingCarbon = 0.0;

        int    totalOrders   = _db.Orders.Count();
        double totalStaffCo2 = _db.Stafffootprints.Any()
            ? _db.Stafffootprints.ToList().Sum(s => s.GetTotalstaffco2())
            : 0.0;
        double staffCarbon = totalOrders > 0 ? totalStaffCo2 / totalOrders : 0.0;

        double totalBuildingCo2 = _db.Buildingfootprints.Any()
            ? _db.Buildingfootprints.ToList().Sum(b => b.ReadTotalroomco2())
            : 0.0;
        double buildingCarbon = totalOrders > 0 ? totalBuildingCo2 / totalOrders : 0.0;

        double total = productCarbon + packagingCarbon + staffCarbon + buildingCarbon;

        var record = Ordercarbondatum.Create(orderId, productCarbon, packagingCarbon,
            staffCarbon, buildingCarbon, total,
            ClassifyImpact(total), DateTime.UtcNow);

        _carbonGateway.Save(record);
        return record;
    }

    public int CalculateEcoScore(int orderId)
    {
        var data = _carbonGateway.FindByOrderId(orderId);
        if (data is null) return -1;

        const double maxCarbon = 400.0;
        int score = (int)Math.Round(Math.Max(0, (1.0 - data.GetTotalcarbon() / maxCarbon) * 100));
        return Math.Clamp(score, 0, 100);
    }

public Customerreward? DetermineReward(int orderId)
{
    var data = _carbonGateway.FindByOrderId(orderId);
    if (data is null) return null;

    var existing = _rewardGateway.FindByOrderCarbonDataId(data.GetOrdercarbondataid());
    if (existing is not null) return existing;

    (string type, double value)? rewardInfo = data.GetImpactlevel() switch
    {
        "Low"      => ("Voucher", 10.0),
        "Moderate" => ("Voucher", 5.0),
        _          => null
    };

    if (rewardInfo is null) return null;

    var order = _orderGateway.FindById(orderId);
    if (order is null) return null;

    var reward = Customerreward.Create(
        order.GetCustomerid(),
        data.GetOrdercarbondataid(),
        rewardInfo.Value.type,
        rewardInfo.Value.value,
        DateTime.UtcNow);

    _rewardGateway.Save(reward);
    return reward;
}

    public List<Order> GetAllOrders()        => _orderGateway.FindAll();
    public List<Ordercarbondatum> GetAllCarbonRecords() => _carbonGateway.FindAll();
    public List<Customerreward> GetAllRewards()         => _rewardGateway.FindAll();

    private static string ClassifyImpact(double totalCarbon) => totalCarbon switch
    {
        < LowThreshold      => "Low",
        < ModerateThreshold => "Moderate",
        < HighThreshold     => "High",
        _                   => "Very High"
    };
}
