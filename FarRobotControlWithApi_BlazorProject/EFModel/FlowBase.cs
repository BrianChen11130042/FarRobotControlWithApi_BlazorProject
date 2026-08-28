using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarRobotControlWithApi_BlazorProject.EFModel
{
    public abstract class FlowBase
    {
        [Key]
        public Guid Id { get; set; }

        public Guid MissionId { get; set; }

        [MaxLength(500)]
        public string? AmrSerialNumber { get; set; }

        [MaxLength(500)]
        public string? FlowId { get; set; }

        [MaxLength(500)]
        public string? TaskId { get; set; }

        public int Priority { get; set; }

        public int State { get; set; }

        [MaxLength(500)]
        public string? StateString { get; set; }

        public double CompletePercent { get; set; }

        public DateTime EstablishTime { get; set; }

        [NotMapped]
        public bool IsStart => StartTime is not null;

        public DateTime? StartTime { get; set; }

        [NotMapped]
        public bool IsError => string.Equals(StateString, "FAILED", StringComparison.OrdinalIgnoreCase);

        public string? StatusCode { get; set; }

        public string? StatusMessage { get; set; }

        [NotMapped]
        public bool IsFinish => FinishTime is not null;

        public DateTime? FinishTime { get; set; }

        [NotMapped]
        public bool IsCancel => CancelTime is not null;

        public DateTime? CancelTime { get; set; }

        public virtual AmrMissionTable? Mission{ get; set; }
    }
}
