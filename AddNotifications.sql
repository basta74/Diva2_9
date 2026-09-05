-- MySQL / MariaDB
-- Vytvori tri tabulky notifikaci podle EF migrace 20260822162949_AddNotifications.

CREATE TABLE IF NOT EXISTS `spin_user_device` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `UserId` int NOT NULL,
    `PushToken` varchar(512) NOT NULL,
    `Platform` varchar(16) NOT NULL,
    `RegisteredAt` datetime(6) NOT NULL,
    `LastSeenAt` datetime(6) NOT NULL,
    `Active` tinyint(1) NOT NULL,
    CONSTRAINT `PK_spin_user_device` PRIMARY KEY (`Id`),
    UNIQUE KEY `IX_spin_user_device_PushToken` (`PushToken`),
    KEY `IX_spin_user_device_UserId_Active` (`UserId`, `Active`)
) CHARACTER SET=utf8mb4;

CREATE TABLE IF NOT EXISTS `spin_user_notification` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `UserId` int NOT NULL,
    `LessonId` int NULL,
    `Type` varchar(64) NOT NULL,
    `Title` varchar(200) NOT NULL,
    `Text` varchar(2000) NOT NULL,
    `CreatedAt` datetime(6) NOT NULL,
    `SentAt` datetime(6) NULL,
    `DeliveredAt` datetime(6) NULL,
    `ReadAt` datetime(6) NULL,
    `ReactedAt` datetime(6) NULL,
    `Reaction` varchar(32) NOT NULL,
    `Error` varchar(2000) NULL,
    `Attempts` int NOT NULL,
    CONSTRAINT `PK_spin_user_notification` PRIMARY KEY (`Id`),
    UNIQUE KEY `IX_spin_user_notification_LessonId_UserId_Type` (`LessonId`, `UserId`, `Type`),
    KEY `IX_spin_user_notification_UserId_CreatedAt` (`UserId`, `CreatedAt`),
    KEY `IX_spin_user_notification_SentAt_Attempts` (`SentAt`, `Attempts`)
) CHARACTER SET=utf8mb4;

CREATE TABLE IF NOT EXISTS `spin_user_notification_delivery` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `UserNotificationId` int NOT NULL,
    `UserDeviceId` int NOT NULL,
    `SentAt` datetime(6) NULL,
    `DeliveredAt` datetime(6) NULL,
    `Attempts` int NOT NULL,
    `ProviderMessageId` varchar(256) NULL,
    `Error` varchar(2000) NULL,
    CONSTRAINT `PK_spin_user_notification_delivery` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_delivery_notification`
        FOREIGN KEY (`UserNotificationId`) REFERENCES `spin_user_notification` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_delivery_device`
        FOREIGN KEY (`UserDeviceId`) REFERENCES `spin_user_device` (`Id`) ON DELETE CASCADE,
    UNIQUE KEY `UX_notification_delivery_notification_device` (`UserNotificationId`, `UserDeviceId`),
    KEY `IX_spin_user_notification_delivery_UserDeviceId` (`UserDeviceId`),
    KEY `IX_spin_user_notification_delivery_SentAt_Attempts` (`SentAt`, `Attempts`)
) CHARACTER SET=utf8mb4;

-- Oznaci EF migraci jako provedenou. Tento zapis ponechte az za uspesnym vytvorenim vsech tabulek.
CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
SELECT '20260822162949_AddNotifications', '9.0.15'
WHERE NOT EXISTS (
    SELECT 1
    FROM `__EFMigrationsHistory`
    WHERE `MigrationId` = '20260822162949_AddNotifications'
);
