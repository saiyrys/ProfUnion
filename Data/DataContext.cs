namespace Profunion.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<EventCategories> EventCategories { get; set; }
        public DbSet<Application> Application { get; set; }
        public DbSet<RejectedApplication> RejectedApplication { get; set; }
        public DbSet<ReservationList> ReservationList { get; set; }
        public DbSet<Comments> Comment { get; set; }
        public DbSet<Uploads> Uploads { get; set; }
        public DbSet<EventUploads> EventUploads { get; set; }
        public DbSet<NewsUploads> NewsUploads { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => new { u.userId });
                entity.HasIndex(u => new { u.userName }).IsUnique();
                entity.HasIndex(u => new { u.email }).IsUnique();
                entity.HasIndex(u => new { u.password }).IsUnique();
            });

            modelBuilder.Entity<Categories>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Categories>()
                .HasMany(c => c.EventCategories)
                .WithOne(ec => ec.Categories)
                .HasForeignKey(ec => ec.CategoriesId);

            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.eventId);
                entity.HasIndex(e => e.title).IsUnique();
                entity.Ignore(e => e.Categories);
                entity.Ignore(e => e.Uploads);
            });
                
            modelBuilder.Entity<Event>()
                .HasMany(e => e.EventCategories)
                .WithOne()
                .HasForeignKey(ec => ec.eventId)
                .IsRequired();

            modelBuilder.Entity<Event>()
               .HasMany(e => e.EventUploads)
               .WithOne()
               .HasForeignKey(ec => ec.eventId)
               .IsRequired();


            modelBuilder.Entity<EventCategories>()
                .HasKey(ec => new { ec.CategoriesId, ec.eventId });

            modelBuilder.Entity<EventCategories>()
                .HasOne(ec => ec.Event)  
                .WithMany(e => e.EventCategories) 
                .HasForeignKey(c => c.eventId);

            modelBuilder.Entity<EventCategories>()
                .HasOne(ec => ec.Categories)
                .WithMany(e => e.EventCategories)
                .HasForeignKey(c => c.CategoriesId);

            modelBuilder.Entity<News>(entity =>
            {
                entity.HasKey(n => new { n.newsId });
                entity.HasIndex(n => n.title).IsUnique();
                entity.Ignore(n => n.Uploads);
            });

            

            modelBuilder.Entity<Application>(entity =>
            {
                entity.HasKey(e => new { e.Id });
           
                entity.HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId);

                entity.HasOne(a => a.Event)
                .WithMany()
                .HasForeignKey(a => a.EventId)
                .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<RejectedApplication>(entity =>
            {
                entity.HasKey(e => new { e.Id });

                entity.HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId);

                entity.HasOne(b => b.Event)
                .WithMany()
                .HasForeignKey(b => b.EventId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ReservationList>(entity =>
            {
                entity.HasKey(e => new { e.Id });

                entity.HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId);

                entity.HasOne(b => b.Event)
                .WithMany()
                .HasForeignKey(b => b.EventId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Comments>(entity =>
            {
                entity.HasKey(com => new { com.Id });

                entity.HasOne(com => com.User)
                .WithMany()
                .HasForeignKey(com => com.userId);

            });

            modelBuilder.Entity<Uploads>()
                .HasKey(u => new { u.fileName });

            modelBuilder.Entity<EventUploads>()
                .HasKey(eu => new { eu.eventId, eu.fileId });

            modelBuilder.Entity<EventUploads>()
               .HasOne(eu => eu.Event)
               .WithMany(e => e.EventUploads)
               .HasForeignKey(eu => eu.eventId)
                .IsRequired();

            modelBuilder.Entity<EventUploads>()
                .HasOne(eu => eu.Uploads)
                .WithMany(u => u.EventUploads)
                .HasForeignKey(eu => eu.fileId)
                .IsRequired();

            modelBuilder.Entity<NewsUploads>()
                .HasKey(eu => new { eu.newsId, eu.fileId });

            modelBuilder.Entity<NewsUploads>()
               .HasOne(eu => eu.News)
               .WithMany(e => e.NewsUploads)
               .HasForeignKey(eu => eu.newsId)
                .IsRequired();

            modelBuilder.Entity<NewsUploads>()
                .HasOne(eu => eu.Uploads)
                .WithMany(u => u.NewsUploads)
                .HasForeignKey(eu => eu.fileId)
                .IsRequired();

        }



    }
}
