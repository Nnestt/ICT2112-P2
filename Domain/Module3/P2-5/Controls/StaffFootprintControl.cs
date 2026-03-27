using ProRental.Data.Module3.P2_5.Interfaces;
using ProRental.Domain.Entities;
using ProRental.Domain.Entities.Module3;
using ProRental.Interfaces.Module3;
using ProRental.Interfaces.Module3.P2_5;

namespace ProRental.Domain.Module3.P2_5.Controls;

public sealed class StaffFootprintControl : IStaffFootprintControl
{
    private readonly IStaffFootprintGateway _staffGateway;
    private readonly StaffFootprintStrategy _staffFootprintStrategy;

    public StaffFootprintControl(IStaffFootprintGateway staffGateway)
    {
        _staffGateway = staffGateway;
        _staffFootprintStrategy = new StaffFootprintStrategy();
    }

    public Task<List<StaffFootprintListItem>> GetStaffFootprintsAsync()
    {
        return _staffGateway.GetStaffFootprintsAsync();
    }

    public Task<List<StaffLookupItem>> GetStaffLookupAsync()
    {
        return _staffGateway.GetStaffLookupAsync();
    }

    public Task<string?> GetDepartmentByStaffIdAsync(int staffId)
    {
        return _staffGateway.GetDepartmentByStaffIdAsync(staffId);
    }

    public async Task<Stafffootprint> CreateStaffFootprintAsync(int staffId, DateTime checkInTime, DateTime checkOutTime)
    {
        ValidateStaffInputs(staffId, checkInTime, checkOutTime);

        if (!await _staffGateway.StaffExistsAsync(staffId))
            throw new ArgumentException("staffId does not exist.", nameof(staffId));

        var department = await _staffGateway.GetDepartmentByStaffIdAsync(staffId);
        if (string.IsNullOrWhiteSpace(department))
            throw new InvalidOperationException("Unable to determine department for this staffId.");

        var (roundedHoursWorked, totalStaffCo2) = CalculateStaffFootprint(checkInTime, checkOutTime, department);

        return await _staffGateway.CreateStaffFootprintAsync(
            staffId,
            checkOutTime,
            roundedHoursWorked,
            totalStaffCo2);
    }

    public async Task<Stafffootprint?> UpdateStaffFootprintAsync(int staffCarbonFootprintId, int staffId, DateTime checkInTime, DateTime checkOutTime)
    {
        if (staffCarbonFootprintId <= 0)
            throw new ArgumentOutOfRangeException(nameof(staffCarbonFootprintId), "staffCarbonFootprintId must be a positive integer.");

        ValidateStaffInputs(staffId, checkInTime, checkOutTime);

        if (!await _staffGateway.StaffExistsAsync(staffId))
            throw new ArgumentException("staffId does not exist.", nameof(staffId));

        var department = await _staffGateway.GetDepartmentByStaffIdAsync(staffId);
        if (string.IsNullOrWhiteSpace(department))
            throw new InvalidOperationException("Unable to determine department for this staffId.");

        var (roundedHoursWorked, totalStaffCo2) = CalculateStaffFootprint(checkInTime, checkOutTime, department);

        return await _staffGateway.UpdateStaffFootprintAsync(
            staffCarbonFootprintId,
            staffId,
            checkOutTime,
            roundedHoursWorked,
            totalStaffCo2);
    }

    public Task<bool> DeleteStaffFootprintAsync(int staffCarbonFootprintId)
    {
        if (staffCarbonFootprintId <= 0)
            throw new ArgumentOutOfRangeException(nameof(staffCarbonFootprintId), "staffCarbonFootprintId must be a positive integer.");

        return _staffGateway.DeleteStaffFootprintAsync(staffCarbonFootprintId);
    }

    private static void ValidateStaffInputs(int staffId, DateTime checkInTime, DateTime checkOutTime)
    {
        if (staffId <= 0)
            throw new ArgumentOutOfRangeException(nameof(staffId), "staffId must be a positive integer.");

        if (checkOutTime <= checkInTime)
            throw new ArgumentException("checkOutTime must be later than checkInTime.");
    }

    private (double roundedHoursWorked, double totalStaffCo2) CalculateStaffFootprint(
        DateTime checkInTime,
        DateTime checkOutTime,
        string department)
    {
        var hoursWorked = (checkOutTime - checkInTime).TotalHours;
        if (hoursWorked <= 0)
            throw new ArgumentException("hoursWorked must be a positive number.");

        var roundedHoursWorked = Math.Round(hoursWorked, 2);
        var totalStaffCo2 = _staffFootprintStrategy.CalculateFootprint(
            roundedHoursWorked,
            _staffFootprintStrategy.GetDepartmentWeight(department));

        return (roundedHoursWorked, totalStaffCo2);
    }
}
