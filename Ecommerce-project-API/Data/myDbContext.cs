using Ecommerce_project_API.Models;
using Microsoft.EntityFrameworkCore;

public class myDbContext : DbContext
{
    public myDbContext(DbContextOptions<myDbContext> options) : base(options) { }

    public DbSet<UserModel> Users { get; set; }

    public DbSet<Categories> Category { get; set; }

    public DbSet<Products> Products { get; set; }

    public DbSet<Wishlist> Wishlist { get; set; }





}
