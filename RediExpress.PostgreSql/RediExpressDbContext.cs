using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RediExpress.Core.Model.Auth;
using RediExpress.PostgreSql.Configuration;
using RediExpress.PostgreSql.Model;

namespace RediExpress.PostgreSql;

public class RediExpressDbContext(DbContextOptions<RediExpressDbContext> options) 
    : DbContext(options)
{
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<OrderEntity> Orders { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }
    private ILoggerFactory CreateLoggerFactory() => LoggerFactory.Create(builder => {
        builder.AddConsole();
        builder.AddDebug();
    });
}