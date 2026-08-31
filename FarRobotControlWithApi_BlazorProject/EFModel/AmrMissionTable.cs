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

        public int FlowCount { get; set; }

        public DateTime EstablishTime { get; set; }

        [NotMapped]
        public bool IsStart => StartTime is not null;

        public DateTime? StartTime { get; set; }

        [MaxLength(500)]
        public string? MissionState { get; set; }

        [NotMapped]
        public bool IsError
        {
            get
            {
                
                if (Flows.Any(f => f.IsError && !f.IsCancel))
                    return true;
                
                if (IsCancel && Flows.Any(f => f.IsError))
                    return true;

                return false;
            }
        }

        [NotMapped]
        public bool IsFinish => FinishTime is not null;

        public DateTime? FinishTime { get; set; }

        [NotMapped]
        public bool IsCancel => CancelTime is not null;

        public DateTime? CancelTime { get; set; }

        public virtual ICollection<FlowBase> Flows { get; set; } = new List<FlowBase>();

    }
}
