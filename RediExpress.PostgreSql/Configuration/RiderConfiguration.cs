using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RediExpress.PostgreSql.Model;

namespace RediExpress.PostgreSql.Configuration;

public sealed class RiderConfiguration : IEntityTypeConfiguration<RiderEntity>
{
    public void Configure(EntityTypeBuilder<RiderEntity> builder)
    {
        builder.ToTable("Rider");

        builder.HasKey(k => k.UserId);
        
        builder.HasOne(r => r.User)
            .WithOne()
            .HasForeignKey<RiderEntity>(r => r.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(r => r.Rating);
        builder.Property(r => r.RatingCount);
        
        builder.OwnsMany(r => r.Reviews, reviewBuilder =>
        {
            reviewBuilder.WithOwner().HasForeignKey("RiderId");

            reviewBuilder.Property(r => r.Comment)
                .HasMaxLength(500)
                .IsRequired();

            reviewBuilder.Property(r => r.Rating)
                .IsRequired();

            reviewBuilder.Property(r => r.CreatedAt)
                .IsRequired();

            reviewBuilder.Property<Guid>("AuthorUserId")
                .HasColumnName("AuthorUserId")
                .IsRequired();

            reviewBuilder.ToTable("RiderReviews");
            reviewBuilder.HasKey("RiderId", "CreatedAt");
        });
        
        
    }
}