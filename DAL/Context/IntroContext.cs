using Microsoft.EntityFrameworkCore;

namespace Intro.DAL.Context
{
    public class IntroContext : DbContext
    {
        public DbSet<Entities.User> Users { get; set; }
        public DbSet<Entities.Topic> Topics { get; set; }
        public DbSet<Entities.Article> Articles { get; set; }
        public IntroContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Этот метод вызывается, когда создается модель. Это на случай,
            // когда мы создаем БД из кода (Code First). Можно задать начальные настройки.
            modelBuilder.ApplyConfiguration(new UsersConfiguration());
        }
    }
}
