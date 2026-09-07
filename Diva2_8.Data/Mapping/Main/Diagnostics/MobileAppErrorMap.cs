using Diva2.Core.Main.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Diva2.Data.Mapping.Main.Diagnostics;

internal sealed class MobileAppErrorMap : IEntityTypeConfiguration<MobileAppError>
{
    public void Configure(EntityTypeBuilder<MobileAppError> builder)
    {
        builder.ToTable("spin_mobile_app_error");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Platform).HasMaxLength(32).IsRequired();
        builder.Property(x => x.OsVersion).HasMaxLength(64);
        builder.Property(x => x.DeviceManufacturer).HasMaxLength(100);
        builder.Property(x => x.DeviceModel).HasMaxLength(100);
        builder.Property(x => x.AppVersion).HasMaxLength(32);
        builder.Property(x => x.AppBuild).HasMaxLength(32);
        builder.Property(x => x.Page).HasMaxLength(200);
        builder.Property(x => x.Action).HasMaxLength(200);
        builder.Property(x => x.ExceptionType).HasMaxLength(500);
        builder.Property(x => x.Message).HasMaxLength(4000).IsRequired();
        builder.Property(x => x.StackTrace).HasColumnType("longtext");
        builder.Property(x => x.InnerException).HasColumnType("longtext");
        builder.HasIndex(x => x.ReceivedAt);
        builder.HasIndex(x => new { x.UserId, x.ReceivedAt });
        builder.HasIndex(x => new { x.LessonId, x.ReceivedAt });
    }
}
