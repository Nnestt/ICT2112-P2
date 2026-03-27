using ProRental.Domain.Module3.P2_5.Entities;
using ProRental.Domain.Entities;

namespace ProRental.Data.Module3.P2_5.Interfaces;

public interface IStaffFootprintGateway
{
    List<ChartData> GetHourlyChartData();
    List<ChartData> GetStaffGraphData();
    List<ChartData> GetHotspotData(int top = 5);
    Task<List<StaffFootprintListItem>> GetStaffFootprintsAsync();
    Task<bool> StaffExistsAsync(int staffId);
    Task<string?> GetDepartmentByStaffIdAsync(int staffId);
    Task<List<StaffLookupItem>> GetStaffLookupAsync();
    Task<Stafffootprint> CreateStaffFootprintAsync(int staffId, DateTime time, double hoursWorked, double totalStaffCo2);
    Task<Stafffootprint?> UpdateStaffFootprintAsync(int staffCarbonFootprintId, int staffId, DateTime time, double hoursWorked, double totalStaffCo2);
    Task<bool> DeleteStaffFootprintAsync(int staffCarbonFootprintId);
}

public sealed record StaffLookupItem(int StaffId, string Department);
public sealed record StaffFootprintListItem(
    int StaffCarbonFootprintId,
    int StaffId,
    DateTime Time,
    double HoursWorked,
    double TotalStaffCo2);
