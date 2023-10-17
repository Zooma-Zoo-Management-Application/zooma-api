using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace zooma_api.Models
{
    public partial class zoomadbContext : DbContext
    {
        public zoomadbContext()
        {
        }

        public zoomadbContext(DbContextOptions<zoomadbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Animal> Animals { get; set; } = null!;
        public virtual DbSet<AnimalUser> AnimalUsers { get; set; } = null!;
        public virtual DbSet<Area> Areas { get; set; } = null!;
        public virtual DbSet<Cage> Cages { get; set; } = null!;
        public virtual DbSet<Configuration> Configurations { get; set; } = null!;
        public virtual DbSet<Diet> Diets { get; set; } = null!;
        public virtual DbSet<DietDetail> DietDetails { get; set; } = null!;
        public virtual DbSet<Food> Foods { get; set; } = null!;
        public virtual DbSet<FoodSpecy> FoodSpecies { get; set; } = null!;
        public virtual DbSet<News> News { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Skill> Skills { get; set; } = null!;
        public virtual DbSet<Species> Species { get; set; } = null!;
        public virtual DbSet<Ticket> Tickets { get; set; } = null!;
        public virtual DbSet<TrainerExp> TrainerExps { get; set; } = null!;
        public virtual DbSet<TrainingDetail> TrainingDetails { get; set; } = null!;
        public virtual DbSet<TrainingPlan> TrainingPlans { get; set; } = null!;
        public virtual DbSet<Transaction> Transactions { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=tcp:zooma-db.database.windows.net,1433;Initial Catalog=zooma-db;Persist Security Info=False;User ID=zooma-db;Password=huydiet@SWP;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Animal>(entity =>
            {
                entity.ToTable("Animal");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ArrivalDate).HasColumnType("date");

                entity.Property(e => e.CageId).HasColumnName("CageID");

                entity.Property(e => e.DateOfBirth).HasColumnType("date");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.DietId).HasColumnName("DietID");

                entity.Property(e => e.ImageUrl).HasMaxLength(255);

                entity.Property(e => e.MaxRer).HasColumnName("MaxRER");

                entity.Property(e => e.MinRer).HasColumnName("MinRER");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.SpieciesId).HasColumnName("SpieciesID");

                entity.Property(e => e.TrainingPlanId).HasColumnName("TrainingPlanID");

                entity.HasOne(d => d.Cage)
                    .WithMany(p => p.Animals)
                    .HasForeignKey(d => d.CageId)
                    .HasConstraintName("FKAnimal252210");

                entity.HasOne(d => d.Diet)
                    .WithMany(p => p.Animals)
                    .HasForeignKey(d => d.DietId)
                    .HasConstraintName("FKAnimal242590");

                entity.HasOne(d => d.Spiecies)
                    .WithMany(p => p.Animals)
                    .HasForeignKey(d => d.SpieciesId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKAnimal821690");

                entity.HasOne(d => d.TrainingPlan)
                    .WithMany(p => p.Animals)
                    .HasForeignKey(d => d.TrainingPlanId)
                    .HasConstraintName("FKAnimal451235");
            });

            modelBuilder.Entity<AnimalUser>(entity =>
            {
                entity.HasKey(e => new { e.AnimalId, e.UserId })
                    .HasName("PK__Animal_U__7362FFED604B3315");

                entity.ToTable("Animal_User");

                entity.Property(e => e.AnimalId).HasColumnName("AnimalID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Animal)
                    .WithMany(p => p.AnimalUsers)
                    .HasForeignKey(d => d.AnimalId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKAnimal_Use737608");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AnimalUsers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKAnimal_Use562803");
            });

            modelBuilder.Entity<Area>(entity =>
            {
                entity.ToTable("Area");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Status).HasColumnName("status");
            });

            modelBuilder.Entity<Cage>(entity =>
            {
                entity.ToTable("Cage");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AreaId).HasColumnName("AreaID");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.HasOne(d => d.Area)
                    .WithMany(p => p.Cages)
                    .HasForeignKey(d => d.AreaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKCage426657");
            });

            modelBuilder.Entity<Configuration>(entity =>
            {
                entity.ToTable("Configuration");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ConfigDataType).HasMaxLength(255);

                entity.Property(e => e.DataValue).HasMaxLength(255);

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.Name).HasMaxLength(75);
            });

            modelBuilder.Entity<Diet>(entity =>
            {
                entity.ToTable("Diet");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.Property(e => e.EndAt).HasColumnType("datetime");

                entity.Property(e => e.Goal).HasMaxLength(255);

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.ScheduleAt).HasColumnType("datetime");

                entity.Property(e => e.UpdateAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<DietDetail>(entity =>
            {
                entity.ToTable("DietDetail");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.DietId).HasColumnName("DietID");

                entity.Property(e => e.EndAt).HasColumnType("datetime");

                entity.Property(e => e.FoodId).HasColumnName("FoodID");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.ScheduleAt).HasColumnType("datetime");

                entity.Property(e => e.UpdateAt).HasColumnType("datetime");

                entity.HasOne(d => d.Diet)
                    .WithMany(p => p.DietDetails)
                    .HasForeignKey(d => d.DietId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKDietDetail129264");

                entity.HasOne(d => d.Food)
                    .WithMany(p => p.DietDetails)
                    .HasForeignKey(d => d.FoodId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKDietDetail981596");
            });

            modelBuilder.Entity<Food>(entity =>
            {
                entity.ToTable("Food");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.Image).HasMaxLength(50);

                entity.Property(e => e.ImageUrl).HasMaxLength(255);

                entity.Property(e => e.Name).HasMaxLength(100);
            });

            modelBuilder.Entity<FoodSpecy>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.SpeciesId, e.FoodId })
                    .HasName("PK__Food_Spe__120201D39B673BD6");

                entity.ToTable("Food_Species");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.SpeciesId).HasColumnName("SpeciesID");

                entity.Property(e => e.FoodId).HasColumnName("FoodID");

                entity.HasOne(d => d.Food)
                    .WithMany(p => p.FoodSpecies)
                    .HasForeignKey(d => d.FoodId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKFood_Speci337357");

                entity.HasOne(d => d.Species)
                    .WithMany(p => p.FoodSpecies)
                    .HasForeignKey(d => d.SpeciesId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKFood_Speci296025");
            });

            modelBuilder.Entity<News>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.Image).HasMaxLength(255);

                entity.Property(e => e.Title).HasMaxLength(255);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.News)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKNews92628");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.LastUpdateDate).HasColumnType("datetime");

                entity.Property(e => e.Notes).HasColumnType("text");

                entity.Property(e => e.OrderDate).HasColumnType("date");

                entity.Property(e => e.PaymentMethod).HasMaxLength(50);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKOrder63439");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.ToTable("OrderDetail");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.TicketDate).HasColumnType("date");

                entity.Property(e => e.TicketId).HasColumnName("TicketID");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKOrderDetai762136");

                entity.HasOne(d => d.Ticket)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.TicketId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKOrderDetai841054");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.HasIndex(e => e.Name, "UQ__Role__737584F655DF86C7")
                    .IsUnique();

                entity.HasIndex(e => e.Name, "UQ__Role__737584F6A6EA5EAC")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<Skill>(entity =>
            {
                entity.ToTable("Skill");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.HasMany(d => d.TrainingDetails)
                    .WithMany(p => p.Skills)
                    .UsingEntity<Dictionary<string, object>>(
                        "SkillTrainingDetail",
                        l => l.HasOne<TrainingDetail>().WithMany().HasForeignKey("TrainingDetailId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FKSkill_Trai327970"),
                        r => r.HasOne<Skill>().WithMany().HasForeignKey("SkillId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FKSkill_Trai990760"),
                        j =>
                        {
                            j.HasKey("SkillId", "TrainingDetailId").HasName("PK__Skill_Tr__D0837DD63C16853B");

                            j.ToTable("Skill_TrainingDetail");

                            j.IndexerProperty<short>("SkillId").HasColumnName("SkillID");

                            j.IndexerProperty<int>("TrainingDetailId").HasColumnName("TrainingDetailID");
                        });
            });

            modelBuilder.Entity<Species>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.Name).HasMaxLength(100);
            });

            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.ToTable("Ticket");

                entity.HasIndex(e => e.Name, "UQ__Ticket__737584F6C377995B")
                    .IsUnique();

                entity.HasIndex(e => e.Name, "UQ__Ticket__737584F6E85A4188")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TrainerExp>(entity =>
            {
                entity.ToTable("TrainerExp");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.SkillId).HasColumnName("SkillID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Skill)
                    .WithMany(p => p.TrainerExps)
                    .HasForeignKey(d => d.SkillId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKTrainerExp225863");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TrainerExps)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKTrainerExp695829");
            });

            modelBuilder.Entity<TrainingDetail>(entity =>
            {
                entity.ToTable("TrainingDetail");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.TrainingPlanId).HasColumnName("TrainingPlanID");

                entity.HasOne(d => d.TrainingPlan)
                    .WithMany(p => p.TrainingDetails)
                    .HasForeignKey(d => d.TrainingPlanId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKTrainingDe45542");
            });

            modelBuilder.Entity<TrainingPlan>(entity =>
            {
                entity.ToTable("TrainingPlan");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.TrainingGoal).HasColumnType("text");
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.ToTable("Transaction");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AccountNumber).HasMaxLength(50);

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.PaymentType).HasMaxLength(50);

                entity.Property(e => e.TransactionNo).HasMaxLength(50);

                entity.Property(e => e.TransactionToken).HasMaxLength(255);

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKTransactio146432");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.HasIndex(e => e.UserName, "UQ__User__C9F284566AD80713")
                    .IsUnique();

                entity.HasIndex(e => e.UserName, "UQ__User__C9F28456F4319729")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AvatarUrl).HasMaxLength(255);

                entity.Property(e => e.DateOfBirth).HasColumnType("date");

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.FullName).HasMaxLength(150);

                entity.Property(e => e.Gender).HasMaxLength(20);

                entity.Property(e => e.Password).HasMaxLength(50);

                entity.Property(e => e.PhoneNumber).HasMaxLength(20);

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.UserName).HasMaxLength(50);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKUser349791");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
