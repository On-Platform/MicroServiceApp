using MicroServiceApp.UserService.Models;
using Microsoft.EntityFrameworkCore;

namespace MicroServiceApp.UserService;

public class UserDbContext: DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
}