using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RediExpress.Core.Model.ValueObjects;
using RediExpress.PostgreSql.Model;

namespace RediExpress.PostgreSql.Configuration;

public sealed class OrderConfiguration : IEntityTypeConfiguration<OrderEntity>
{
    public void Configure(EntityTypeBuilder<OrderEntity> builder)
    {
        builder.ToTable("Orders");
        
        builder.HasKey(x => x.Id);

        builder.ComplexProperty(p => p.Package, b =>
        {
            b.IsRequired();
            b.Property(x => x.PackageItems).HasColumnName("PackageItems");
            b.Property(x => x.WeightOfItems).HasColumnName("WeightOfItems");
            b.Property(x => x.WorthOfItems).HasColumnName("WorthOfItems");
        });
        
        builder.Property(o => o.Status)
            .HasConversion<string>();
        
        builder.Property(o => o.CreatedTime);
        builder.ComplexProperty(o => o.OriginDetails, od =>
        {
            od.Property(p => p.Address).HasColumnName("OriginAddress");

            od.ComplexProperty(p => p.PhoneNumber, pn =>
            {
                pn.Property(p => p.Number).HasColumnName("OriginPhoneNumber");
                pn.IsRequired();
            });

            od.ComplexProperty(p => p.GeoPoint, gp =>
            {
                gp.Property(p => p.Latitude).HasColumnName("OriginLatitude");
                gp.Property(p => p.Longitude).HasColumnName("OriginLongitude");
            });
        });
        
        builder.ComplexProperty(o => o.DestinationDetails, dd =>
        {
            dd.Property(p => p.Address).HasColumnName("DestinationAddress");

            dd.ComplexProperty(p => p.PhoneNumber, pn =>
            {
                pn.Property(p => p.Number).HasColumnName("DestinationPhoneNumber");
                pn.IsRequired();
            });

            dd.ComplexProperty(p => p.GeoPoint, gp =>
            {
                gp.Property(p => p.Latitude).HasColumnName("DestinationLatitude");
                gp.Property(p => p.Longitude).HasColumnName("DestinationLongitude");
            });
        });
        builder.HasOne(o => o.User)
            .WithMany()
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}