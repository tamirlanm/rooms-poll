using Microsoft.EntityFrameworkCore;
using Services.Api.Models;

namespace Services.Api.Data;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}

    public DbSet<Room> Rooms => Set<Room>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Room>(entity =>
        {
           entity.HasKey(room => room.Id);

           entity.Property(room => room.Name).IsRequired().HasMaxLength(100);

           entity.Property(room => room.CreatedAt).IsRequired();

           entity.Property(room => room.MemberCount).IsRequired(); 
        });
    }
}