using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RediExpress.PostgreSql.Model;

namespace RediExpress.PostgreSql.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.ToTable("Users");
        
        builder.HasKey(x => x.Id);
        
        builder.ComplexProperty(s => s.FullName, b =>
        {
            b.IsRequired();
            b.Property(x => x.FirstName).HasColumnName("FirstName").IsRequired().HasMaxLength(18);
            b.Property(x => x.LastName).HasColumnName("LastName").IsRequired().HasMaxLength(18);
            b.Property(x => x.MiddleName).HasColumnName("MiddleName").IsRequired(false).HasMaxLength(18);
        });
        
        builder.ComplexProperty(s => s.Email, b =>
        {
            b.IsRequired();
            b.Property(x => x.EmailAddress).HasColumnName("EmailAddress");
        });
        
        builder.ComplexProperty(s => s.PhoneNumber, b =>
        {
            b.IsRequired();
            b.Property(x => x.Number).HasColumnName("PhoneNumber");
        });
        
        builder.Property(s => s.PasswordHash).HasColumnName("Password")
            .IsRequired();
    }
}