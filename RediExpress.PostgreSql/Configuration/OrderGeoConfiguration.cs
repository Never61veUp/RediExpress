using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RediExpress.PostgreSql.Model;

namespace RediExpress.PostgreSql.Configuration;

public sealed class OrderGeoConfiguration : IEntityTypeConfiguration<OrderGeoEntity>
{
    public void Configure(EntityTypeBuilder<OrderGeoEntity> builder)
    {
        builder.ToTable("OrderGeos");
        builder.ComplexProperty(o => o.GeoPoint, builder =>
        {
            builder.IsRequired();
            builder.Property(o => o.Latitude).HasColumnName("Latitude");
            builder.Property(o => o.Longitude).HasColumnName("Longitude");
        });
    }
}