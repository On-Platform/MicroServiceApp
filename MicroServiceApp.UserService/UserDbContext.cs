using System.Linq.Expressions;
using System.Security.Claims;
using MicroServiceApp.UserService.Abstractions;
using MicroServiceApp.UserService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace MicroServiceApp.UserService;

public class UserDbContext: DbContext
{
    ClaimsPrincipal? _user;
    IHttpContextAccessor? _httpContextAccessor;
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {
    }

    private Guid? GetUserId()
    {
        using (var serviceScope = this.GetService<IServiceScopeFactory>().CreateScope())
        {
            if (_user is null)
            {
                _httpContextAccessor = this.GetService<IHttpContextAccessor>();
                _user = _httpContextAccessor.HttpContext?.User;
            }
            if (_user.Identity is { IsAuthenticated: false })
            {
                return null;
            }

            var value = _user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (value != null)
                return Guid.Parse(value);
            return null;
        }
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
    {
        UpdateAuditEntities();
        UpdateSoftDeleteEntities();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        UpdateAuditEntities();
        UpdateSoftDeleteEntities();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        UpdateAuditEntities();
        UpdateSoftDeleteEntities();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override int SaveChanges()
    {
        UpdateAuditEntities();
        UpdateSoftDeleteEntities();
        return base.SaveChanges();
    }
    
    private void UpdateAuditEntities()
    {
        var userId = GetUserId();
        var entities = ChangeTracker.Entries().Where(x => x.Entity is IAuditedEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));
        foreach (var entity in entities)
        {
            if (entity.Entity is IAuditedEntity auditEntity)
            {
                if (entity.State == EntityState.Added)
                {
                    auditEntity.CreatedAt = DateTime.UtcNow;
                    auditEntity.CreatedBy = userId;
                }
                else
                {
                    auditEntity.UpdatedAt = DateTime.UtcNow;
                    auditEntity.UpdatedBy = userId;
                }
            }
        }
    }
    
    private void UpdateSoftDeleteEntities()
    {
        var entities = ChangeTracker.Entries().Where(x => x.Entity is ISoftDelete && x.State == EntityState.Deleted);
        foreach (var entity in entities)
        {
            if (entity.Entity is ISoftDelete softDeleteEntity)
            {
                entity.State = EntityState.Modified;
                softDeleteEntity.IsDeleted = true;
            }
        }
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var propertyMethodInfo = typeof(EF).GetMethod("Property")?.MakeGenericMethod(typeof(bool));
                var isDeletedProperty = Expression.Call(propertyMethodInfo!, parameter, Expression.Constant("IsDeleted"));
                BinaryExpression compareExpression = Expression.MakeBinary(ExpressionType.Equal, isDeletedProperty, Expression.Constant(false));
                var lambda = Expression.Lambda(compareExpression, parameter);
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }

    public DbSet<User> Users { get; set; }
}