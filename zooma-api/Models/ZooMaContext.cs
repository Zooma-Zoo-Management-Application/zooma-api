using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace zooma_api.Models
{
    public partial class ZooMaContext : DbContext
    {
        public ZooMaContext()
        {
        }

        public ZooMaContext(DbContextOptions<ZooMaContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Animal> Animals { get; set; }
        public virtual DbSet<AnimalUser> AnimalUsers { get; set; }
        public virtual DbSet<Area> Areas { get; set; }
        public virtual DbSet<Cage> Cages { get; set; }
        public virtual DbSet<Diet> Diets { get; set; }
        public virtual DbSet<DietDetail> DietDetails { get; set; }
        public virtual DbSet<Food> Foods { get; set; }
        public virtual DbSet<FoodSpecy> FoodSpecies { get; set; }
        public virtual DbSet<News> News { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Skill> Skills { get; set; }
        public virtual DbSet<Species> Species { get; set; }
        public virtual DbSet<Ticket> Tickets { get; set; }
        public virtual DbSet<TrainerExp> TrainerExps { get; set; }
        public virtual DbSet<TrainingDetail> TrainingDetails { get; set; }
        public virtual DbSet<TrainingPlan> TrainingPlans { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("server=(local);database=ZooMa;uid=sa;pwd=12345;TrustServerCertificate=True");
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
                    .HasName("PK__Animal_U__7362FFED41C006AA");

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

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Status).HasColumnName("status");
            });

            modelBuilder.Entity<Cage>(entity =>
            {
                entity.ToTable("Cage");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AreaId).HasColumnName("AreaID");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Status).HasColumnName("status");

                entity.HasOne(d => d.Area)
                    .WithMany(p => p.Cages)
                    .HasForeignKey(d => d.AreaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKCage426657");
            });

            modelBuilder.Entity<Diet>(entity =>
            {
                entity.ToTable("Diet");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasColumnName("description");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.ScheduleAt).HasColumnType("datetime");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.UpdateAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<DietDetail>(entity =>
            {
                entity.ToTable("DietDetail");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.DietId).HasColumnName("DietID");

                entity.Property(e => e.FoodId).HasColumnName("FoodID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

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

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<FoodSpecy>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.SpeciesId, e.FoodId })
                    .HasName("PK__Food_Spe__120201D3AF258983");

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

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(255);

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

                entity.Property(e => e.Notes).HasColumnType("text");

                entity.Property(e => e.OrderDate).HasColumnType("date");

                entity.Property(e => e.PaymentMethod)
                    .IsRequired()
                    .HasMaxLength(50);

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

                entity.HasIndex(e => e.Name, "UQ__Role__737584F6AFC831C3")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Skill>(entity =>
            {
                entity.ToTable("Skill");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Species>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.ToTable("Ticket");

                entity.HasIndex(e => e.Name, "UQ__Ticket__737584F6DCC29DA5")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TrainerExp>(entity =>
            {
                entity.ToTable("TrainerExp");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Description).HasMaxLength(255);

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

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.TrainingGoal)
                    .IsRequired()
                    .HasColumnType("text");
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.ToTable("Transaction");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AccountNumber).HasMaxLength(50);

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

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

                entity.HasIndex(e => e.UserName, "UQ__User__C9F284569EFDF936")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AvatarUrl).HasMaxLength(255);

                entity.Property(e => e.DateOfBirth).HasColumnType("date");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.FullName).HasMaxLength(150);

                entity.Property(e => e.Gender).HasMaxLength(20);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.PhoneNumber).HasMaxLength(20);

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(50);

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
