using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VerticalSliceArchictureDemo.Web.Domain.Entities;

namespace VerticalSliceArchictureDemo.Web.Persistence.Configurations
{
    public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Username)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(p => p.CreatedOn)
                .IsRequired()
                .HasConversion(p => p, p => DateTime.SpecifyKind(p, DateTimeKind.Utc));

            builder.Property(p => p.LastModifiedOn)
                .IsRequired()
                .HasConversion(p => p, p => DateTime.SpecifyKind(p, DateTimeKind.Utc));
        }
    }
}
