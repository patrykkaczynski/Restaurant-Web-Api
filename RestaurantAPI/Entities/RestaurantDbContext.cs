using Microsoft.EntityFrameworkCore;

namespace RestaurantAPI.Entities
{
    public class RestaurantDbContext : DbContext
    {
        public RestaurantDbContext(DbContextOptions options) : base(options)
        {
        }

        //private string _connectionString = @"Server=localhost\MSSQLSERVER02;Database=RestaurantDb;Trusted_Connection=True;TrustServerCertificate=True";

        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        //konfiguracja encji
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
               .Property(r => r.Email)
               .IsRequired();

            modelBuilder.Entity<Role>()
               .Property(r => r.Name)
               .IsRequired();

            modelBuilder.Entity<Restaurant>()
                .Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(25);

            modelBuilder.Entity<Dish>()
                .Property(d => d.Name)
                .IsRequired();

            modelBuilder.Entity<Address>()
                .Property(a => a.City)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Address>()
                .Property(a => a.Street)
                .IsRequired()
                .HasMaxLength(50);

            //modelBuilder.Entity<Restaurant>()
            //    .HasData();
        }
        //konfiuguracja połączenia do bazy danych

        //Plik z konkretną migracją o nazwie która została sprecyzowana w komendzie Add-Migration
        //Zawiera niezbędne informacje, które musi wykonać EF po to aby utworzyć konkretną bazę danych, bądź też wprowadzić odpowiednie zmiany w bazie danych
        //Posiada dwie metody Up i Down (metoda Up służy do aplikowania migracji a metoda Down do jej cofnięcia)

        //Plik ModelSnapshot jest to klasa która zapisuje aktualny stan bazy danych po to aby następne migracje mogły zostać wygenerowane na podstawie porównania tej klasy a klasy DbContext
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(_connectionString);
        //}
    }
}
