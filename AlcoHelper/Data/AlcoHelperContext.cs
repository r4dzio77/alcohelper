using Microsoft.EntityFrameworkCore;
using AlcoHelper.Models;
using AlcoHelper.ViewModels; // Upewnij się, że przestrzeń nazw jest poprawna

namespace AlcoHelper.Data // Możesz zmienić przestrzeń nazw, jeśli wolisz inną
{
    public class AlcoHelperContext : DbContext
    {
        public AlcoHelperContext(DbContextOptions<AlcoHelperContext> options) : base(options)
        {
        }

        public DbSet<Alcohol> Alcohols { get; set; }
        public DbSet<AlcoholStore> AlcoholStores { get; set; }
        public DbSet<AlcoholTag> AlcoholTags { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<EmailNotification> EmailNotifications { get; set; }
        public DbSet<FavoriteAlco> FavoriteAlcos { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<WishList> Wishlists { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Konfiguracja relacji dla AlcoholStore (many-to-many między Alcohol a Store)
            modelBuilder.Entity<AlcoholStore>()
                .HasKey(a => new { a.AlcoholId, a.StoreId }); // Klucz złożony

            modelBuilder.Entity<AlcoholStore>()
                .HasOne((AlcoholStore alcoholStore) => alcoholStore.Alcohol)
                .WithMany(a => a.AlcoholStores)
                .HasForeignKey("AlcoholId");

            modelBuilder.Entity<AlcoholStore>()
                .HasOne((AlcoholStore alcoholStore) => alcoholStore.Store)
                .WithMany(s => s.AlcoholStores)
                .HasForeignKey("StoreId");

            // Konfiguracja relacji dla AlcoholTag (many-to-many między Alcohol a Tag)
            modelBuilder.Entity<AlcoholTag>()
                .HasKey(at => new { at.AlcoholId, at.TagId }); // Klucz złożony

            modelBuilder.Entity<AlcoholTag>()
                .HasOne(at => at.Alcohol)
                .WithMany(a => a.AlcoholTags)
                .HasForeignKey(at => at.AlcoholId);

            modelBuilder.Entity<AlcoholTag>()
                .HasOne(at => at.Tag)
                .WithMany(t => t.AlcoholTags)
                .HasForeignKey(at => at.TagId);

            // Konfiguracja relacji jeden-do-wielu między Alcohol a Review
            modelBuilder.Entity<Alcohol>()
                .HasMany(a => a.Reviews)
                .WithOne(r => r.Alcohol)
                .HasForeignKey(r => r.AlcoholId);

            // Konfiguracja relacji jeden-do-wielu między User a Review
            modelBuilder.Entity<User>()
                .HasMany(u => u.Reviews)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId);

            // Konfiguracja relacji jeden-do-wielu między Alcohol a FavoriteAlco
            modelBuilder.Entity<Alcohol>()
                .HasMany(a => a.Favorites)
                .WithOne(fa => fa.Alcohol)
                .HasForeignKey(fa => fa.AlcoholId);

            // Konfiguracja relacji jeden-do-wielu między User a FavoriteAlco
            modelBuilder.Entity<User>()
                .HasMany(u => u.Favorites)
                .WithOne(fa => fa.User)
                .HasForeignKey(fa => fa.UserId);

            // Konfiguracja relacji jeden-do-wielu między Alcohol a WishList
            modelBuilder.Entity<Alcohol>()
                .HasMany(a => a.Wishlist)
                .WithOne(wl => wl.Alcohol)
                .HasForeignKey(wl => wl.AlcoholId);

            // Konfiguracja relacji jeden-do-wielu między User a WishList
            modelBuilder.Entity<User>()
                .HasMany(u => u.Wishlist)
                .WithOne(wl => wl.User)
                .HasForeignKey(wl => wl.UserId);

            // Konfiguracja relacji wiele-do-jednego między Comment a User
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany() // Użytkownik może mieć wiele komentarzy
                .HasForeignKey(c => c.UserId);

            // Konfiguracja relacji wiele-do-jednego między Comment a Review
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Review)
                .WithMany() // Recenzja może mieć wiele komentarzy
                .HasForeignKey(c => c.ReviewId);

            // Konfiguracja relacji wiele-do-jednego między EmailNotification a User
            modelBuilder.Entity<EmailNotification>()
                .HasOne(en => en.User)
                .WithMany() // Użytkownik może mieć wiele powiadomień
                .HasForeignKey(en => en.UserId);

            // Konfiguracja relacji wiele-do-jednego między Report a Alcohol
            modelBuilder.Entity<Report>()
                .HasOne(r => r.Alcohol)
                .WithMany() // Alkohol może mieć wiele zgłoszeń
                .HasForeignKey(r => r.AlcoholId);

            // Konfiguracja relacji wiele-do-jednego między Report a User
            modelBuilder.Entity<Report>()
                .HasOne(r => r.User)
                .WithMany() // Użytkownik może zgłosić wiele błędów
                .HasForeignKey(r => r.UserId);

            base.OnModelCreating(modelBuilder);
        }
    }
}