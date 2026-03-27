using ProRental.Domain.Entities;
using ProRental.Data.Module3.P2_5.Interfaces;

namespace ProRental.Interfaces.Module3.P2_5;

public interface IStaffFootprintControl
{
    Task<Stafffootprint> CreateStaffFootprintAsync(int staffId, DateTime checkInTime, DateTime checkOutTime);
    Task<List<StaffFootprintListItem>> GetStaffFootprintsAsync();
    Task<Stafffootprint?> UpdateStaffFootprintAsync(int staffCarbonFootprintId, int staffId, DateTime checkInTime, DateTime checkOutTime);
    Task<bool> DeleteStaffFootprintAsync(int staffCarbonFootprintId);
    Task<List<StaffLookupItem>> GetStaffLookupAsync();
    Task<string?> GetDepartmentByStaffIdAsync(int staffId);
}
