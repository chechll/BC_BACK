namespace BC_BACK.Data;

public partial class BcDbContext : DbContext
{
    public BcDbContext()
    {
    }

    public BcDbContext(DbContextOptions<BcDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AnsweredTask> AnsweredTasks { get; set; }

    public virtual DbSet<Board> Boards { get; set; }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<Models.Task> Tasks { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=localhost\\MSSQLSERVER01;Database=bc_db;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AnsweredTask>(entity =>
        {
            entity.ToTable("Answered_Tasks");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IdTask).HasColumnName("id_task");
            entity.Property(e => e.IdTeam).HasColumnName("id_team");

            entity.HasOne(d => d.IdTaskNavigation).WithMany(p => p.AnsweredTasks)
                .HasForeignKey(d => d.IdTask)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("R_4");

            entity.HasOne(d => d.IdTeamNavigation).WithMany(p => p.AnsweredTasks)
                .HasForeignKey(d => d.IdTeam)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("R_5");
        });

        modelBuilder.Entity<Board>(entity =>
        {
            entity.HasKey(e => e.IdBoard).HasName("XPKBoard");

            entity.ToTable("Board");

            entity.Property(e => e.IdBoard).HasColumnName("id_board");
            entity.Property(e => e.Board1)
                .HasMaxLength(625)
                .IsUnicode(false)
                .HasColumnName("board");
            entity.Property(e => e.IdGame).HasColumnName("id_game");
            entity.Property(e => e.Size).HasColumnName("size");

            entity.HasOne(d => d.IdGameNavigation).WithMany(p => p.Boards)
                .HasForeignKey(d => d.IdGame)
                .HasConstraintName("R_8");
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.IdGame).HasName("XPKGame");

            entity.ToTable("Game");

            entity.Property(e => e.IdGame).HasColumnName("id_game");
            entity.Property(e => e.DateGame)
                .HasColumnType("date")
                .HasColumnName("date_game");
            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("name");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Games)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("R_1");
        });

        modelBuilder.Entity<Models.Task>(entity =>
        {
            entity.HasKey(e => e.IdTask).HasName("XPKTasks");

            entity.Property(e => e.IdTask).HasColumnName("id_task");
            entity.Property(e => e.Answer)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("answer");
            entity.Property(e => e.IdGame).HasColumnName("id_game");
            entity.Property(e => e.Number).HasColumnName("number");
            entity.Property(e => e.Question)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("question");

            entity.HasOne(d => d.IdGameNavigation).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.IdGame)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("R_9");
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.IdTeam).HasName("XPKTeam");

            entity.ToTable("Team");

            entity.Property(e => e.IdTeam).HasColumnName("id_team");
            entity.Property(e => e.Colour)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("colour");
            entity.Property(e => e.IdGame).HasColumnName("id_game");
            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Password).HasColumnName("password");
            entity.Property(e => e.PositionX).HasColumnName("positionX");
            entity.Property(e => e.PositionY).HasColumnName("positionY");
            entity.Property(e => e.Score).HasColumnName("score");

            entity.HasOne(d => d.IdGameNavigation).WithMany(p => p.Teams)
                .HasForeignKey(d => d.IdGame)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("R_10");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("XPKUser_");

            entity.ToTable("User_");

            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.Password).HasColumnName("password");
            entity.Property(e => e.Rights).HasColumnName("rights");
            entity.Property(e => e.Username)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
