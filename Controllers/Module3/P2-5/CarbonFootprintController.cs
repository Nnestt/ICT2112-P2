using Microsoft.AspNetCore.Mvc;
using ProRental.Data.Module3.P2_5.Interfaces;
using ProRental.Domain.Entities;
using ProRental.Interfaces.Module3.P2_5;

namespace ProRental.Controllers.Module3.P2_5;

public class CarbonFootprintController : Controller
{
    private readonly IBuildingFootprintGateway _buildingGateway;
    private readonly IStaffFootprintGateway _staffGateway;
    private readonly IPackagingProfilerControl _packagingControl;

    public CarbonFootprintController(IBuildingFootprintGateway buildingGateway, IStaffFootprintGateway staffGateway, IPackagingProfilerControl packagingControl)
    {
        _buildingGateway = buildingGateway;
        _staffGateway = staffGateway;
        _packagingControl = packagingControl;
    }

    public IActionResult ProductFootprintView()
    {
        return View("~/Views/Module3/P2-5/ProductFootprintView.cshtml");
    }

    public IActionResult StaffFootprintView()
    {
        return View("~/Views/Module3/P2-5/StaffFootprintView.cshtml");
    }

    public IActionResult BuildingFootprintView()
    {
        return View("~/Views/Module3/P2-5/BuildingFootprintView.cshtml");
    }

    public IActionResult PackagingFootprintView()
    {
        var footprints = _packagingControl.GetAllPackagingFootprints();
        return View("~/Views/Module3/P2-5/PackagingFootprintView.cshtml", footprints);
    }

    /// <summary>
    /// Calculate and create a building footprint record.
    /// Formula: totalRoomCo2 = Sr × Cr × Wz × Wf × k (where k = 0.000729)
    /// </summary>
    [HttpPost]
    [Route("api/building-footprint")]
    [ProducesResponseType(typeof(Buildingfootprint), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CalculateBuildingFootprint([FromBody] BuildingFootprintRequest request)
    {
        // Validate roomSize and co2Level are positive
        if (request.RoomSize <= 0)
            return BadRequest(new { error = "roomSize must be a positive number." });

        if (request.Co2Level <= 0)
            return BadRequest(new { error = "co2Level must be a positive number." });

        var zoneWeights = new Dictionary<string, double>
        {
            { "North", 1.00 },
            { "South", 1.25 },
            { "East", 1.10 },
            { "West", 1.15 },
            { "Central", 1.35 }
        };

        var floorWeights = new Dictionary<string, double>
        {
            { "Level 1", 1.00 },
            { "Level 2", 1.20 },
            { "Level 3", 1.45 },
            { "Level 4", 1.60 },
            { "Level 5", 1.75 }
        };

        // Validate zone
        if (string.IsNullOrWhiteSpace(request.Zone) || !zoneWeights.ContainsKey(request.Zone))
            return BadRequest(new { error = "zone must be one of: North, South, East, West, Central." });

        // Validate floor
        if (string.IsNullOrWhiteSpace(request.Floor) || !floorWeights.ContainsKey(request.Floor))
            return BadRequest(new { error = "floor must be one of: Level 1, Level 2, Level 3, Level 4, Level 5." });

        // Validate block and room are not empty
        if (string.IsNullOrWhiteSpace(request.Block))
            return BadRequest(new { error = "block cannot be empty." });

        if (string.IsNullOrWhiteSpace(request.Room))
            return BadRequest(new { error = "room cannot be empty." });

        try
        {
            const double CalibrationConstant = 0.000729;

            // Calculate: Sr × Cr × Wz × Wf × k
            double totalRoomCo2 = request.RoomSize * request.Co2Level * zoneWeights[request.Zone] * floorWeights[request.Floor] * CalibrationConstant;
            totalRoomCo2 = Math.Round(totalRoomCo2, 2);

            // Create the footprint record
            var footprint = new Buildingfootprint
            {
                Timehourly = DateTime.UtcNow,
                Zone = request.Zone,
                Block = request.Block,
                Floor = request.Floor,
                Room = request.Room,
                Totalroomco2 = totalRoomCo2
            };

            // Save to database
            var created = await _buildingGateway.CreateBuildingFootprintAsync(footprint);

            return CreatedAtAction(nameof(CalculateBuildingFootprint), created);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = $"An error occurred: {ex.Message}" });
        }
    }

    /// <summary>
    /// Calculate and create a staff footprint record.
    /// Formula (seed-calibrated): totalStaffCo2 = hoursWorked × emissionRatePerHour
    /// where emissionRatePerHour = 3.53 kg CO2/hour
    /// </summary>
    [HttpPost]
    [Route("api/staff-footprint")]
    [ProducesResponseType(typeof(Stafffootprint), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CalculateStaffFootprint([FromBody] StaffFootprintRequest request)
    {
        if (request.StaffId <= 0)
            return BadRequest(new { error = "staffId must be a positive integer." });

        if (request.CheckOutTime <= request.CheckInTime)
            return BadRequest(new { error = "checkOutTime must be later than checkInTime." });

        if (!await _staffGateway.StaffExistsAsync(request.StaffId))
            return BadRequest(new { error = "staffId does not exist." });

        var department = await _staffGateway.GetDepartmentByStaffIdAsync(request.StaffId);
        if (string.IsNullOrWhiteSpace(department))
            return BadRequest(new { error = "Unable to determine department for this staffId." });

        var hoursWorked = (request.CheckOutTime - request.CheckInTime).TotalHours;
        if (hoursWorked <= 0)
            return BadRequest(new { error = "hoursWorked must be a positive number." });

        // Calibrated from seed data:
        // (14.2/4.0 + 12.8/3.5 + 16.9/5.0) / 3 = 3.529... ≈ 3.53
        const double EmissionRatePerHour = 3.53;
        var departmentWeight = GetDepartmentWeight(department);

        var roundedHoursWorked = Math.Round(hoursWorked, 2);
        var totalStaffCo2 = Math.Round(roundedHoursWorked * EmissionRatePerHour * departmentWeight, 2);

        try
        {
            var created = await _staffGateway.CreateStaffFootprintAsync(
                request.StaffId,
                request.CheckOutTime,
                roundedHoursWorked,
                totalStaffCo2);

            return CreatedAtAction(nameof(CalculateStaffFootprint), created);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = $"An error occurred: {ex.Message}" });
        }
    }

    [HttpGet]
    [Route("api/staff-footprint/staff-options")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStaffOptions()
    {
        var staffItems = await _staffGateway.GetStaffLookupAsync();

        var options = staffItems
            .Select(item => new
            {
                staffId = item.StaffId,
                department = item.Department,
                departmentWeight = GetDepartmentWeight(item.Department)
            })
            .ToList();

        return Ok(options);
    }

    private static double GetDepartmentWeight(string department)
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

/// <summary>
/// Request model for building footprint calculation
/// </summary>
public class BuildingFootprintRequest
{
    public double RoomSize { get; set; }
    public double Co2Level { get; set; }
    public string Zone { get; set; } = string.Empty;
    public string Block { get; set; } = string.Empty;
    public string Floor { get; set; } = string.Empty;
    public string Room { get; set; } = string.Empty;
}

public class StaffFootprintRequest
{
    public int StaffId { get; set; }
    public DateTime CheckInTime { get; set; }
    public DateTime CheckOutTime { get; set; }
}
