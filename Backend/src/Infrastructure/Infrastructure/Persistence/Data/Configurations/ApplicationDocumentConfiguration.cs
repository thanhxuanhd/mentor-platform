using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Data.Configurations
{
    public class ApplicationDocumentConfiguration : IEntityTypeConfiguration<ApplicationDocument>
    {
        public void Configure(EntityTypeBuilder<ApplicationDocument> builder)
        {
            builder.HasKey(ad => ad.Id);

            builder.Property(ad => ad.DocumentUrl)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(ad => ad.DocumentType)
                .IsRequired()
                .HasConversion<string>()
                .HasDefaultValue(FileType.Pdf);

            builder.HasOne(ad => ad.MentorApplication)
                    .WithMany(ma => ma.ApplicationDocuments)
                    .HasForeignKey(ad => ad.MentorApplicationId)
                    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
