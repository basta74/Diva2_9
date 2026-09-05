using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
namespace Diva2_8.Data.Migrations;

public partial class AddNotifications : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable("spin_user_device", table => new
        {
            Id = table.Column<int>("int", nullable: false).Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
            UserId = table.Column<int>("int", nullable: false),
            PushToken = table.Column<string>("varchar(512)", maxLength: 512, nullable: false),
            Platform = table.Column<string>("varchar(16)", maxLength: 16, nullable: false),
            RegisteredAt = table.Column<DateTime>("datetime(6)", nullable: false),
            LastSeenAt = table.Column<DateTime>("datetime(6)", nullable: false),
            Active = table.Column<bool>("tinyint(1)", nullable: false)
        }, constraints: table =>
        {
            table.PrimaryKey("PK_spin_user_device", x => x.Id);
        }).Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable("spin_user_notification", table => new
        {
            Id = table.Column<int>("int", nullable: false).Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
            UserId = table.Column<int>("int", nullable: false),
            LessonId = table.Column<int>("int", nullable: true),
            Type = table.Column<string>("varchar(64)", maxLength: 64, nullable: false),
            Title = table.Column<string>("varchar(200)", maxLength: 200, nullable: false),
            Text = table.Column<string>("varchar(2000)", maxLength: 2000, nullable: false),
            CreatedAt = table.Column<DateTime>("datetime(6)", nullable: false),
            SentAt = table.Column<DateTime>("datetime(6)", nullable: true),
            DeliveredAt = table.Column<DateTime>("datetime(6)", nullable: true),
            ReadAt = table.Column<DateTime>("datetime(6)", nullable: true),
            ReactedAt = table.Column<DateTime>("datetime(6)", nullable: true),
            Reaction = table.Column<string>("varchar(32)", maxLength: 32, nullable: false),
            Error = table.Column<string>("varchar(2000)", maxLength: 2000, nullable: true),
            Attempts = table.Column<int>("int", nullable: false)
        }, constraints: table =>
        {
            table.PrimaryKey("PK_spin_user_notification", x => x.Id);
        }).Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable("spin_user_notification_delivery", table => new
        {
            Id = table.Column<int>("int", nullable: false).Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
            UserNotificationId = table.Column<int>("int", nullable: false),
            UserDeviceId = table.Column<int>("int", nullable: false),
            SentAt = table.Column<DateTime>("datetime(6)", nullable: true),
            DeliveredAt = table.Column<DateTime>("datetime(6)", nullable: true),
            Attempts = table.Column<int>("int", nullable: false),
            ProviderMessageId = table.Column<string>("varchar(256)", maxLength: 256, nullable: true),
            Error = table.Column<string>("varchar(2000)", maxLength: 2000, nullable: true)
        }, constraints: table =>
        {
            table.PrimaryKey("PK_spin_user_notification_delivery", x => x.Id);
            table.ForeignKey("FK_delivery_notification", x => x.UserNotificationId, "spin_user_notification", "Id", onDelete: ReferentialAction.Cascade);
            table.ForeignKey("FK_delivery_device", x => x.UserDeviceId, "spin_user_device", "Id", onDelete: ReferentialAction.Cascade);
        }).Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateIndex("IX_spin_user_device_PushToken", "spin_user_device", "PushToken", unique: true);
        migrationBuilder.CreateIndex("IX_spin_user_device_UserId_Active", "spin_user_device", new[] { "UserId", "Active" });
        migrationBuilder.CreateIndex("IX_spin_user_notification_LessonId_UserId_Type", "spin_user_notification", new[] { "LessonId", "UserId", "Type" }, unique: true);
        migrationBuilder.CreateIndex("IX_spin_user_notification_UserId_CreatedAt", "spin_user_notification", new[] { "UserId", "CreatedAt" });
        migrationBuilder.CreateIndex("IX_spin_user_notification_SentAt_Attempts", "spin_user_notification", new[] { "SentAt", "Attempts" });
        migrationBuilder.CreateIndex("UX_notification_delivery_notification_device", "spin_user_notification_delivery", new[] { "UserNotificationId", "UserDeviceId" }, unique: true);
        migrationBuilder.CreateIndex("IX_spin_user_notification_delivery_UserDeviceId", "spin_user_notification_delivery", "UserDeviceId");
        migrationBuilder.CreateIndex("IX_spin_user_notification_delivery_SentAt_Attempts", "spin_user_notification_delivery", new[] { "SentAt", "Attempts" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable("spin_user_notification_delivery");
        migrationBuilder.DropTable("spin_user_device");
        migrationBuilder.DropTable("spin_user_notification");
    }
}
