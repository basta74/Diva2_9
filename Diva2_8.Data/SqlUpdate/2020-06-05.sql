
/*
ALTER TABLE `spin_user_zbytek_kredit`
	ADD COLUMN `id` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT FIRST,
	CHANGE COLUMN `userID` `userID` INT(8) UNSIGNED NOT NULL AFTER `id`,
	CHANGE COLUMN `pokladnaID` `pokladnaID` TINYINT UNSIGNED NOT NULL DEFAULT 0 AFTER `userID`,
	ADD PRIMARY KEY (`id`);

	
	ALTER TABLE `spin_user_transakce`
	ADD COLUMN `videoID` INT(6) NULL AFTER `spinID`;

	ALTER TABLE `spin_ini_main`
	ADD COLUMN `UseVideo` BIT NOT NULL DEFAULT 0 AFTER `GatePays`;

	ALTER TABLE `videa`
	ADD COLUMN `kredity` SMALLINT UNSIGNED NOT NULL DEFAULT 0 AFTER `desc`;



CREATE TABLE IF NOT EXISTS `users_videa` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `userID` int(11) NOT NULL DEFAULT 0,
  `videoID` int(11) NOT NULL DEFAULT 0,
  `nazev` varchar(250) NOT NULL DEFAULT '0',
  `url` varchar(250) NOT NULL DEFAULT '0',
  `kredity` int(11) NOT NULL DEFAULT 0,
  `zaplaceno` bit(1) NOT NULL DEFAULT b'0',
  `zaplacenoDt` datetime DEFAULT NULL,
  `aktivovano` bit(1) NOT NULL DEFAULT b'0',
  `aktivovanoDt` datetime DEFAULT NULL,
  `zobrazeno` bit(1) NOT NULL DEFAULT b'0',
  `zobrazenoDt` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `userID` (`userID`),
  KEY `videoID` (`videoID`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

2020-07-22

ALTER TABLE `pages`
	ADD COLUMN `controller` VARCHAR(50) NOT NULL DEFAULT 'Home' AFTER `urlName`,
	ADD COLUMN `method` VARCHAR(50) NOT NULL DEFAULT 'Index' AFTER `controller`;

UPDATE `pages` SET `controller`='Video' WHERE  `id`= 3;
UPDATE `pages` SET `method`='About' WHERE  `id`= 2;
UPDATE `pages` SET `method`='Info' WHERE  `id`= 1;

ALTER TABLE `users_videa`
		ADD COLUMN `image` VARCHAR(200) NOT NULL AFTER `url`;

ALTER TABLE `videa`
	ADD COLUMN `image` VARCHAR(200) NOT NULL AFTER `url`;

	ALTER TABLE `videa`
	CHANGE COLUMN `image` `image` VARCHAR(200) NOT NULL DEFAULT '/images/default-video-thumbnail.jpg' COLLATE 'utf8_czech_ci' AFTER `url`;


--------- 2020-07-26 ---------------------

	ALTER TABLE `spin_pobocka`
	ADD COLUMN `pobockaType` TINYINT(3) UNSIGNED NOT NULL DEFAULT 0 AFTER `poradi`,
	ADD COLUMN `calendarSetting` TEXT NOT NULL DEFAULT '' AFTER `pobockaType`;



CREATE TABLE IF NOT EXISTS `cal_event` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `pobockaId` tinyint(3) unsigned NOT NULL,
  `drahaId` tinyint(3) unsigned NOT NULL,
  `day` tinyint(3) unsigned NOT NULL,
  `date` date NOT NULL,
  `from` smallint(5) unsigned NOT NULL DEFAULT 0,
  `to` smallint(5) unsigned NOT NULL DEFAULT 0,
  `createdUserId` int(10) unsigned NOT NULL,
  `createdDt` datetime NOT NULL,
  `ts` datetime NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=1 DEFAULT CHARSET=utf8 COLLATE=utf8_czech_ci;



	
    ----------- 2020-07-27 -----------------

    ALTER TABLE `spinhod`
	ADD COLUMN `minuta` SMALLINT UNSIGNED NULL DEFAULT NULL COMMENT 'minuta ve dni' AFTER `hodina`;

    ALTER TABLE `spinhod_sablona`
	ADD COLUMN `minuta` SMALLINT UNSIGNED NULL DEFAULT NULL COMMENT 'minuta ve dni' AFTER `hodina`;


    ALTER TABLE `spin_ini_zacatek`
	ADD COLUMN `id` SMALLINT UNSIGNED NOT NULL AUTO_INCREMENT FIRST,
	ADD COLUMN `minuta` SMALLINT UNSIGNED NOT NULL DEFAULT 0 AFTER `Zacatek`,
	ADD PRIMARY KEY (`id`);


    UPDATE spin_ini_zacatek z
    SET z.minuta =  MINUTE(CONVERT(z.Zacatek, TIME)) + (Hour(CONVERT(z.Zacatek, TIME))*60);

	----------- 2020-07-30 -----------------
	
	DROP TABLE IF EXISTS cal_ini_minutes;

CREATE TABLE IF NOT EXISTS `cal_ini_minutes` (
  `pobockaId` tinyint(3) unsigned NOT NULL,
  `minute` smallint(5) unsigned NOT NULL DEFAULT 0,
  `time` time DEFAULT '00:00:00',
  `visible` bit(1) NOT NULL DEFAULT b'0',
  UNIQUE KEY `Index 1` (`pobockaId`,`minute`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

	UPDATE spinhod SET `minuta`= NULL;
	

    UPDATE spinhod h
	JOIN spin_ini_zacatek z ON z.Poradi = h.hodina AND z.Pobocka = h.pobocka
	SET h.minuta = z.minuta;

	UPDATE spinhod_sablona SET `minuta`= NULL;
	

	UPDATE spinhod_sablona h
	JOIN spin_ini_zacatek z ON z.Poradi = h.hodina AND z.Pobocka = h.pobocka
	SET h.minuta = z.minuta;



ALTER TABLE `spinhod`
	DROP INDEX `pobocka`,
	ADD UNIQUE INDEX `pobocka` (`pobocka`, `zdroj`, `rok`, `tyden`, `denVTydnu`, `minuta`) USING BTREE;

ALTER TABLE `spinhod_sablona`
	DROP INDEX `pobocka`,
	ADD UNIQUE INDEX `pobocka` (`pobocka`, `zdroj`, `denVTydnu`, `minuta`) USING BTREE;
