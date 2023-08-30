using Microsoft.EntityFrameworkCore;
using SampleApp.UserService.Models;

namespace SampleApp.UserService;

public class UserDbContext: DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
}