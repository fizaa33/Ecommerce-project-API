using Ecommerce_project_API.Models;
using Microsoft.EntityFrameworkCore;

public class myDbContext : DbContext
{
    public myDbContext(DbContextOptions<myDbContext> options) : base(options) { }

    public DbSet<UserModel> Users { get; set; }


}
