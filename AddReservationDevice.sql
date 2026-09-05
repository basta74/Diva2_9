-- MySQL / MariaDB
-- Spustte nad produkcni tenant databazi po migraci AddNotifications.

ALTER TABLE `spinuser`
    ADD COLUMN `device` tinyint unsigned NOT NULL DEFAULT 0
    COMMENT '0 = web, 1 = app';

-- Oznaci EF migraci jako provedenou. Tento zapis ponechte az za uspesnym ALTER TABLE.
INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
SELECT '20260823122203_AddReservationDevice', '9.0.15'
WHERE NOT EXISTS (
    SELECT 1
    FROM `__EFMigrationsHistory`
    WHERE `MigrationId` = '20260823122203_AddReservationDevice'
);
