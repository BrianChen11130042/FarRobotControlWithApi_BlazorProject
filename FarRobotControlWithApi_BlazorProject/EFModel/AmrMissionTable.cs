using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarRobotControlWithApi_BlazorProject.EFModel
{
    public class AmrMissionTable
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string AmrSerialNumber { get; set; } = null!;

        public int Priority { get; set; }

        public DateTime EstablishTime { get; set; }

        [NotMapped]
        public bool IsStart => StartTime is not null;

        public DateTime? StartTime { get; set; }

        [NotMapped]
        public bool IsError => ErrorCode is not 0 || !string.IsNullOrEmpty(ErrorMessage);

        public int ErrorCode { get; set; }

        public string? ErrorMessage { get; set; }

        [NotMapped]
        public bool IsFinish => FinishTime is not null;

        public DateTime? FinishTime { get; set; }

        public bool IsCancel { get; set; }

        public virtual ICollection<FlowBase> Flows { get; set; } = new List<FlowBase>();

    }
}
