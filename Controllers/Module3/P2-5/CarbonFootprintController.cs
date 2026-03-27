using Microsoft.AspNetCore.Mvc;
using ProRental.Domain.Entities;
using ProRental.Interfaces.Module3.P2_5;

namespace ProRental.Controllers.Module3.P2_5;

public class CarbonFootprintController : Controller
{
    private readonly IBuildingFootprintControl _buildingControl;
    private readonly IStaffFootprintControl _staffControl;
    private readonly IPackagingProfilerControl _packagingControl;

    public CarbonFootprintController(IBuildingFootprintControl buildingControl, IStaffFootprintControl staffControl, IPackagingProfilerControl packagingControl)
    {
        _buildingControl = buildingControl;
        _staffControl = staffControl;
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

    [HttpPost]
    public IActionResult CreatePackagingProfile([FromBody] CreateProfileRequest req)
    {
        try
        {
            var profile = _packagingControl.CreatePackagingProfile(req.OrderId, req.Volume, req.FragilityLevel);
            if (profile.Packagingconfigurations != null && profile.Packagingconfigurations.Any())
            {
                return BadRequest(new { message = $"A packaging configuration already exists." });
            }

            _packagingControl.CreatePackagingConfiguration(profile);
            
            return Json(new { success = true, message = "Packaging profile and configuration successfully generated!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
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
        try
        {
            var created = await _buildingControl.CreateBuildingFootprintAsync(
                request.RoomSize,
                request.Co2Level,
                request.Zone,
                request.Block,
                request.Floor,
                request.Room);

            return CreatedAtAction(nameof(CalculateBuildingFootprint), created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = $"An error occurred: {ex.Message}" });
        }
    }

    [HttpGet]
    [Route("api/building-footprint")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBuildingFootprints()
    {
        var footprints = await _buildingControl.GetBuildingFootprintsAsync();

        var response = footprints.Select(item => new
        {
            buildingCarbonFootprintId = item.BuildingCarbonFootprintId,
            timehourly = item.Timehourly,
            zone = item.Zone,
            block = item.Block,
            floor = item.Floor,
            room = item.Room,
            totalRoomCo2 = item.TotalRoomCo2
        });

        return Ok(response);
    }

    [HttpPut]
    [Route("api/building-footprint/{buildingCarbonFootprintId:int}")]
    [ProducesResponseType(typeof(Buildingfootprint), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateBuildingFootprint(int buildingCarbonFootprintId, [FromBody] BuildingFootprintRequest request)
    {
        try
        {
            var updated = await _buildingControl.UpdateBuildingFootprintAsync(
                buildingCarbonFootprintId,
                request.RoomSize,
                request.Co2Level,
                request.Zone,
                request.Block,
                request.Floor,
                request.Room);

            if (updated == null)
            {
                return NotFound(new { error = "Building footprint record not found." });
            }

            return Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = $"An error occurred: {ex.Message}" });
        }
    }

    [HttpDelete]
    [Route("api/building-footprint/{buildingCarbonFootprintId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteBuildingFootprint(int buildingCarbonFootprintId)
    {
        try
        {
            var wasDeleted = await _buildingControl.DeleteBuildingFootprintAsync(buildingCarbonFootprintId);
            if (!wasDeleted)
            {
                return NotFound(new { error = "Building footprint record not found." });
            }

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
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
        try
        {
            var created = await _staffControl.CreateStaffFootprintAsync(
                request.StaffId,
                request.CheckInTime,
                request.CheckOutTime);

            return CreatedAtAction(nameof(CalculateStaffFootprint), created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = $"An error occurred: {ex.Message}" });
        }
    }

    [HttpGet]
    [Route("api/staff-footprint")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStaffFootprints()
    {
        var footprints = await _staffControl.GetStaffFootprintsAsync();
        var response = new List<object>(footprints.Count);

        foreach (var item in footprints)
        {
            var department = await _staffControl.GetDepartmentByStaffIdAsync(item.StaffId) ?? "Unknown";
            var checkOutTime = item.Time;
            var checkInTime = checkOutTime.AddHours(-item.HoursWorked);

            response.Add(new
            {
                staffCarbonFootprintId = item.StaffCarbonFootprintId,
                staffId = item.StaffId,
                department,
                checkInTime,
                checkOutTime,
                hoursWorked = Math.Round(item.HoursWorked, 2),
                totalStaffCo2 = Math.Round(item.TotalStaffCo2, 2)
            });
        }

        return Ok(response);
    }

    [HttpPut]
    [Route("api/staff-footprint/{staffCarbonFootprintId:int}")]
    [ProducesResponseType(typeof(Stafffootprint), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStaffFootprint(int staffCarbonFootprintId, [FromBody] StaffFootprintRequest request)
    {
        try
        {
            var updated = await _staffControl.UpdateStaffFootprintAsync(
                staffCarbonFootprintId,
                request.StaffId,
                request.CheckInTime,
                request.CheckOutTime);

            if (updated == null)
            {
                return NotFound(new { error = "Staff footprint record not found." });
            }

            return Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = $"An error occurred: {ex.Message}" });
        }
    }

    [HttpDelete]
    [Route("api/staff-footprint/{staffCarbonFootprintId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteStaffFootprint(int staffCarbonFootprintId)
    {
        try
        {
            var wasDeleted = await _staffControl.DeleteStaffFootprintAsync(staffCarbonFootprintId);
            if (!wasDeleted)
            {
                return NotFound(new { error = "Staff footprint record not found." });
            }

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
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
        var staffItems = await _staffControl.GetStaffLookupAsync();

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

public class CreateProfileRequest
{
    public int OrderId { get; set; }
    public float Volume { get; set; }
    public string FragilityLevel { get; set; } = "low";
}









