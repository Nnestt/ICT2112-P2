using ProRental.Interfaces.Module3;

namespace ProRental.Domain.Entities.Module3;

public class StaffFootprintStrategy : ICalculateCarbonStrategy
{
    private const double EmissionRatePerHour = 3.53;

    public double CalculateFootprint(params double[] values)
    {
        if (values.Length != 2)
        {
            throw new ArgumentException("Staff footprint strategy requires hours worked and department weight.");
        }

        var hoursWorked = values[0];
        var departmentWeight = values[1];

        if (hoursWorked <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(values), "Hours worked must be a positive number.");
        }

        if (departmentWeight <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(values), "Department weight must be a positive number.");
        }

        var roundedHoursWorked = Math.Round(hoursWorked, 2);
        return Math.Round(roundedHoursWorked * EmissionRatePerHour * departmentWeight, 2);
    }

    public double GetDepartmentWeight(string department)
    {
        return department.Trim().ToLowerInvariant() switch
        {
            "customer support" => 1.00,
            "operations" => 1.15,
            "finance" => 1.10,
            "marketing" => 2.00,
            "it" => 1.20,
            _ => 1.00
        };
    }
}
