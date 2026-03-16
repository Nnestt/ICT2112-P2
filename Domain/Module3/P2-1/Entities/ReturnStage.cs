using ProRental.Domain.Enums;

namespace ProRental.Domain.Entities;

public partial class ReturnStage
{
    public CarbonStageType? StageType { get; set; }
    public StageType? StageTypeAlt { get; set; }
}
