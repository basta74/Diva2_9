ALTER TABLE `spin_pobocka`
	ADD COLUMN `pokladnaId` INT(3) NOT NULL DEFAULT 1 AFTER `minuty`,
	ADD COLUMN `motivHlavni` VARCHAR(20) NOT NULL AFTER `color2`,
	ADD COLUMN `motivKontra` VARCHAR(20) NOT NULL AFTER `motivHlavni`;