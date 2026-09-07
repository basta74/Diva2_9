-- MySQL / MariaDB
-- Tabulka pro chyby odeslane z mobilni aplikace Diva2Maui.

CREATE TABLE IF NOT EXISTS `spin_mobile_app_error` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `UserId` int NOT NULL,
    `LessonId` int NULL,
    `OccurredAt` datetime(6) NOT NULL,
    `ReceivedAt` datetime(6) NOT NULL,
    `Platform` varchar(32) NOT NULL,
    `OsVersion` varchar(64) NOT NULL,
    `DeviceManufacturer` varchar(100) NOT NULL,
    `DeviceModel` varchar(100) NOT NULL,
    `AppVersion` varchar(32) NOT NULL,
    `AppBuild` varchar(32) NOT NULL,
    `Page` varchar(200) NOT NULL,
    `Action` varchar(200) NOT NULL,
    `ExceptionType` varchar(500) NOT NULL,
    `Message` varchar(4000) NOT NULL,
    `StackTrace` longtext NOT NULL,
    `InnerException` longtext NOT NULL,
    CONSTRAINT `PK_spin_mobile_app_error` PRIMARY KEY (`Id`),
    KEY `IX_spin_mobile_app_error_ReceivedAt` (`ReceivedAt`),
    KEY `IX_spin_mobile_app_error_UserId_ReceivedAt` (`UserId`, `ReceivedAt`),
    KEY `IX_spin_mobile_app_error_LessonId_ReceivedAt` (`LessonId`, `ReceivedAt`)
) CHARACTER SET=utf8mb4;
