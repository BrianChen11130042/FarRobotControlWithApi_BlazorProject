using Microsoft.EntityFrameworkCore;

namespace FarRobotControlWithApi_BlazorProject.EFModel
{
    public class SwarmCoreDbContext : DbContext
    {
        public SwarmCoreDbContext(DbContextOptions<SwarmCoreDbContext> options) :base(options)
        {

        }

        public virtual DbSet<AmrMissionTable> AmrMissionTables { get; set; }

        public virtual DbSet<FlowBase> FlowBases { get; set; }

        public virtual DbSet<MoveFlowTable> MoveFlowTables { get; set; }

        public virtual DbSet<LogTable> LogTables { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AmrMissionTable>(entity => 
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.AmrSerialNumber);
                entity.Property(e => e.AmrSerialNumber).HasMaxLength(500);
            });

            modelBuilder.Entity<FlowBase>(entity => 
            {
                entity.UseTpcMappingStrategy();

                entity.HasKey(e => e.Id);

                entity.Property(e => e.AmrSerialNumber).HasMaxLength(500);

                entity.HasIndex(e => e.FlowId);
                entity.Property(e => e.FlowId).HasMaxLength(500);

                entity.Property(e => e.StateString).HasMaxLength(500);

                entity.HasIndex(e => e.MissionId);

                entity.HasOne(x => x.Mission).WithMany(x => x.Flows).HasForeignKey(x => x.MissionId);
            });

            modelBuilder.Entity<MoveFlowTable>(entity =>
            {
                entity.Property(e => e.CellName).HasMaxLength(500);
            });

            modelBuilder.Entity<LogTable>(entity => 
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).UseIdentityColumn();

                entity.Property(e => e.Equipment).HasMaxLength(500);

                entity.Property(e => e.LogType).HasMaxLength(500);

                entity.Property(e => e.RecordTime).HasColumnType("datetime2");
            });
        }
    }
}
